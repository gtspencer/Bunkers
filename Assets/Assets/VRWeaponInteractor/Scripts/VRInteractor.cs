using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Valve.VR;

[RequireComponent (typeof (SteamVR_TrackedController))]
[RequireComponent (typeof (SteamVR_TrackedObject))]
[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (SphereCollider))]
public class VRInteractor : MonoBehaviour 
{
	public string[] VRActions;

	public int triggerKey = 1;
	public int padTop = 2;
	public int padLeft = 2;
	public int padRight = 2;
	public int padBottom = 3;
	public int padCentre = 2;
	public int gripKey = 0;
	public int menuKey = 0;
	public bool actionKeyCanPickup = true;
	public bool ejectKeyCanPickup = true;
	public float interactorSphereSize = 0.12f;
	public GameObject weaponReference;
	public bool debugMode;

	List<VRInteractableItem> currentInteractables = new List<VRInteractableItem>();
	SteamVR_RenderModel controllerRenderModel;
	SteamVR_TrackedObject trackedObject;
	SteamVR_TrackedController controller;

	VRInteractableItem _hoverItem;
	public VRInteractableItem hoverItem
	{
		get { return _hoverItem; }
		set { _hoverItem = value; }
	}
	VRInteractableItem _heldItem;
	public VRInteractableItem heldItem
	{
		get { return _heldItem; }
	}
	private bool controllerNeverRendered;

	public bool ActionPressed(string action)
	{
		for(int i=0; i<VRActions.Length; i++)
		{
			if (action == VRActions[i])
			{
				return ActionPressed(i);
			}
		}
		if (debugMode)
		{
			string debugString = "No action with the name " + action + ". Current options are ";
			foreach(string availableOption in VRActions)
			{
				debugString += availableOption + " ";
			}
			Debug.LogWarning(debugString);
		}
		return false;
	}

	public bool ActionPressed(int action)
	{
		if (triggerKey == action && controller.triggerPressed)
			return true;
		if (padTop == action && PadUpPressed)
			return true;
		if (padLeft == action && PadLeftPressed)
			return true;
		if (padRight == action && PadRightPressed)
			return true;
		if (padBottom == action && PadDownPressed)
			return true;
		if (padCentre == action && PadCentrePressed)
			return true;
		if (menuKey == action && controller.menuPressed)
			return true;
		if (gripKey == action && controller.gripped)
			return true;
		return false;
	}

	public float TriggerPressure
	{
		get
		{
			var device = SteamVR_Controller.Input((int)controller.controllerIndex);
			return device.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger).x;
		}
	}
	public bool PadUpPressed
	{
		get
		{
			if (controller.padPressed)
			{
				var device = SteamVR_Controller.Input((int)controller.controllerIndex);
				Vector2 axis = device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);
				if (axis.y > 0.4f &&
					axis.x < axis.y &&
					axis.x > -axis.y)
					return true;
			}
			return false;
		}
	}
	public bool PadLeftPressed
	{
		get
		{
			if (controller.padPressed)
			{
				var device = SteamVR_Controller.Input((int)controller.controllerIndex);
				Vector2 axis = device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);
				if (axis.x < -0.4f &&
					axis.y > axis.x &&
					axis.y < -axis.x)
					return true;
			}
			return false;
		}
	}
	public bool PadRightPressed
	{
		get
		{
			if (controller.padPressed)
			{
				var device = SteamVR_Controller.Input((int)controller.controllerIndex);
				Vector2 axis = device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);
				if (axis.x > 0.4f &&
					axis.y < axis.x &&
					axis.y > -axis.x)
					return true;
			}
			return false;
		}
	}
	public bool PadDownPressed
	{
		get
		{
			if (controller.padPressed)
			{
				var device = SteamVR_Controller.Input((int)controller.controllerIndex);
				Vector2 axis = device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);
				if ((axis.y < -0.4f &&
					axis.x > axis.y &&
					axis.x < -axis.y) ||
					axis == Vector2.zero)
					return true;
			}
			return false;
		}
	}
	public bool PadCentrePressed
	{
		get
		{
			if (controller.padPressed)
			{
				var device = SteamVR_Controller.Input((int)controller.controllerIndex);
				Vector2 axis = device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);

				if (axis.y >= -0.4f && axis.y <= 0.4f && axis.x >= -0.4f && axis.x <= 0.4f)
					return true;
			}
			return false;
		}
	}
	public Vector3 Velocity
	{
		get
		{
			var device = SteamVR_Controller.Input((int)controller.controllerIndex);
			return device.velocity;
		}
	}

	void Start()
	{
		Init();
	}

	public virtual void Init()
	{
		SphereCollider col = GetComponent<SphereCollider>();
		col.isTrigger = true;
		col.radius = interactorSphereSize;
		Rigidbody body = GetComponent<Rigidbody>();
		body.useGravity = false;
		body.isKinematic = true;
		body.constraints = RigidbodyConstraints.FreezeAll;
		controllerRenderModel = GetComponentInChildren<SteamVR_RenderModel>();
		trackedObject = GetComponent<SteamVR_TrackedObject>();
		if (weaponReference != null)
		{
			VRGunHandler gunHandler = weaponReference.GetComponentInChildren<VRGunHandler>();
			if (gunHandler != null)
			{
				hoverItem = gunHandler;
				weaponReference.transform.position = transform.position;
				TryPickup();
				controllerNeverRendered = true;
			} else Debug.LogWarning("Couldn't find gun handler in weapon reference");
		}
	}

	void OnEnable()
	{
		controller = GetComponent<SteamVR_TrackedController>();
		if (controller == null)
		{
			Debug.LogError("No controller SteamVR_TrackedController found");
			return;
		}
		controller.TriggerClicked += TriggerClicked;
		controller.TriggerUnclicked += TriggerReleased;
		controller.PadClicked += TrackpadDown;
		controller.PadUnclicked += TrackpadUp;
		controller.Gripped += Gripped;
		controller.Ungripped += UnGripped;
		controller.MenuButtonClicked += MenuClicked;
		controller.MenuButtonUnclicked += MenuReleased;
	}

	void OnDisable()
	{
		if (controller == null) return;
		controller.TriggerClicked -= TriggerClicked;
		controller.TriggerUnclicked -= TriggerReleased;
		controller.PadClicked -= TrackpadDown;
		controller.PadUnclicked -= TrackpadUp;
		controller.Gripped -= Gripped;
		controller.Ungripped -= UnGripped;
		controller.MenuButtonClicked -= MenuClicked;
		controller.MenuButtonUnclicked -= MenuReleased;
	}

	void Update()
	{
		if (!enabled) return;
		CheckHover();
		CheckDistance();
	}

	void CheckHover()
	{
		if (_heldItem != null) return; 

		if (currentInteractables.Count == 0)
		{
			if (hoverItem != null)
			{
				hoverItem.DisableHover();
				hoverItem = null;
			}
			return;
		}

		VRInteractableItem closestItem = null;
		if (currentInteractables.Count == 1)
		{
			if (currentInteractables[0] == null)
			{
				currentInteractables.Clear();
				return;
			}
			if (currentInteractables[0].heldBy == null) closestItem = currentInteractables[0];
		} else
		{
			float closestDistance = 10;
			bool containsNull = false;
			foreach(VRInteractableItem item in currentInteractables)
			{
				if (item == null) 
				{
					containsNull = true;
					continue;
				}
				if (item.heldBy != null) continue;
				float distance = Vector3.Distance(item.transform.position, transform.position);
				if (distance < closestDistance)
				{
					closestDistance = distance;
					closestItem = item;
				}
			}
			if (containsNull) currentInteractables.Remove(null);
		}
		if (closestItem == null)
		{ 
			currentInteractables.Clear();
			return;
		}
		if (hoverItem == null)
		{
			hoverItem = closestItem;
			closestItem.EnableHover();
		} else if (hoverItem != closestItem)
		{
			hoverItem.DisableHover();
			hoverItem = closestItem;
			closestItem.EnableHover();
		}
		currentInteractables.Clear();
	}

	void CheckDistance()
	{
		if (_heldItem == null) return;

		float distance = Vector3.Distance(_heldItem.transform.position, transform.position);
		if (distance > transform.lossyScale.x * _heldItem.breakLimit)
		{
			Drop();
		}
	}

	void OnTriggerStay(Collider col)
	{
		if (!enabled) return;
		VRInteractableItem ii = col.GetComponent<VRInteractableItem>();
		if (ii != null && ii.enabled && (ii.parentItem == null || ii.parentItem.heldBy != null))
		{
			if (!currentInteractables.Contains(ii))
			{
				currentInteractables.Add(ii);
			}
		}
	}

	void TriggerClicked(object sender, ClickedEventArgs e)
	{
		if (triggerKey >= VRActions.Length)
		{
			Debug.LogWarning("Trigger key index (" + triggerKey + ") out of range (" + VRActions.Length+")");
			return;
		}
		SendMessage(VRActions[triggerKey], SendMessageOptions.DontRequireReceiver);
	}

	void TriggerReleased(object sender, ClickedEventArgs e)
	{
		if (triggerKey >= VRActions.Length)
		{
			Debug.LogWarning("Trigger key index (" + triggerKey + ") out of range (" + VRActions.Length+")");
			return;
		}
		SendMessage(VRActions[triggerKey]+"Released", SendMessageOptions.DontRequireReceiver);
	}

	void TrackpadDown(object sender, ClickedEventArgs e)
	{
		int action = 0;
		if (PadUpPressed) action = padTop;
		if (PadLeftPressed) action= padLeft;
		if (PadRightPressed) action = padRight;
		if (PadDownPressed) action = padBottom;
		if (PadCentrePressed) action = padCentre;
		if (action >= VRActions.Length)
		{
			Debug.LogWarning("Pad key index (" + action + ") out of range (" + VRActions.Length+")");
			return;
		}
		SendMessage(VRActions[action], SendMessageOptions.DontRequireReceiver);
	}

	void TrackpadUp(object sender, ClickedEventArgs e)
	{
		for(int i=0; i<VRActions.Length; i++)
		{
			if (padLeft == i || padTop == i || padRight == i || padBottom == i || padCentre == i)
				SendMessage(VRActions[i]+"Released", SendMessageOptions.DontRequireReceiver);
		}
	}

	void Gripped(object sender, ClickedEventArgs e)
	{
		if (gripKey >= VRActions.Length)
		{
			Debug.LogWarning("Gripped key index (" + gripKey + ") out of range (" + VRActions.Length+")");
			return;
		}
		SendMessage(VRActions[gripKey], SendMessageOptions.DontRequireReceiver);
	}

	void UnGripped(object sender, ClickedEventArgs e)
	{
		if (gripKey >= VRActions.Length)
		{
			Debug.LogWarning("Gripped key index (" + gripKey + ") out of range (" + VRActions.Length+")");
			return;
		}
		SendMessage(VRActions[gripKey]+"Released", SendMessageOptions.DontRequireReceiver);
	}

	void MenuClicked(object sender, ClickedEventArgs e)
	{
		if (menuKey >= VRActions.Length)
		{
			Debug.LogWarning("Menu key index (" + menuKey + ") out of range (" + VRActions.Length+")");
			return;
		}
		SendMessage(VRActions[menuKey], SendMessageOptions.DontRequireReceiver);
	}

	void MenuReleased(object sender, ClickedEventArgs e)
	{
		if (menuKey >= VRActions.Length)
		{
			Debug.LogWarning("Menu key index (" + menuKey + ") out of range (" + VRActions.Length+")");
			return;
		}
		SendMessage(VRActions[menuKey]+"Released", SendMessageOptions.DontRequireReceiver);
	}

	public bool TryPickup()
	{
		if (_heldItem != null) return false;
		if (hoverItem != null)
		{
			if (hoverItem.heldBy != null) return false;
			_heldItem = hoverItem;
			_heldItem.DisableHover();
			bool success = _heldItem.Pickup(this);
			if (success)
			{
				hoverItem = null;
				controllerRenderModel.gameObject.SetActive(false);
				return true;
			} else
			{
				_heldItem = null;
			}
		}
		return false;
	}

	public void Drop()
	{
		if (_heldItem == null) return;
		_heldItem.Drop(trackedObject);
		_heldItem = null;
		controllerRenderModel.gameObject.SetActive(true);
		if (controllerNeverRendered)
		{
			SteamVR_Utils.Event.Send("ModelSkinSettingsHaveChanged");
			controllerNeverRendered = false;
		}
	}
}
