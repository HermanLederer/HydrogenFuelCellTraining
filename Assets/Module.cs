using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class Module : MonoBehaviour
{
	[SerializeField] private ParticleSystem fireParticles;
	[SerializeField] private ParticleSystem attachParticles;
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

	public void Lock()
	{
		collider.enabled = false;
	}

	public void Unlock()
	{
		collider.enabled = true;
	}

	public void Detach()
	{
		Debug.Log("detach");
	}
}
