﻿using System.Collections;
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
		Screwdriver
	}

	public class HydrogenInteractable : MonoBehaviour
	{
		//public delegate void InteractableDestroyedEventHandler(object source, EventArgs args);
		//public event InteractableDestroyedEventHandler InteractableDestroyed;
		public event Action<HydrogenInteractable> OnHydrogenInteractableDestroyed;

		protected virtual void OnDestroy()
		{
			OnHydrogenInteractableDestroyed?.Invoke(this);
		}
	}
}
