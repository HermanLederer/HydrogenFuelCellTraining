using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using HydrogenInteractables;

public class GameManager : MonoBehaviour
{
	//
	// Editor fields
	[Header("Game elements")]
	public Bus bus;
	public Volume postProcessingVolume;
	public AudioSource winMusic;
	[Header("Prefabs")]
	public GameObject largeFilterPrefab;
	public GameObject smallFilterPrefab;
	public GameObject gunModulePrefab;
	public GameObject instructionsClipboard;
	public GameObject playAgainClipboard;
	[Header("Sounds")]
	public AudioClip loseSound;
	public AudioClip spawnSound;

	public Transform spawnTransform;

	//
	// Variables
	private FadeAndFocus m_FadeAndFocusComponent;

	private void Awake()
	{
		FadeAndFocus tmp;
		if (postProcessingVolume.profile.TryGet(out tmp))
		{
			m_FadeAndFocusComponent = tmp;
			FadeView(false);
		}
		else
			Debug.LogError("FadeAndFocus component not found on " + postProcessingVolume.name);
	}

	private void Start()
	{
		// Registering destroy events
		foreach (HydrogenInteractable interactable in FindObjectsOfType<HydrogenInteractable>())
		{
			interactable.onHydrogenInteractableDestroyed.AddListener(OnInteractableDestroyed);
		}

		StartCoroutine(StartGameCorutine());
	}

	IEnumerator StartGameCorutine()
	{
		FadeView(1f, 1f, true);
		yield return new WaitForSeconds(2f);

		//SpawnNewHydrogenInteractable(HydrogenInteractableType.InstructionsClipboard);
		SpawnNewHydrogenInteractable(HydrogenInteractableType.PlayAgainClipboard);
	}

	public void Confirm()
	{
		if (!winMusic.isPlaying)
		{
			if (bus.Confirm())
			{
				// win
				winMusic.Play();
				SpawnNewHydrogenInteractable(HydrogenInteractableType.PlayAgainClipboard);
			}
			else
			{
				// lose
				//HL.AudioManagement.AudioManager.Instance.PlayIn2D(loseSound, 0.5f);
			}
		}
	}

	public void SpawnNewHydrogenInteractable(HydrogenInteractableType type) => StartCoroutine(SpawnNewHydrogenInteractableCorutine(type, 1f));

	IEnumerator SpawnNewHydrogenInteractableCorutine(HydrogenInteractableType type, float delay)
	{
		yield return new WaitForSeconds(delay);

		Vector3 spawnPos = transform.position;

		HydrogenInteractable interactable;
		switch (type)
		{
			case HydrogenInteractableType.LargeFilter:
				interactable = Instantiate(largeFilterPrefab, spawnPos, Quaternion.identity).GetComponent<HydrogenInteractable>();
				interactable.onHydrogenInteractableDestroyed.AddListener(OnInteractableDestroyed);
				break;
			case HydrogenInteractableType.SmallFilter:
				interactable = Instantiate(smallFilterPrefab, spawnPos, Quaternion.identity).GetComponent<HydrogenInteractable>();
				interactable.onHydrogenInteractableDestroyed.AddListener(OnInteractableDestroyed);
				break;
			case HydrogenInteractableType.GunModule:
				interactable = Instantiate(gunModulePrefab, spawnPos, Quaternion.identity).GetComponent<HydrogenInteractable>();
				interactable.onHydrogenInteractableDestroyed.AddListener(OnInteractableDestroyed);
				break;
			case HydrogenInteractableType.InstructionsClipboard:
				Instantiate(instructionsClipboard, spawnPos, Quaternion.identity);
				break;
			case HydrogenInteractableType.PlayAgainClipboard:
				var button = Instantiate(playAgainClipboard, spawnPos, Quaternion.identity).GetComponentInChildren<ClipboardButton>();
				button.onClicked.AddListener(Restart);
				break;
		}

		HL.AudioManagement.AudioManager.Instance.PlayIn3D(spawnSound, 1f, spawnPos, 1f, 10f);
	}

	public void OnInteractableDestroyed(HydrogenInteractable interactable)
	{
		//if (interactable.type == HydrogenInteractableType.LargeFilter || interactable.type == HydrogenInteractableType.SmallFilter)
		//{
		//	HydrogenFilter filter;
		//	filter = (HydrogenFilter)interactable;
		//	if (filter.isInGoodCondition) Debug.Log("Baaad! You baaad");
		//}

		SpawnNewHydrogenInteractable(interactable.type);
	}

	public void Restart()
	{
		// Unsubscribing GameManager from destroy events
		foreach (HydrogenInteractable interactable in FindObjectsOfType<HydrogenInteractable>())
		{
			interactable.onHydrogenInteractableDestroyed.RemoveAllListeners();
		}

		// Unsubscribing filter sockets from everything
		foreach (XRSocketInteractor interactable in FindObjectsOfType<XRSocketInteractor>())
		{
			interactable.onSelectExited.RemoveAllListeners();
			interactable.onSelectEntered.RemoveAllListeners();
		}

		StartCoroutine(RestartCorutine());
	}

	IEnumerator RestartCorutine()
	{
		FadeView(0f, 1f, false);
		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	private void FadeView(bool fadeIn)
	{
		m_FadeAndFocusComponent.fade.value = fadeIn ? 0f : 1f;
	}

	private void FadeView(float delay, float duration, bool fadeIn) => StartCoroutine(FadeViewCorutine(delay, duration, fadeIn));

	IEnumerator FadeViewCorutine(float delay, float duration, bool fadeIn)
	{
		yield return new WaitForSeconds(delay);

		if (!fadeIn)
		{
			for (float t = 0f; t < duration; t += Time.deltaTime)
			{
				m_FadeAndFocusComponent.fade.value = Mathf.Lerp(0f, 1f, t / duration);
				yield return new WaitForEndOfFrame();
			}

			m_FadeAndFocusComponent.fade.value = 1f;
		}
		else // fade out
		{
			for (float t = 0f; t < duration; t += Time.deltaTime)
			{
				m_FadeAndFocusComponent.fade.value = Mathf.Lerp(1f, 0f, t / duration);
				yield return new WaitForEndOfFrame();
			}

			m_FadeAndFocusComponent.fade.value = 0f;
		}
	}

	private void FocusView(bool focus)
	{
		m_FadeAndFocusComponent.focus.value = focus ? 0f : 1f;
	}
}
