using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRRayInteractorLineControl : MonoBehaviour
{
	public XRInteractorLineVisual lineRenderer;

	private void Awake()
	{
		lineRenderer = GetComponent<XRInteractorLineVisual>();
	}

	public void ShowLine()
	{
		lineRenderer.enabled = true;
	}

	public void HideLine()
	{
		lineRenderer.enabled = false;
	}
}
