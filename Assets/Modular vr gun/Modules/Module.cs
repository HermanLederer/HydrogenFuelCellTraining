using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class Module : HydrogenInteractable
{
	private ParticleSystem fireParticles;
	[SerializeField] private ParticleSystem attachParticles;
	[SerializeField] private ParticleSystem firePrefab;
	[SerializeField] new private MeshRenderer renderer;

	new private Collider collider;
	private XRGrabInteractable xrGrabInteractable;

	public float power = 8f;

	private void Awake()
	{
		collider = GetComponent<Collider>();
		xrGrabInteractable = GetComponent<XRGrabInteractable>();
	}

	public void Shoot()
	{
		if (power > 0)
		{
			fireParticles.Play();
			power -= 1f;
		}

		if (power <= 0)
		{
			renderer.material.SetColor("_EmissionColor", Color.black);
		}
	}

	public void AttachToGun(Transform firePoint)
	{
		collider.enabled = false;
		fireParticles = Instantiate(firePrefab, firePoint);
		attachParticles.Play();
	}

	public void Detach()
	{
		collider.enabled = true;
		Destroy(fireParticles);
	}
}
