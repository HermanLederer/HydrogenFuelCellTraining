using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ModuleSocket : XRBaseInteractor
{
	// Editor fields
	public Transform firePoint;

	XRBaseInteractable m_ValidTarget;

	public Module selectedModule { get; private set; }

	private bool loaded = false;

	/// <inheritdoc />
	protected override void Awake()
	{
		base.Awake();
	}

	protected void OnTriggerEnter(Collider col)
	{
		if (loaded) return;

		var interactable = interactionManager.TryGetInteractableForCollider(col);
		if (interactable && selectTarget != interactable)
		{
			Module em;
			if (col.gameObject.TryGetComponent<Module>(out em))
			{
				this.selectedModule = em;
				if (em.power > 0f)
				{
					m_ValidTarget = interactable;
					em.AttachToGun(firePoint);
					loaded = true;
				}
			}
		}
	}

	/// <summary>
	/// Retrieve the list of interactables that this interactor could possibly interact with this frame.
	/// This list is sorted by priority (in this case distance).
	/// </summary>
	/// <param name="validTargets">Populated List of interactables that are valid for selection or hover.</param>
	public override void GetValidTargets(List<XRBaseInteractable> validTargets)
	{
		validTargets.Clear();
		if (m_ValidTarget)
		{
			if (selectedModule.power > 0f)
			{
				validTargets.Add(m_ValidTarget);
			}
			else
			{
				if (selectTarget) interactionManager.SelectExit(this, selectTarget);
				selectedModule.Detach();
				loaded = false;
				selectedModule = null;
				m_ValidTarget = null;
			}
		}
	}
}
