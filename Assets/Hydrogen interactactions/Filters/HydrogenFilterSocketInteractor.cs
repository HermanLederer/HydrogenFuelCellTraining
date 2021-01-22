using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HydrogenInteractables
{
	public enum HydrogenFilterSocketProblems
	{
		NoProblem,
		FilterMissing,
		FilterInBadCondition
	}

	public class HydrogenFilterSocketInteractor : HydrogenSocketIntractor
	{
		[Header("Filter socket")]
		public HydrogenInteractableType filterType;
		public ParticleSystem troubleParticles;
		public AudioSource troubleAudio;

		new protected void OnTriggerEnter(Collider col)
		{
			HydrogenFilter filter = col.gameObject.GetComponentInParent<HydrogenFilter>();
			if (filter)
				if (filter.type == filterType)
					base.OnTriggerEnter(col);
		}

		new protected void OnTriggerExit(Collider col)
		{
			HydrogenFilter filter = col.gameObject.GetComponentInParent<HydrogenFilter>();
			if (filter)
				if (filter.type == filterType)
					base.OnTriggerExit(col);
		}

		public HydrogenFilterSocketProblems PowerOn()
		{
			var problem = HydrogenFilterSocketProblems.FilterMissing;

			if (selectTarget)
			{
				HydrogenFilter filter = selectTarget.GetComponent<HydrogenFilter>();
				if (filter.isInGoodCondition)
					problem = HydrogenFilterSocketProblems.NoProblem;
				else
					problem = HydrogenFilterSocketProblems.FilterInBadCondition;
			}

			// Problem feedback
			if (problem != HydrogenFilterSocketProblems.NoProblem)
			{
				troubleParticles.Play(true);
				troubleAudio.volume = 1f;
				troubleAudio.Play();
			}

			return problem;
		}

		public void PowerOff()
		{
			troubleParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
			StartCoroutine(StopTroubleAudioCorutine(0.5f));
		}

		IEnumerator StopTroubleAudioCorutine(float duration)
		{
			for (float t = 0f; t < duration; t += Time.deltaTime)
			{
				troubleAudio.volume = Mathf.Lerp(1f, 0f, t / duration);
				yield return new WaitForEndOfFrame();
			}

			troubleAudio.volume = 0f;
			troubleAudio.Stop();
		}
	}
}