using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailllightSet : MonoBehaviour
{
	//
	// Editor fields
	public Taillight redTaillight = null;
	public Taillight orangeTaillight = null;

	//
	// Variables
	private float sinvalue = 0f;
	private bool isAlarmOn = false;

	private void Update()
	{
		if (isAlarmOn)
		{
			redTaillight.intensity = Mathf.Sin(sinvalue) + 1;
			sinvalue += Time.deltaTime * 2;
		}
	}

	public void TurnOn() => StartCoroutine(TurnOnCorutine());

	public void TurnOff() => StartCoroutine(TurnOffCorutine());

	public void StartAlarm()
	{
		isAlarmOn = true;
		sinvalue = Mathf.PI;
	}

	public void StopAlarm()
	{
		isAlarmOn = false;
	}

	IEnumerator TurnOnCorutine()
	{
		StartCoroutine(FadeLight(orangeTaillight, true, 0.1f));
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(FadeLight(orangeTaillight, false, 0.1f));
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(FadeLight(orangeTaillight, true, 0.1f));
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(FadeLight(orangeTaillight, false, 0.1f));
		yield return new WaitForSeconds(0.2f);

		StartCoroutine(FadeLight(redTaillight, true, 0.5f));
	}

	IEnumerator TurnOffCorutine()
	{
		StartCoroutine(FadeLight(orangeTaillight, true, 0.1f));
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(FadeLight(orangeTaillight, false, 0.1f));
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(FadeLight(orangeTaillight, true, 0.1f));
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(FadeLight(orangeTaillight, false, 0.1f));
		yield return new WaitForSeconds(0.2f);

		StartCoroutine(FadeLight(redTaillight, false, 1f));
	}

	IEnumerator FadeLight(Taillight light, bool fadeIn, float duration)
	{
		if (fadeIn)
		{
			for (float t = 0f; t < duration; t += Time.deltaTime)
			{
				light.intensity = Mathf.Lerp(0f, 1f, t / duration);
				yield return new WaitForEndOfFrame();
			}

			light.intensity = 1f;
		}
		else // fade out
		{
			for (float t = 0f; t < duration; t += Time.deltaTime)
			{
				light.intensity = Mathf.Lerp(1f, 0f, t / duration);
				yield return new WaitForEndOfFrame();
			}

			light.intensity = 0f;
		}
	}
}
