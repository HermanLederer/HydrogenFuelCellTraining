using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace HL.MoreXRIntractions
{
	public class ButtonInteractable : XRBaseInteractable
	{
		[ColorUsage(true, true)]
		public Color pressedColor;
		new public MeshRenderer renderer = null;
		public AudioSource auidoSource = null;
		public AudioClip click;

		public UnityEvent OnPress = null;

		private float yMin = 0f;
		private float yMax = 0f;
		private bool previousPress = false;

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
			auidoSource.PlayOneShot(click);
			if (other.gameObject.TryGetComponent<XRBaseInteractor>(out interactor))
			{
				hoverInteractor = interactor;
				previousHandY = GetLocalYPosition(interactor.transform.position);
			}
		}

		private void OnTriggerExit()
		{
			hoverInteractor = null;
			previousPress = false;
			SetYPosition(yMax);
			renderer.material.SetColor("_EmissionColor", Color.black);
		}

		private void Start()
		{
			SetMinMax();
			renderer.material.SetColor("_EmissionColor", Color.black);
		}

		private void SetMinMax()
		{
			Collider collider = GetComponent<Collider>();

			yMin = transform.localPosition.y - (collider.bounds.size.y - 0.01f);
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

			if (isInPosition)
			{
				if (isInPosition != previousPress)
					OnPress.Invoke();

				renderer.material.SetColor("_EmissionColor", pressedColor);
			}
			else
			{
				renderer.material.SetColor("_EmissionColor", Color.black);
			}

			previousPress = isInPosition;
		}

		private bool IsInPosition()
		{
			float inRange = Mathf.Clamp(transform.localPosition.y, yMin, yMin + 0.01f);
			return (transform.localPosition.y) == inRange;
		}
	}
}
