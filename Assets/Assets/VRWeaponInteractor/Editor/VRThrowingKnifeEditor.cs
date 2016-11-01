using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VRThrowingKnife))]
public class VRThrowingKnifeEditor : VRInteractableItemEditor 
{
	// target component
	public VRThrowingKnife throwingKnife = null;
	SerializedObject serializedKnife;

	override public void OnEnable()
	{
		throwingKnife = (VRThrowingKnife)target;
		serializedKnife = new SerializedObject(throwingKnife);
		base.OnEnable();
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		GUILayout.Label("Knife Settings");

		serializedKnife.Update();

		SerializedProperty guided = serializedKnife.FindProperty("guided");
		guided.boolValue = EditorGUILayout.Toggle("Guided", guided.boolValue);

		SerializedProperty knifeLayer = serializedKnife.FindProperty("knifeLayer");
		knifeLayer.stringValue = EditorGUILayout.TextField("Knife Layer", knifeLayer.stringValue);

		serializedKnife.ApplyModifiedProperties();
	}
}
