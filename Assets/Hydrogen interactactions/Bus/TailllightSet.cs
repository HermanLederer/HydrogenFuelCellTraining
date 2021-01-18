using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailllightSet : MonoBehaviour
{
    public Light redLight;
    private float sinvalue = 0f;
    private bool isAlarmOn = false;

    private void Update()
    {
        if (isAlarmOn == true) {
            redLight.intensity = Mathf.Sin(sinvalue) + 1;
            sinvalue += Time.deltaTime * 2;
        }
    }

    public void TurnOn()
    {
        redLight.intensity = 1f;
    }

    public void TurnOff()
    {
        redLight.intensity = 0f;
    }

    public void StartAlarm()
    {
        this.isAlarmOn = true;
    }

    public void StopAlarm()
    {
        isAlarmOn = false;
    }

}
