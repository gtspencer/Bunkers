using UnityEngine;
using System.Collections;

public class VRGunTrigger : MonoBehaviour {

	public VRGunHandler gunHandler;
	public Vector3 defaultTriggerPosition = Vector3.zero;
	public Quaternion defaultTriggerRotation = Quaternion.identity;
	public Vector3 pulledTriggerPosition = Vector3.zero;
	public Quaternion pulledTriggerRotation = Quaternion.identity;

	void Start () 
	{
		transform.localPosition = defaultTriggerPosition;
		transform.localRotation = defaultTriggerRotation;
	}

	void Update()
	{
		if (gunHandler == null || gunHandler.heldBy == null) return;

		float triggerPressure = gunHandler.heldBy.TriggerPressure;
		transform.localPosition = Vector3.Lerp(defaultTriggerPosition, pulledTriggerPosition, triggerPressure);
		transform.localRotation = Quaternion.Lerp(defaultTriggerRotation, pulledTriggerRotation, triggerPressure);
	}
}
