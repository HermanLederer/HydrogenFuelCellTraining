using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bus : MonoBehaviour
{
	// Editor fields
	public bool isPowered = false;
	public bool criticalMode = false;

	public PowerIndicators powerIndicators = null;

	[Header("Game components")]
	public MonoBehaviour battery;

	private void Awake()
	{
		if (!powerIndicators) Debug.LogWarning("power indicators object has not been assigend");
	}

	private void Start()
	{
		// generate filetrs with random conditions
	}

	public bool Confirm()
	{
		return true;
	}

	public void EmergencyShutDown()
	{
		PowerOff();
	}

	public void TogglePower()
	{
		if (isPowered) PowerOff();
		else PowerOn();
	}

	private void PowerOff()
	{
		powerIndicators.PowerOff();
		isPowered = false;
	}

	private void PowerOn()
	{
		powerIndicators.PowerOn();
		isPowered = true;
	}
}
