using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace HL.MoreXRIntractions
{
	public class ButtonInteractable : XRBaseInteractable
	{
		//
		// Editor fields
		[ColorUsage(true, true)]
		public Color pressedColor;
		new public MeshRenderer renderer = null;
		[Header("Sounds")]
		public float volume;
		public float minRadius;
		public float maxRadius;
		public AudioClip press;
		public AudioClip release;
		[Header("Events")]
		public UnityEvent OnPress = null;
		[Header("Buttton movement")]
		private float yMin = 0f;
		private float yMax = 0f;
		private bool previousPressState = false;

		//
		// Private variables
		private float previousHandY;
		private XRBaseInteractor hoverInteractor = null;

		protected override void Awake()
		{
			base.Awake();

			//onHoverEntered.AddListener(StartPress);
			//onHoverExited.AddListener(EndPress);
		}

		protected override void OnDestroy()
		{
			//onHoverEntered.RemoveListener(StartPress);
			//onHoverExited.RemoveListener(EndPress);

			base.OnDestroy();
		}

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
			hoverInteractor = null;
			previousPressState = false;
			SetYPosition(yMax);
			Debug.Log(transform.localPosition.y);
			Debug.Log(Mathf.Lerp(yMin, yMax, 0.5f));
			CheckPress();
		}

		private void Start()
		{
			SetMinMax();
			renderer.material.SetColor("_EmissionColor", Color.black);
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
			OnPress.Invoke();

			renderer.material.SetColor("_EmissionColor", pressedColor);
			HL.AudioManagement.AudioManager.Instance.PlayIn3D(press, volume, transform.position, minRadius, maxRadius);
			previousPressState = true;
		}

		public void Release()
		{
			renderer.material.SetColor("_EmissionColor", Color.black);
			HL.AudioManagement.AudioManager.Instance.PlayIn3D(release, volume, transform.position, minRadius, maxRadius);
			previousPressState = false;
		}
	}
}
