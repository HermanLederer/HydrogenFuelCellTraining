using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HydrogenInteractables
{
	public class ModularVRGun : MonoBehaviour
	{
		public ModuleSocket moduleSocket;
		new private Collider collider;

		private void Awake()
		{
			collider = GetComponent<Collider>();
		}

		public void Lock()
		{
			collider.enabled = false;
		}

		public void Unlock()
		{
			collider.enabled = true;
		}

		public void Shoot()
		{
			if (moduleSocket.selectedModule)
			{
				moduleSocket.selectedModule.Shoot();
			}
		}
	}
}
