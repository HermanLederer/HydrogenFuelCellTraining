using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HydrogenInteractables;

public class PowerIndicators : MonoBehaviour
{
	//
	// Editor fields
	public TailllightSet tailLightSetLeft;
	public TailllightSet tailLightSetRight;

	public ButtonInteractable emergencyButton;

	public AudioClip powerSound;
	public AudioClip alertSound;
	public Transform hornTransform;

	public AudioClip pressureReleaseSound;
	public Transform pressureSoundTransform;

	public AudioSource engineAudioSource;

	//
	// Variables
	private float nextAlertTime = 0f;
	private bool isAlarmOn = false;
	private float originalEmergencyButtonLightResting = 0f;

	private void Start()
	{
		originalEmergencyButtonLightResting = emergencyButton.LightResting;
	}

	private void Update()
	{
		if (isAlarmOn)
		{
			if (Time.time >= nextAlertTime)
			{
				StartCoroutine(AlertCorutine());
				nextAlertTime = Time.time + 2.66f;
			}
		}
	}

	IEnumerator AlertCorutine()
	{
		HL.AudioManagement.AudioManager.Instance.PlayIn3D(alertSound, 1f, hornTransform.position, 2f, 40f);

		for (int i = 0; i < 3; i++)
		{
			StartCoroutine(FadeLight(tailLightSetLeft.redTaillight, true, 0.165f));
			StartCoroutine(FadeLight(tailLightSetRight.redTaillight, true, 0.165f));
			emergencyButton.LightResting = emergencyButton.lightPressed;
			yield return new WaitForSeconds(0.165f);
			StartCoroutine(FadeLight(tailLightSetLeft.redTaillight, false, 0.165f));
			StartCoroutine(FadeLight(tailLightSetRight.redTaillight, false, 0.165f));
			emergencyButton.LightResting = originalEmergencyButtonLightResting;
			yield return new WaitForSeconds(0.165f);
		}
	}

	public void PowerOn() => StartCoroutine(PowerOnCorutine());

	public void PowerOn(bool silent)
	{
		if (silent)
		{
			tailLightSetLeft.redTaillight.intensity = 1f;
			tailLightSetRight.redTaillight.intensity = 1f;

			tailLightSetLeft.orangeTaillight.intensity = 0f;
			tailLightSetRight.orangeTaillight.intensity = 0f;
		}
		else PowerOn();
	}

	IEnumerator PowerOnCorutine()
	{
		HL.AudioManagement.AudioManager.Instance.PlayIn3D(powerSound, 1f, hornTransform.position, 2f, 40f);

		StartCoroutine(FadeLight(tailLightSetLeft.orangeTaillight, true, 0.1f));
		StartCoroutine(FadeLight(tailLightSetRight.orangeTaillight, true, 0.1f));
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(FadeLight(tailLightSetLeft.orangeTaillight, false, 0.1f));
		StartCoroutine(FadeLight(tailLightSetRight.orangeTaillight, false, 0.1f));
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(FadeLight(tailLightSetLeft.orangeTaillight, true, 0.1f));
		StartCoroutine(FadeLight(tailLightSetRight.orangeTaillight, true, 0.1f));
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(FadeLight(tailLightSetLeft.orangeTaillight, false, 0.1f));
		StartCoroutine(FadeLight(tailLightSetRight.orangeTaillight, false, 0.1f));
		yield return new WaitForSeconds(0.2f);

		StartCoroutine(FadeLight(tailLightSetLeft.redTaillight, true, 0.5f));
		StartCoroutine(FadeLight(tailLightSetRight.redTaillight, true, 0.5f));
	}

	public void PowerOff() => StartCoroutine(PowerOffCorutine());

	public void PowerOff(bool silent)
	{
		if (silent)
		{
			tailLightSetLeft.redTaillight.intensity = 0f;
			tailLightSetRight.redTaillight.intensity = 0f;

			tailLightSetLeft.orangeTaillight.intensity = 0f;
			tailLightSetRight.orangeTaillight.intensity = 0f;
		}
		else PowerOff();
	}

	IEnumerator PowerOffCorutine()
	{
		StopEmergency();

		StartCoroutine(StopEngineCorutine(0.5f, 0.5f));
		HL.AudioManagement.AudioManager.Instance.PlayIn3D(powerSound, 1f, hornTransform.position, 2f, 40f);

		StartCoroutine(FadeLight(tailLightSetLeft.orangeTaillight, true, 0.1f));
		StartCoroutine(FadeLight(tailLightSetRight.orangeTaillight, true, 0.1f));
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(FadeLight(tailLightSetLeft.orangeTaillight, false, 0.1f));
		StartCoroutine(FadeLight(tailLightSetRight.orangeTaillight, false, 0.1f));
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(FadeLight(tailLightSetLeft.orangeTaillight, true, 0.1f));
		StartCoroutine(FadeLight(tailLightSetRight.orangeTaillight, true, 0.1f));
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(FadeLight(tailLightSetLeft.orangeTaillight, false, 0.1f));
		StartCoroutine(FadeLight(tailLightSetRight.orangeTaillight, false, 0.1f));
		yield return new WaitForSeconds(0.2f);

		StartCoroutine(FadeLight(tailLightSetLeft.redTaillight, false, 0.5f));
		StartCoroutine(FadeLight(tailLightSetRight.redTaillight, false, 0.5f));

		//yield return new WaitForSeconds(0.5f);
		HL.AudioManagement.AudioManager.Instance.PlayIn3D(pressureReleaseSound, 0.2f, pressureSoundTransform.position, 0.5f, 4f);
	}

	public void StartEmergency()
	{
		isAlarmOn = true;
	}

	private void StopEmergency()
	{
		isAlarmOn = false;
		emergencyButton.LightResting = originalEmergencyButtonLightResting;
	}

	public void EmergencyPowerOff()
	{
		StopEmergency();
		tailLightSetLeft.redTaillight.intensity = 0f;
		tailLightSetRight.redTaillight.intensity = 0f;
		StartCoroutine(StopEngineCorutine(0f, 0.5f));
		HL.AudioManagement.AudioManager.Instance.PlayIn3D(pressureReleaseSound, 0.2f, pressureSoundTransform.position, 0.5f, 4f);
	}

	public void EngineOn()
	{
		engineAudioSource.volume = 1f;
		engineAudioSource.Play();
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

	IEnumerator StopEngineCorutine(float delay, float duration)
	{
		yield return new WaitForSeconds(delay);

		for (float t = 0f; t < duration; t += Time.deltaTime)
		{
			engineAudioSource.volume = Mathf.Lerp(1f, 0f, t / duration);
			yield return new WaitForEndOfFrame();
		}

		engineAudioSource.volume = 0f;
		engineAudioSource.Stop();
	}
}
