using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HydrogenInteractables
{
	public class Bus : MonoBehaviour
	{
		// Editor fields
		public bool isPowered = true;

		[Header("Bus components")]
		public PowerIndicators powerIndicators = null;
		public ButtonInteractable confirmButton;
		public ButtonInteractable powerButton;
		public ButtonInteractable emergencyButton;
		public HydrogenFilterSocketInteractor largeFilterSocket;
		public HydrogenFilterSocketInteractor smallFilterSocket;

		[Header("Audio")]
		public AudioClip emergencySound;

		private void Awake()
		{
			if (!powerIndicators) Debug.LogError("power indicators object has not been assigend");
		}

		private void OnEnable()
		{
			// Filter listeners
			largeFilterSocket.onSelectExited.AddListener(processFilterRemoval);
			smallFilterSocket.onSelectExited.AddListener(processFilterRemoval);
		}

		private void OnDisable()
		{
			largeFilterSocket.onSelectExited.RemoveListener(processFilterRemoval);
			smallFilterSocket.onSelectExited.RemoveListener(processFilterRemoval);
		}

		private void Start()
		{
			// Power
			if (isPowered) PowerOn(true);
			else PowerOff(true);

			// Random filter conditions
			HydrogenFilter filter;
			filter = smallFilterSocket.selectTarget.GetComponent<HydrogenFilter>();
			filter.RandomizeCondition();
			if (filter.isInGoodCondition)
			{
				filter = largeFilterSocket.selectTarget.GetComponent<HydrogenFilter>();
				filter.isInGoodCondition = false;
			}
			else
			{
				filter = largeFilterSocket.selectTarget.GetComponent<HydrogenFilter>();
				filter.RandomizeCondition();
			}
		}

		public bool Confirm()
		{
			var largeFilterProblem = largeFilterSocket.PowerOn();
			var smallFilterProblem = smallFilterSocket.PowerOn();

			if (largeFilterProblem == HydrogenFilterSocketProblems.FilterMissing || smallFilterProblem == HydrogenFilterSocketProblems.FilterMissing)
			{
				// One or more filters are missing
				StartEmergency();
				return false;
			}
			else if (largeFilterProblem == HydrogenFilterSocketProblems.FilterInBadCondition || smallFilterProblem == HydrogenFilterSocketProblems.FilterInBadCondition)
			{
				// One or more filters are bad
				StartEmergency();
				return false;
			}

			// Good
			powerIndicators.EngineOn();
			return true;
		}

		private void processFilterRemoval(XRBaseInteractable interactable)
		{
			if (isPowered) StartEmergency();
		}

		public void StartEmergency()
		{
			HL.AudioManagement.AudioManager.Instance.PlayIn3D(emergencySound, 0.4f, confirmButton.transform.position, 0.5f, 2f);
			powerIndicators.StartEmergency();
			confirmButton.Deactivate(0, 0.5f);
			powerButton.Deactivate(0, 0.5f);
		}

		public void EmergencyShutDown()
		{
			largeFilterSocket.PowerOff();
			smallFilterSocket.PowerOff();

			powerIndicators.EmergencyPowerOff();

			confirmButton.Deactivate(0f, 0f);
			emergencyButton.Deactivate(0f, 0.5f);
			powerButton.Activate(1f, 1f);

			isPowered = false;
		}

		public void TogglePower()
		{
			if (isPowered) PowerOff();
			else PowerOn();
		}

		private void PowerOff(bool silent = false)
		{
			largeFilterSocket.PowerOff();
			smallFilterSocket.PowerOff();

			powerIndicators.PowerOff(silent);

			if (silent)
			{
				confirmButton.Deactivate(0f, 0f);
				emergencyButton.Deactivate(0f, 0f);
			}
			else
			{
				confirmButton.Deactivate(0.5f, 0.5f);
				emergencyButton.Deactivate(0.5f, 0.5f);
			}

			isPowered = false;
		}

		private void PowerOn(bool silent = false)
		{
			largeFilterSocket.PowerOn(true);
			smallFilterSocket.PowerOn(true);

			powerIndicators.PowerOn(silent);

			if (silent)
			{
				confirmButton.Activate(0f, 0f);
				emergencyButton.Activate(0f, 0f);
			}
			else
			{
				confirmButton.Activate(0.5f, 0.5f);
				emergencyButton.Activate(0.5f, 0.5f);
			}
			
			isPowered = true;
		}
	}
}
