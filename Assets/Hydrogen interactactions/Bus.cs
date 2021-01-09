using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bus : MonoBehaviour
{
	// Editor fields
	public bool isPowered = false;
	public bool criticalMode = false;

	[Header("Game components")]
	public MonoBehaviour battery;

	private void Awake()
	{
		
	}

	private void Start()
	{
		// generate filetrs with random conditions
	}

	public void Confirm()
	{
		//if (isPowered)
	}

	public void EmergencyShutDown()
	{
		//if (isPowered)
	}
}
