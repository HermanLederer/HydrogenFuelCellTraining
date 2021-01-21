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
		[SerializeField] private bool startActive = true;
		public float pressDelay = 0.1f;
		[Header("Light")]
		public float lightResting = 1f;
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

		private float activeness = 1f;
		private float yMin = 0f;
		private float yMax = 0f;
		private bool previousPressState = false;
		private float previousPressTime = 0f;

		//
		// Private variables
		private float previousHandY;
		private XRBaseInteractor hoverInteractor = null;

		private void OnTriggerEnter(Collider other)
		{
			XRBaseInteractor interactor;
			if (other.gameObject.TryGetComponent<XRBaseInteractor>(out interactor))
			{
				hoverInteractor = interactor;
				previousHandY = GetLocalYPosition(interactor.transform.position);
			}
		}

		private void OnTriggerExit()
		{
			SetYPosition(yMax);
			CheckPress();
			hoverInteractor = null;
			previousPressState = false;
		}

		private void Start()
		{
			SetMinMax();
			if (startActive) Activate(0f, 0f);

			renderer.material.SetFloat("_Light", GetRestingLight());
		}

		private void SetMinMax()
		{
			Collider collider = GetComponent<Collider>();

			yMin = transform.localPosition.y - (collider.bounds.size.y);
			yMax = transform.localPosition.y;
		}

		public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			if (hoverInteractor)
			{
				float newHandY = GetLocalYPosition(hoverInteractor.transform.position);
				float handDelta = previousHandY - newHandY;
				previousHandY = newHandY;

				float newPosition = transform.localPosition.y - handDelta;
				SetYPosition(newPosition);

				CheckPress();
			}
		}

		private float GetLocalYPosition(Vector3 pos)
		{
			Vector3 localPos = transform.root.InverseTransformPoint(pos);
			return localPos.y;
		}

		private void SetYPosition(float y)
		{
			Vector3 newPos = transform.localPosition;
			newPos.y = Mathf.Clamp(y, yMin, yMax);
			transform.localPosition = newPos;
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
			return transform.localPosition.y < Mathf.Lerp(yMin, yMax, 0.5f);
		}

		public void Press()
		{
			if (IsActive() && Time.time > previousPressTime + pressDelay)
			{
				OnPress.Invoke();
				renderer.material.SetFloat("_Light", lightPressed);
				previousPressTime = Time.time;
			}
			
			HL.AudioManagement.AudioManager.Instance.PlayIn3D(press, volume, transform.position, minRadius, maxRadius);
			previousPressState = true;
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

				activeness = 1f;
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

				activeness = 0f;
			}

			renderer.material.SetFloat("_Light", GetRestingLight());
		}
	}
}
