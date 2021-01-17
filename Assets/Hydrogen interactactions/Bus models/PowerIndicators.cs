using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerIndicators : MonoBehaviour
{
	private void Start()
	{

	}

	private void Update()
	{

	}

    public void PowerOn()
    {
        Debug.Log("Power On");
	}

	public void PowerOff()
	{
        Debug.Log("Power Off");
    }

    public void StartEmergency()
    {
        Debug.Log("Start Emergency");
    }

    public void StopEmergency()
    {
        Debug.Log("Stop Emergency");
    }

    public void EngineOn()
    {
        Debug.Log("Engine On");
    }

    
}
