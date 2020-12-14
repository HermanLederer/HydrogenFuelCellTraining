using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class InscatteringVolume : MonoBehaviour
{
	public Material material;

	private void Update()
	{
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.x, transform.localScale.x);

		material.SetVector("_VolumePos", transform.position);
		material.SetFloat("_VolumeRadius", transform.localScale.x / 2f);
	}
}
