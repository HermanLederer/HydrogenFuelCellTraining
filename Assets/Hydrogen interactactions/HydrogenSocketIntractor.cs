using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HydrogenInteractables
{
	public class HydrogenSocketIntractor : XRSocketInteractor
	{
		[Header("HydrogenSocketIntractor")]
		public GameObject hydrogenInteractorHighlighPrefab;

		private ParticleSystem m_HoverHighlight;

		protected override void Awake()
		{
			base.Awake();

			m_HoverHighlight = Instantiate(hydrogenInteractorHighlighPrefab, attachTransform).GetComponent<ParticleSystem>();

			onHoverEntered.AddListener(StartHoverHighlight);
			onHoverExited.AddListener(StopHoverHighlight);
		}

		private void StartHoverHighlight(XRBaseInteractable interactable)
		{
			m_HoverHighlight.Play(true);
		}

		private void StopHoverHighlight(XRBaseInteractable interactable)
		{
			m_HoverHighlight.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		}
	}
}
