using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SphereCollider))]
public class PhysicalizedAudioSource : MonoBehaviour
{
	//
	// Other components
	#region Other components
	private AudioSource source;
	#endregion

	//
	// Editor variables
	#region Editor variables
	[Header("Audio clips")]
	[SerializeField] private AudioClip[] clips = null;
	[SerializeField] private Vector2 randomPitch = Vector2.one;
	[SerializeField] private float cooldown = 0f;
	#endregion

	//
	// Private variables
	#region Private variables
	[Header("Audio clips")]
	private float nextSound = 0f;
	#endregion

	//--------------------------
	// MonoBehaviourPunCallbacks events
	//--------------------------
	void Awake()
	{
		source = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (Time.time >= nextSound)
		{
			source.pitch = Random.Range(randomPitch.x, randomPitch.y);
			source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
			nextSound = Time.time + cooldown;
		}
	}
}
