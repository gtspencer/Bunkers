using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VRInventory))]
public class VRInventoryEditor : Editor {

	public VRInventory inventory = null;
	SerializedObject serializedInventory;

	virtual public void OnEnable()
	{
		inventory = (VRInventory)target;
		serializedInventory = new SerializedObject(inventory);
	}

	public override void OnInspectorGUI()
	{
		serializedInventory.Update();

		SerializedProperty cameraTransform = serializedInventory.FindProperty("cameraTransform");
		cameraTransform.objectReferenceValue = EditorGUILayout.ObjectField("Camera Transform", cameraTransform.objectReferenceValue, typeof(Transform), true);

		SerializedProperty slots = serializedInventory.FindProperty("slots");
		EditorGUILayout.PropertyField(slots, true);

		if (GUILayout.Button("Add New Slot"))
		{
			GameObject newSlot = new GameObject("Slot");
			newSlot.transform.parent = inventory.transform;
			VRInventorySlot newSlotScript = newSlot.AddComponent<VRInventorySlot>();
			newSlotScript.inventory = inventory;
			slots.InsertArrayElementAtIndex(slots.arraySize);
			SerializedProperty newSlotProperty = slots.GetArrayElementAtIndex(slots.arraySize-1);
			newSlotProperty.objectReferenceValue = newSlotScript;
			Selection.activeGameObject = newSlot;
		}
		serializedInventory.ApplyModifiedProperties();
	}
}
