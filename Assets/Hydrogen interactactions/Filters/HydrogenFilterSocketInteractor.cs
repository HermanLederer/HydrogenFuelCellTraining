using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HydrogenInteractables
{
	public class HydrogenFilterSocketInteractor : HydrogenSocketIntractor
	{
		public HydrogenInteractableType type;

		new protected void OnTriggerEnter(Collider col)
		{
			HydrogenFilter filter = col.gameObject.GetComponentInParent<HydrogenFilter>();
			if (filter)
				if (filter.type == type)
					base.OnTriggerEnter(col);
		}

		new protected void OnTriggerExit(Collider col)
		{
			HydrogenFilter filter = col.gameObject.GetComponentInParent<HydrogenFilter>();
			if (filter)
				if (filter.type == type)
					base.OnTriggerExit(col);
		}
	}
}