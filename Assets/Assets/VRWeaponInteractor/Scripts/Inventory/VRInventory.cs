using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VRInventory : MonoBehaviour {

	public Transform cameraTransform;
	public List<VRInventorySlot> slots = new List<VRInventorySlot>();

	void Update()
	{
		Vector3 newForward = cameraTransform.forward;
		newForward.y = 0;
		transform.position = cameraTransform.position;
		transform.forward = newForward;
	}
}
