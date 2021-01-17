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

        //Flash orange light twice and slowly fade in the red lights
  
	}

	public void PowerOff()
	{
        Debug.Log("Power Off");

        //The ligts turn off
    }

    public void StartEmergency()
    {
        Debug.Log("Start Emergency");

        //Siren sound and red lights start flashing
        //The problem area starts to spark
    }

    public void StopEmergency()
    {
        Debug.Log("Stop Emergency");
        PowerOff();
        //Alarm stops playing
        //The particle effect turns off
    }

    public void EngineOn()
    {
        Debug.Log("Engine On");

        //Engine sound and victory music starts
    }   //

    
}
