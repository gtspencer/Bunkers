using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VRSecondHeld))]
public class VRSecondHeldEditor : VRInteractableItemEditor 
{
	// target component
	public VRSecondHeld secondHeld = null;

	override public void OnEnable()
	{
		secondHeld = (VRSecondHeld)target;
		base.OnEnable();
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}
}
