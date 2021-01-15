using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HL.AudioManagement
{
	public class AudioManager : MonoBehaviour
	{
		// Singleton
		private static AudioManager m_Instance;
		public static AudioManager Instance
		{
			get
			{
				return m_Instance;
			}
		}

		//
		// Editor variables
		#region Editor variables
		//[Header("Audio sources")]
		//[SerializeField] private AudioSource musicSource = null;
		//[Header("Music")]
		//[SerializeField] private AudioClip[] music = null;
		[Header("Pool prefabs")]
		[SerializeField] private GameObject soundObject2dPrefab = null;
		[SerializeField] private GameObject soundObject3dPrefab = null;
		#endregion

		private bool firstMusicSourceIsPlaying;
		private const string pool2d = "AudioPool2d";
		private const string pool3d = "AudioPool3d";

		//--------------------------
		// MonoBehaviour methods
		//--------------------------
		private void Awake()
		{
			// singleton
			if (m_Instance != null && m_Instance != this)
			{
				Debug.LogWarning("Impossible to initiate more than one AudioManager. Destryoing the instance...");
					Destroy(this.gameObject);
					return;
			}
			else
			{
				m_Instance = this;
			}

			DontDestroyOnLoad(this.gameObject);
		}

		private void Start()
		{
			ObjectPooler.Instance.CreateNewPool(pool2d, soundObject2dPrefab, 5);
			ObjectPooler.Instance.CreateNewPool(pool3d, soundObject3dPrefab, 60);
		}

		private void Update()
		{
			//check if music is playing and if it's not randomly start one of the tracks
			//if (!musicSource.isPlaying)
			//{
			//	PlayMusic(music[Random.Range(0, music.Length)]);
			//}
		}

		//--------------------------
		// AudioManager methods
		//--------------------------
		// Audio sources
		//public void PlayMusic(AudioClip musicClip)
		//{
		//	musicSource.clip = musicClip;
		//	musicSource.Play();
		//}

		//public void StopMusic()
		//{
		//	musicSource.Stop();
		//}

		public void PlayIn2D(AudioClip clip, float volume)
		{
			GameObject obj = ObjectPooler.Instance.SpawnFromPool(pool2d, Vector3.zero, Quaternion.identity);
			AudioSource source = obj.GetComponent<AudioSource>();
			source.volume = volume;
			source.clip = clip;
			source.Play();
		}

		public void PlayIn3D(AudioClip clip, float volume, Vector3 position, float minRadius, float maxRadius)
		{
			GameObject obj = ObjectPooler.Instance.SpawnFromPool(pool3d, position, Quaternion.identity);
			AudioSource source = obj.GetComponent<AudioSource>();
			source.volume = volume;
			source.minDistance = minRadius;
			source.maxDistance = maxRadius;
			source.clip = clip;
			source.Play();
		}
	}
}