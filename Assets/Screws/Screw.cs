using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Screw : MonoBehaviour
{
	public Transform screwhole = null;
	public float screwedOutMax = 1f;
	public float screwOffset = 0f;

	private float screwedOut = 1f;
	public float ScrewedOut
	{
		get { return screwedOut; }
		set { screwedOut = Mathf.Clamp(value, 0.0001f, 1f); }
	}

	private void Update()
	{
		transform.position = screwhole.position + transform.up * (ScrewedOut * (screwedOutMax - screwOffset) + screwOffset);
	}
}
