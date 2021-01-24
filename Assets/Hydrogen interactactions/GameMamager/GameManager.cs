using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HydrogenInteractables;

public class GameManager : MonoBehaviour
{
	// Editor fields
	[Header("Game elements")]
	public Bus bus;
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

	private void Start()
	{
		// Registering destroy events
		foreach (HydrogenInteractable interactable in FindObjectsOfType<HydrogenInteractable>())
		{
			interactable.OnHydrogenInteractableDestroyed += OnInteractableDestroyed;
		}

		SpawnNewHydrogenInteractable(HydrogenInteractableType.InstructionsClipboard);
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

	public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
				interactable.OnHydrogenInteractableDestroyed += OnInteractableDestroyed;
				break;
			case HydrogenInteractableType.SmallFilter:
				interactable = Instantiate(smallFilterPrefab, spawnPos, Quaternion.identity).GetComponent<HydrogenInteractable>();
				interactable.OnHydrogenInteractableDestroyed += OnInteractableDestroyed;
				break;
			case HydrogenInteractableType.GunModule:
				interactable = Instantiate(gunModulePrefab, spawnPos, Quaternion.identity).GetComponent<HydrogenInteractable>();
				interactable.OnHydrogenInteractableDestroyed += OnInteractableDestroyed;
				break;
			case HydrogenInteractableType.InstructionsClipboard:
				interactable = Instantiate(instructionsClipboard, spawnPos, Quaternion.identity).GetComponent<HydrogenInteractable>();
				interactable.OnHydrogenInteractableDestroyed += OnInteractableDestroyed;
				break;
			case HydrogenInteractableType.PlayAgainClipboard:
				interactable = Instantiate(playAgainClipboard, spawnPos, Quaternion.identity).GetComponent<HydrogenInteractable>();
				interactable.OnHydrogenInteractableDestroyed += OnInteractableDestroyed;
				interactable.GetComponentInChildren<ClipboardButton>().onClicked.AddListener(Restart);
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

	private void OnDisable()
	{
		// Unsubscribing GameManager from destroy events
		foreach (HydrogenInteractable interactable in FindObjectsOfType<HydrogenInteractable>())
		{
			interactable.OnHydrogenInteractableDestroyed -= OnInteractableDestroyed;
		}
	}
}
