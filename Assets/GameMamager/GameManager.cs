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

	public Transform spawnTransform;

	private void Start()
	{
		foreach (HydrogenInteractable interactable in GetComponents<HydrogenInteractable>())
			interactable.OnHydrogenInteractableDestroyed += OnInteractableDestroyed;
	}

	public void Confirm()
	{
		if (bus.Confirm()) Win();
		HL.AudioManagement.AudioManager.Instance.PlayIn2D(winSound, 1f);
	}

	public void Win()
	{
		// Add success indication
	}

	public void SpawnNewHydrogenInteractable(HydrogenInteractableType type) => StartCoroutine(WaitForFrameAndInstanciateHydrogenInteractable(type));

	IEnumerator WaitForFrameAndInstanciateHydrogenInteractable(HydrogenInteractableType type)
	{
		yield return new WaitForEndOfFrame();
		switch (type)
		{
			case HydrogenInteractableType.LargeFilter:
				Instantiate(largeFilterPrefab, Vector3.zero, Quaternion.identity).GetComponent<HydrogenInteractable>().OnHydrogenInteractableDestroyed += OnInteractableDestroyed;
				break;
			case HydrogenInteractableType.SmallFilter:
				Instantiate(smallFilterPrefab, Vector3.zero, Quaternion.identity).GetComponent<HydrogenInteractable>().OnHydrogenInteractableDestroyed += OnInteractableDestroyed;
				break;
			case HydrogenInteractableType.GunModule:
				Instantiate(gunModulePrefab, Vector3.zero, Quaternion.identity).GetComponent<HydrogenInteractable>().OnHydrogenInteractableDestroyed += OnInteractableDestroyed;
				break;
			case HydrogenInteractableType.Screwdriver:
				Instantiate(screwdriverPrefab, Vector3.zero, Quaternion.identity).GetComponent<HydrogenInteractable>().OnHydrogenInteractableDestroyed += OnInteractableDestroyed;
				break;
		}
	}

	public void OnInteractableDestroyed(HydrogenInteractable interactable)
	{
		Debug.Log("interactable " + interactable.name + " destroyed");
	}
}
