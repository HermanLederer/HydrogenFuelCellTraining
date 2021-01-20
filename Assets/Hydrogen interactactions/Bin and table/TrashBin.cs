using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HydrogenInteractables
{
	public class TrashBin : HydrogenSocketIntractor
	{
		[Header("TrashBin")]
		public AudioClip trashAudio;

		protected override void Awake()
		{
			base.Awake();

			onSelectEntered.AddListener(ThrowAway);
		}

		new protected void OnTriggerEnter(Collider col)
		{
			HydrogenInteractable interactable = col.gameObject.GetComponentInParent<HydrogenInteractable>();
			if (interactable)
				base.OnTriggerEnter(col);
		}

		new protected void OnTriggerExit(Collider col)
		{
			HydrogenInteractable interactable = col.gameObject.GetComponentInParent<HydrogenInteractable>();
			if (interactable)
				base.OnTriggerExit(col);
		}

		private void ThrowAway(XRBaseInteractable interactable)
		{
			HL.AudioManagement.AudioManager.Instance.PlayIn3D(trashAudio, 1f, attachTransform.position, 0.1f, 3f);
			Destroy(interactable.gameObject);
		}
	}
}