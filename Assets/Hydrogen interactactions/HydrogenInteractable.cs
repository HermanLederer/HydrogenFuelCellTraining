using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace HydrogenInteractables
{
	public enum HydrogenInteractableType
	{
		LargeFilter,
		SmallFilter,
		GunModule,
		InstructionsClipboard,
		PlayAgainClipboard
	}

	public class HydrogenInteractable : MonoBehaviour
	{
		public event Action<HydrogenInteractable> OnHydrogenInteractableDestroyed;
		public HydrogenInteractableType type;

		protected virtual void OnDestroy()
		{
			OnHydrogenInteractableDestroyed?.Invoke(this);
		}
	}
}
