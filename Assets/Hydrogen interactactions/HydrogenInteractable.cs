using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

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

	public class HydrogenInteractableEvent : UnityEvent<HydrogenInteractable>
	{
	}

	public class HydrogenInteractable : MonoBehaviour
	{
		public HydrogenInteractableEvent onHydrogenInteractableDestroyed;
		public HydrogenInteractableType type;

		protected virtual void Awake()
		{
			if(onHydrogenInteractableDestroyed == null)
				onHydrogenInteractableDestroyed = new HydrogenInteractableEvent();
		}

		protected virtual void OnDestroy()
		{
			if (onHydrogenInteractableDestroyed != null) onHydrogenInteractableDestroyed.Invoke(this);
		}
	}
}
