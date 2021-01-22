using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PanelWithScrews : XRGrabInteractable
{
	[Header("XRGrabInteractable")]
	public Screw screw1;
	public Screw screw2;
	public Screw screw3;
	public Screw screw4;

	public bool unlocked = false;

	

	public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
	{
		base.ProcessInteractable(updatePhase);

		if (unlocked)
		{
			screw1.screwedOut = 1f;
			screw2.screwedOut = 1f;
			screw3.screwedOut = 1f;
			screw4.screwedOut = 1f;
		}

		if (screw1.screwedOut >= 0.9f && screw2.screwedOut >= 0.9f && screw3.screwedOut >= 0.9f && screw4.screwedOut >= 0.9f)
			colliders[0].enabled = true;
		else
			colliders[0].enabled = false;
	}
}
