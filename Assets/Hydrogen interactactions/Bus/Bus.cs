using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HydrogenInteractables
{
	public class Bus : MonoBehaviour
	{
		// Editor fields
		public bool isPowered = true;

		public PowerIndicators powerIndicators = null;

		[Header("Bus components")]
		public ButtonInteractable confirmButton;
		public ButtonInteractable powerButton;
		public ButtonInteractable emergencyButton;
		public HydrogenFilterSocketInteractor largeFilterSocket;
		public HydrogenFilterSocketInteractor smallFilterSocket;

		private void Awake()
		{
			if (!powerIndicators) Debug.LogError("power indicators object has not been assigend");
		}

		private void Start()
		{
			// Power
			if (isPowered) powerIndicators.PowerOn(true);
			else powerIndicators.PowerOff(true);

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

		public void StartEmergency()
		{
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

		private void PowerOff()
		{
			powerIndicators.PowerOff();
			confirmButton.Deactivate(0.5f, 0.5f);
			emergencyButton.Deactivate(0.5f, 0.5f);
			isPowered = false;
		}

		private void PowerOn()
		{
			powerIndicators.PowerOn();
			confirmButton.Activate(0.5f, 0.5f);
			emergencyButton.Activate(0.5f, 0.5f);
			isPowered = true;
		}
	}
}
