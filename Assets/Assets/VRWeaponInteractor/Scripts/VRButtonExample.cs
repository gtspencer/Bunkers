using UnityEngine;
using System.Collections;

public class VRButtonExample : VRInteractableItem 
{
	//Target object needs to have a script with a public function call Activate()
	//When the button is pressed the activate method will be called on the target object
	public GameObject targetObject;

	//Use local or world coordinates
	public bool useLocal;
	public Vector3 defaultPosition;
	public Vector3 pressedPosition;

	void Start()
	{
		DisableHover();
	}

	override protected void Step()
	{
	}

	override public bool Pickup(VRInteractor hand)
	{
		return false;
	}

	override public void Drop(SteamVR_TrackedObject trackedObj)
	{
	}

	override public void Drop()
	{
	}

	override public void EnableHover()
	{
		//Pressed
		if (useLocal)
			transform.localPosition = pressedPosition;
		else
			transform.position = pressedPosition;
		if (targetObject == null)
		{
			Debug.LogError("No Target Object Specified");
			return;
		}
		targetObject.SendMessage("Activate", SendMessageOptions.DontRequireReceiver);
	}

	override public void DisableHover()
	{
		//UnPressed
		if (useLocal)
			transform.localPosition = defaultPosition;
		else
			transform.position = defaultPosition;
	}
}
