using UnityEngine;
using System.Collections;

public class VRSecondHeld : VRInteractableItem
{
	override protected void Step()
	{
	}

	override public bool Pickup(VRInteractor hand)
	{
		if (parentItem != null && parentItem.GetType() == typeof(VRGunHandler))
		{
			VRGunHandler gunHandler = (VRGunHandler)parentItem;
			gunHandler.usingSecondHeld = true;
			heldBy = hand;
			return true;
		}
		return false;
	}

	override public void Drop(SteamVR_TrackedObject trackedObj)
	{
		Drop();
	}

	override public void Drop()
	{
		if (parentItem != null && parentItem.GetType() == typeof(VRGunHandler))
		{
			VRGunHandler gunHandler = (VRGunHandler)parentItem;
			gunHandler.usingSecondHeld = false;
		}
		heldBy = null;
	}
}
