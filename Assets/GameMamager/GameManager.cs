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
	[Header("Prefabs")]
	public GameObject largeFilterPrefab;
	public GameObject smallFilterPrefab;
	public GameObject gunModulePrefab;
	public GameObject screwdriverPrefab;
	[Header("Sounds")]
	public AudioClip winSound;
	public AudioClip loseSound;

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
			HL.AudioManagement.AudioManager.Instance.PlayIn2D(winSound, 0.5f);
		}
		else
		{
			//HL.AudioManagement.AudioManager.Instance.PlayIn2D(loseSound, 0.5f);
		}
	}

	public void SpawnNewHydrogenInteractable(HydrogenInteractableType type) => StartCoroutine(WaitForFrameAndInstanciateHydrogenInteractable(type));

	IEnumerator WaitForFrameAndInstanciateHydrogenInteractable(HydrogenInteractableType type)
	{
		yield return new WaitForEndOfFrame();

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
				Instantiate(gunModulePrefab, spawnPos, Quaternion.identity).GetComponent<HydrogenInteractable>().OnHydrogenInteractableDestroyed += OnInteractableDestroyed;
				break;
			case HydrogenInteractableType.Screwdriver:
				Instantiate(screwdriverPrefab, spawnPos, Quaternion.identity).GetComponent<HydrogenInteractable>().OnHydrogenInteractableDestroyed += OnInteractableDestroyed;
				break;
		}
	}

	public void OnInteractableDestroyed(HydrogenInteractable interactable)
	{
		HydrogenFilter filter;
		switch (interactable.type)
		{
			case HydrogenInteractableType.LargeFilter:
				filter = (HydrogenFilter) interactable;
				if (filter.isInGoodCondition) Debug.Log("Baaad! You baaad");
				SpawnNewHydrogenInteractable(interactable.type);
				break;
			case HydrogenInteractableType.SmallFilter:
				filter = (HydrogenFilter)interactable;
				if (filter.isInGoodCondition) Debug.Log("Baaad! You baaad");
				SpawnNewHydrogenInteractable(interactable.type);
				break;
		}
	}
}
