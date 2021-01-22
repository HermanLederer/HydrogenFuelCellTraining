using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace HydrogenInteractables
{
	public class ButtonInteractable : XRBaseInteractable
	{
		//
		// Editor fields
		[Header("Button")]
		public float offset = 0f;
		public float tMax = 0f;
		[Range(0, 1)] public float tActive = 0.5f;
		[SerializeField] private bool startActive = true;
		public float activationDelay = 0.5f;
		[Header("Light")]
		[SerializeField] private float lightResting = 1f;
		public float LightResting
		{
			get { return lightResting; }
			set
			{
				lightResting = value;
				renderer.material.SetFloat("_Light", GetRestingLight());
			}
		}
		public float lightPressed = 5f;
		[Header("Child components")]
		new public MeshRenderer renderer = null;
		[Header("Sounds")]
		public float volume;
		public float minRadius;
		public float maxRadius;
		public AudioClip press;
		public AudioClip release;
		[Header("Events")]
		public UnityEvent OnPress = null;

		//
		// Private variables
		private XRBaseInteractor hoverInteractor = null;

		private Vector3 rootPosition;
		private float previousZ;
		private float pressZ = 0f;

		private bool previousPressState = false;
		private float previousPressTime = 0f;

		private float activeness = 1f;

		private void OnTriggerEnter(Collider other)
		{
			XRBaseInteractor interactor;
			if (other.gameObject.TryGetComponent<XRBaseInteractor>(out interactor))
			{
				hoverInteractor = interactor;
				pressZ = 0f;
				previousZ = GetLocalPosition(hoverInteractor.transform.position).z;
			}
		}

		private void OnTriggerExit()
		{
			pressZ = 0f;
			CheckPress();

			hoverInteractor = null;
			previousPressState = false;
		}

		private void Start()
		{
			rootPosition = transform.position + transform.forward * offset;
			if (startActive) Activate(0f, 0f);

			renderer.material.SetFloat("_Light", GetRestingLight());
		}

		public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			if (hoverInteractor)
			{
				var localZ = GetLocalPosition(hoverInteractor.transform.position).z;
				var deltaZ = localZ - previousZ;
				pressZ = Mathf.Clamp(pressZ + deltaZ, -tMax, 0f);
				previousZ = localZ;
				CheckPress();
			}

			transform.position = rootPosition - transform.forward * offset + transform.forward * pressZ;
		}

		private Vector3 GetLocalPosition(Vector3 position)
		{
			return transform.parent.InverseTransformPoint(position);
		}

		private void CheckPress()
		{
			bool isInPosition = IsInPosition();

			if (isInPosition != previousPressState)
			{
				if (isInPosition) Press();
				else Release();
			}
		}

		private bool IsInPosition()
		{
			return pressZ < -tMax * tActive;
		}

		public void Press()
		{
			if (Time.time > previousPressTime + 0.1f)
			{
				if (IsActive() && Time.time > previousPressTime + activationDelay)
				{
					OnPress.Invoke();
					renderer.material.SetFloat("_Light", lightPressed);
					previousPressTime = Time.time;
				}

				HL.AudioManagement.AudioManager.Instance.PlayIn3D(press, volume, transform.position, minRadius, maxRadius);
				previousPressState = true;
			}
		}

		public void Release()
		{
			HL.AudioManagement.AudioManager.Instance.PlayIn3D(release, volume, transform.position, minRadius, maxRadius);
			renderer.material.SetFloat("_Light", GetRestingLight());
			previousPressState = false;
		}

		private float GetRestingLight()
		{
			return lightResting * activeness;
		}

		public bool IsActive() { return activeness > 0f; }

		public void Activate(float delay, float duration) => StartCoroutine(ActivationCorutine(true, delay, duration));

		public void Deactivate(float delay, float duration) => StartCoroutine(ActivationCorutine(false, delay, duration));

		IEnumerator ActivationCorutine(bool activate, float lightDelay, float duration)
		{
			if (activate)
			{
				activeness = 1f;

				yield return new WaitForSeconds(lightDelay);
				float lightActiveness;
				for (float t = 0f; t < duration; t += Time.deltaTime)
				{
					lightActiveness = Mathf.Lerp(0f, 1f, t / duration);
					renderer.material.SetFloat("_Light", lightResting * lightActiveness);
					yield return new WaitForEndOfFrame();
				}
			}
			else // fade out
			{
				activeness = 0f;

				yield return new WaitForSeconds(lightDelay);
				float lightActiveness;
				for (float t = 0f; t < duration; t += Time.deltaTime)
				{
					lightActiveness = Mathf.Lerp(1f, 0f, t / duration);
					renderer.material.SetFloat("_Light", lightResting * lightActiveness);
					yield return new WaitForEndOfFrame();
				}
			}

			renderer.material.SetFloat("_Light", GetRestingLight());
		}
	}
}
