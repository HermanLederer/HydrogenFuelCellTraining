using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
	}

	public void Confirm()
	{
		if (bus.Confirm())
		{
			if (!winMusic.isPlaying) winMusic.Play();
		}
		else
		{
			//HL.AudioManagement.AudioManager.Instance.PlayIn2D(loseSound, 0.5f);
		}
	}

	public void SpawnNewHydrogenInteractable(HydrogenInteractableType type) => StartCoroutine(SpawnNewHydrogenInteractableCorutine(type, 1f));

	IEnumerator SpawnNewHydrogenInteractableCorutine(HydrogenInteractableType type, float delay)
	{
		yield return new WaitForSeconds(1f);

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
