using UnityEngine;
using System.Collections;

public class VRDefaultControls : MonoBehaviour {

    VRInteractor interactor;
    SteamVR_TrackedController controller;

    void Start()
    {
        interactor = GetComponent<VRInteractor>();
        if (interactor == null) Debug.LogWarning("Default controls require an interactor to work");
        controller = GetComponent<SteamVR_TrackedController>();
        if (controller == null) Debug.LogError("No controller SteamVR_TrackedController found");
    }

    virtual public void ACTION()
    {
        if (interactor.heldItem != null)
            interactor.heldItem.ActionPressed();
        else if (interactor.actionKeyCanPickup)
            interactor.TryPickup();
    }

    virtual public void ACTIONReleased()
    {
        if (interactor.heldItem != null)
        {
            interactor.heldItem.ActionRelease();
            if (!interactor.heldItem.toggleToPickup)
                interactor.Drop();
        }
    }

    virtual public void PICKUP_DROP()
    {
        if (interactor.heldItem == null)
            interactor.TryPickup();
        else if (interactor.heldItem.toggleToPickup)
            interactor.Drop();
    }

    virtual public void PICKUP_DROPReleased()
    {
        if (interactor.heldItem != null && !interactor.heldItem.toggleToPickup)
            interactor.Drop();
    }

    virtual public void EJECT()
    {
        if (interactor.heldItem != null)
            interactor.heldItem.Action2Pressed();
        else if (interactor.ejectKeyCanPickup)
            interactor.TryPickup();
    }

    virtual public void EJECTReleased()
    {
        if (interactor.heldItem != null)
        {
            interactor.heldItem.Action2Release();
            if (!interactor.heldItem.toggleToPickup)
                interactor.Drop();
        }
    }

    virtual public void FLATTEN()
    {
        if (interactor.heldItem != null)
            interactor.heldItem.flattenPressed();
    }

    virtual public void FLATTENReleased()
    {
        if (interactor.heldItem != null)
        {
            interactor.heldItem.flattenReleased();
            if (!interactor.heldItem.toggleToPickup)
                interactor.Drop();
        }
    }

    virtual public void RAISE()
    {
        if (interactor.heldItem != null)
        {
            interactor.heldItem.addPressed();
            if (!interactor.heldItem.toggleToPickup)
                interactor.Drop();
        }
    }

    virtual public void RAISERelease()
    {
        if (interactor.heldItem != null)
        {
            interactor.heldItem.addReleased();
            if (!interactor.heldItem.toggleToPickup)
                interactor.Drop();
        }
    }

	void OnTriggerEnter(Collider col)
	{
		if (!enabled) return;
		VRInteractableItem ii = col.GetComponent<VRInteractableItem>();
		if (ii != null && ii.enabled && (ii.parentItem == null || ii.parentItem.heldBy != null))
		{
			if (controller == null) controller = GetComponent<SteamVR_TrackedController>();
			var device = SteamVR_Controller.Input((int)controller.controllerIndex);
			device.TriggerHapticPulse();
			if (interactor.ActionPressed("ACTION") || interactor.ActionPressed("PICKUP_DROP") || interactor.ActionPressed("EJECT"))
			{
				interactor.hoverItem = ii;
				interactor.TryPickup();
			}
		}
	}
}
