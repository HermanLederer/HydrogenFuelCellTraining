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
			if (largeFilterSocket.selectTarget && smallFilterSocket.selectTarget)
			{
				HydrogenFilter largeFilter = largeFilterSocket.selectTarget.GetComponent<HydrogenFilter>();
				HydrogenFilter smallFilter = smallFilterSocket.selectTarget.GetComponent<HydrogenFilter>();

				if (largeFilter.isInGoodCondition && smallFilter.isInGoodCondition)
				{
					powerIndicators.EngineOn();
					return true;
				}
				else
				{
					// One or more filters are bad
					powerIndicators.StartEmergency();
				}
			}
			else
			{
				// One or more filters are missing
				powerIndicators.StartEmergency();
			}

			return false;
		}

		public void EmergencyShutDown()
		{
			powerIndicators.StopEmergency();
		}

		public void TogglePower()
		{
			if (isPowered) PowerOff();
			else PowerOn();
		}

		private void PowerOff()
		{
			powerIndicators.StopEmergency();
			powerIndicators.PowerOff();
			confirmButton.Deactivate(0.5f, 0.5f);
			isPowered = false;
		}

		private void PowerOn()
		{
			powerIndicators.PowerOn();
			confirmButton.Activate(0.5f, 0.5f);
			isPowered = true;
		}
	}
}
