using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularVRGun : MonoBehaviour
{
	public ModuleSocket moduleSocket;

	public void Shoot()
	{
		//particleSystem.Play();
		if (moduleSocket.selectedModule) moduleSocket.selectedModule.Shoot();
		moduleSocket.drop = true;
	}
}
