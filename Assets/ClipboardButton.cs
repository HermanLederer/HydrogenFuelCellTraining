using UnityEngine;
using UnityEngine.Events;

namespace HydrogenInteractables
{
	public enum ClipboardActions
	{
		Restart
	}

	public class ClipboardButton : MonoBehaviour
	{
		public ClipboardActions action;
		public GameManager gameManager;

		public UnityEvent onClicked;

		private void OnTriggerEnter(Collider other)
		{
			switch (action)
			{
				case ClipboardActions.Restart:
					onClicked.Invoke();
					break;
			}
		}
	}
}
