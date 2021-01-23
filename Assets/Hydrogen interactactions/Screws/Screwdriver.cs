using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace HydrogenInteractables
{
	[RequireComponent(typeof(AudioSource))]
	public class Screwdriver : XRGrabInteractable
	{
		//
		// Editor fields
		[Header("Screwdriver")]
		public bool screwingDirection = true;
		public float maximumSpeed = 2f;

		public InputAction trigger;
		public InputAction button;

		public AudioClip reverseButton;

		//
		// Other components
		private AudioSource audioSource;

		//
		// Variables
		private Screw selectedScrew;
		private bool buttonPressed;

		protected override void Awake()
		{
			base.Awake();

			audioSource = GetComponent<AudioSource>();
		}

		protected void OnEnable()
		{
			trigger.Enable();
			button.Enable();
		}

		private void OnTriggerEnter(Collider other)
		{
			Screw screw;
			if (other.TryGetComponent(out screw))
			{
				selectedScrew = screw;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (selectedScrew) if (other.gameObject == selectedScrew.gameObject) selectedScrew = null;
		}

		private void Update()
		{
			// Control
			var triggetValue = 0f;
			var buttonValue = false;
			if (selectingInteractor)
			{
				triggetValue = trigger.ReadValue<float>();
				if (triggetValue < 0.0001f) triggetValue = 0f;

				buttonValue = button.ReadValue<float>() > 0.5f;
				if (buttonValue)
					if (!buttonPressed)
						Reverse();

				buttonPressed = buttonValue;
			}

			// Screwing
			if (selectedScrew)
				selectedScrew.ScrewedOut += maximumSpeed * triggetValue * (screwingDirection ? 1f : -1f) * Time.deltaTime;

			// Audio
			audioSource.volume = triggetValue;
			audioSource.pitch = triggetValue;
		}

		public void Reverse()
		{
			screwingDirection = !screwingDirection;
			HL.AudioManagement.AudioManager.Instance.PlayIn3D(reverseButton, 1f, transform.position, 0.1f, 2f);
		}
	}
}
