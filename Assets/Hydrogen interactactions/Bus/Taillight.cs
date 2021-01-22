﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taillight : MonoBehaviour
{
	new public Light light;
	new public MeshRenderer renderer;

	private float originalLightIntensity;
	private Color originalEmmision;

	[Range(0f, 1f)]
	public float intensity = 1f;

	private void Awake()
	{
		originalLightIntensity = light.intensity;
		originalEmmision = renderer.material.GetColor("_EmissionColor");
	}

	public void Update()
	{
		light.intensity = originalLightIntensity * intensity;
		renderer.material.SetColor("_EmissionColor", originalEmmision * intensity);
	}
}