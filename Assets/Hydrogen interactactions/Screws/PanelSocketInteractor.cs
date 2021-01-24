using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HydrogenInteractables
{
	public class PanelSocketInteractor : HydrogenSocketIntractor
	{
		public XRSocketInteractor filterSocket;

		protected override void Start()
		{
			base.Start();

			onSelectEntered.AddListener(SlotIn);
			onSelectExited.AddListener(SlotOut);
		}

		new protected void OnTriggerEnter(Collider col)
		{
			PanelWithScrews filter = col.gameObject.GetComponentInParent<PanelWithScrews>();
			if (filter)
				base.OnTriggerEnter(col);
		}

		new protected void OnTriggerExit(Collider col)
		{
			PanelWithScrews filter = col.gameObject.GetComponentInParent<PanelWithScrews>();
			if (filter)
				base.OnTriggerExit(col);
		}

		private void SlotIn(XRBaseInteractable i)
		{
			if (!i) return;

			if (filterSocket.selectTarget) filterSocket.selectTarget.colliders[0].enabled = false;
			else filterSocket.socketActive = false;
			i.GetComponent<PanelWithScrews>().unlocked = false;
		}

		private void SlotOut(XRBaseInteractable i)
		{
			if (!i) return;

			if (filterSocket.selectTarget) filterSocket.selectTarget.colliders[0].enabled = true;
			else filterSocket.socketActive = true;
			i.GetComponent<PanelWithScrews>().unlocked = true;
		}

		protected override void OnDestroy()
		{
			onSelectEntered.RemoveListener(SlotIn);
			onSelectExited.RemoveListener(SlotOut);

			base.OnDestroy();
		}
	}
}
