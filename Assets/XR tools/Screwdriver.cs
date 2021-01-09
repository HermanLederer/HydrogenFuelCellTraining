using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screwdriver : MonoBehaviour
{
	private Screw selectedScrew;
	private float screwingDirection = 0f;

	private void OnTriggerEnter(Collider other)
	{
		Screw screw;
		if (other.TryGetComponent(out screw))
		{
			selectedScrew = screw;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (selectedScrew) if (other.gameObject == selectedScrew.gameObject) selectedScrew = null;
	}

	private void Update()
	{
		if (selectedScrew)
			selectedScrew.ScrewedOut += screwingDirection * Time.deltaTime;
	}

	public void StopScrewing()
	{
		screwingDirection = 0f;
	}

	public void StartScrewingIn()
	{
		screwingDirection = -2f;
	}

	public void StartScrewingOut()
	{
		screwingDirection = 2f;
	}
}
