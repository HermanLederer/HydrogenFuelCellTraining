using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerIndicators : MonoBehaviour
{
    public TailllightSet tailLightSetLeft;
    public TailllightSet tailLightSetRight;

    public AudioClip powerSound;
    public Transform batterySoundTransform;

    public AudioClip pressureReleaseSound;
    public Transform pressureSoundTransform;

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

        HL.AudioManagement.AudioManager.Instance.PlayIn3D(powerSound, 1f, batterySoundTransform.position, 2f, 40f);
	}

    public void PowerOff() => StartCoroutine(PowerOffCorutine());

    IEnumerator PowerOffCorutine()
    {
        tailLightSetLeft.TurnOff();
        tailLightSetRight.TurnOff();

        HL.AudioManagement.AudioManager.Instance.PlayIn3D(powerSound, 1f, batterySoundTransform.position, 2f, 40f);
        yield return new WaitForSeconds(0.5f);
        HL.AudioManagement.AudioManager.Instance.PlayIn3D(pressureReleaseSound, 0.2f, pressureSoundTransform.position, 0.5f, 10f);
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
