using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taillight : MonoBehaviour
{
	new public Light light;
	public Material material;

	private float originalLightIntensity;
	private Color originalEmmision;

	[Range(0f, 1f)]
	public float intensity = 1f;

	private void Awake()
	{
		originalLightIntensity = light.intensity;
		originalEmmision = material.GetColor("_EmissionColor");
	}

	public void Update()
	{
		light.intensity = originalLightIntensity * intensity;
		material.SetColor("_EmissionColor", originalEmmision * intensity);
	}
}
