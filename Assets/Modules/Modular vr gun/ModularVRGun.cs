using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularVRGun : MonoBehaviour
{
	public ParticleSystem particleSystem;

	public void Shoot()
	{
		particleSystem.Play();
	}
}
