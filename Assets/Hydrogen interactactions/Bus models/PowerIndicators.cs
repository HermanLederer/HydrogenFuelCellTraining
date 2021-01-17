using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerIndicators : MonoBehaviour
{

    public TailllightSet tailLightSetLeft;
    public TailllightSet tailLightSetRight;
    public AudioClip batteryOnSound;
    public Transform batterySoundTransform;

    private void Start()
	{

	}

	private void Update()
	{

	}

    public void PowerOn()
    {
        tailLightSetLeft.TurnOn();
        tailLightSetRight.TurnOn();

        HL.AudioManagement.AudioManager.Instance.PlayIn3D(batteryOnSound, 1f, batterySoundTransform.position, 0.5f, 3f);

        // To do flashing animation
	}

	public void PowerOff()
	{
        tailLightSetLeft.TurnOff();
        tailLightSetRight.TurnOff();
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
