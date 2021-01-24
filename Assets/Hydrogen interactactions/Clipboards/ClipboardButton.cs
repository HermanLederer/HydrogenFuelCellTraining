using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

namespace HydrogenInteractables
{
	public class ClipboardButton : MonoBehaviour
	{
		public AudioClip click;
		public UnityEvent onClicked;

		private void OnTriggerEnter(Collider other)
		{
			if (other.GetComponentInParent<XRRayInteractor>())
			{
				HL.AudioManagement.AudioManager.Instance.PlayIn3D(click, 1, transform.position, 0.1f, 1f);
				onClicked.Invoke();
				onClicked.RemoveAllListeners();
			}
		}
	}
}
