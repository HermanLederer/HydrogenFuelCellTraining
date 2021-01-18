using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HydrogenInteractables
{
	public class TrashBin : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			HydrogenInteractable interactable;
			if (other.TryGetComponent<HydrogenInteractable>(out interactable))
			{
				Destroy(interactable.gameObject);
			}
		}
	}
}