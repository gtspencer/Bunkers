using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VRInteractableItem))]
public class VRInteractableItemEditor : Editor 
{
	public VRInteractableItem interactableItem = null;
	SerializedObject serializedItem;

	virtual public void OnEnable()
	{
		Init();
	}

	void Init()
	{
		interactableItem = (VRInteractableItem)target;
		serializedItem = new SerializedObject(interactableItem);
	}

	public override void OnInspectorGUI()
	{
		if (serializedItem == null) Init();
		serializedItem.Update();
		SerializedProperty item = serializedItem.FindProperty("item");
		item.objectReferenceValue = EditorGUILayout.ObjectField("Item", item.objectReferenceValue, typeof(Transform), true);

		SerializedProperty heldMovementSpeed = serializedItem.FindProperty("heldMovementSpeed");
		heldMovementSpeed.floatValue = EditorGUILayout.FloatField("Held Movement Speed", heldMovementSpeed.floatValue);

		SerializedProperty heldRotationSpeed = serializedItem.FindProperty("heldRotationSpeed");
		heldRotationSpeed.floatValue = EditorGUILayout.FloatField("Held Rotation Speed", heldRotationSpeed.floatValue);

		SerializedProperty breakLimit = serializedItem.FindProperty("breakLimit");
		breakLimit.floatValue = EditorGUILayout.FloatField("Break Limit", breakLimit.floatValue);

		EditorGUILayout.HelpBox("Break limit is how far the object has to be from the controller to automatically drop the item", MessageType.Info);

		SerializedProperty toggleToPickup = serializedItem.FindProperty("toggleToPickup");
		toggleToPickup.boolValue = EditorGUILayout.Toggle("Toggle To Pickup", toggleToPickup.boolValue);

		SerializedProperty hover = serializedItem.FindProperty("hover");
		hover.objectReferenceValue = EditorGUILayout.ObjectField("Hover", hover.objectReferenceValue, typeof(Renderer), true);

		SerializedProperty hoverMode = serializedItem.FindProperty("hoverMode");
		hoverMode.intValue = (int)(VRInteractableItem.HoverMode)EditorGUILayout.EnumPopup("Hover Mode", (VRInteractableItem.HoverMode)hoverMode.intValue);
		VRInteractableItem.HoverMode hoverModeEnum = (VRInteractableItem.HoverMode)hoverMode.intValue;
		switch(hoverModeEnum)
		{
		case VRInteractableItem.HoverMode.SHADER:
			EditorGUILayout.HelpBox("Leave null to use current materials shader", MessageType.Info);
			SerializedProperty defaultShader = serializedItem.FindProperty("defaultShader");
			defaultShader.objectReferenceValue = EditorGUILayout.ObjectField("Default Shader", defaultShader.objectReferenceValue, typeof(Shader), false);
			EditorGUILayout.HelpBox("Hover Default is Unlit/Texture", MessageType.Info);
			SerializedProperty hoverShader = serializedItem.FindProperty("hoverShader");
			hoverShader.objectReferenceValue = EditorGUILayout.ObjectField("Hover Shader", hoverShader.objectReferenceValue, typeof(Shader), false);
			break;
		case VRInteractableItem.HoverMode.MATERIAL:
			EditorGUILayout.HelpBox("Leave null to use current material", MessageType.Info);
			SerializedProperty defaultMat = serializedItem.FindProperty("defaultMat");
			defaultMat.objectReferenceValue = EditorGUILayout.ObjectField("Default Material", defaultMat.objectReferenceValue, typeof(Material), false);
			EditorGUILayout.HelpBox("Hover Default is Unlit/Texture", MessageType.Info);
			SerializedProperty hoverMat = serializedItem.FindProperty("hoverMat");
			hoverMat.objectReferenceValue = EditorGUILayout.ObjectField("Hover Material", hoverMat.objectReferenceValue, typeof(Material), false);
			break;
		}

		SerializedProperty parentItem = serializedItem.FindProperty("parentItem");
		parentItem.objectReferenceValue = EditorGUILayout.ObjectField("Parent Item", parentItem.objectReferenceValue, typeof(VRInteractableItem), true);

		SerializedProperty enterHover = serializedItem.FindProperty("enterHover");
		enterHover.objectReferenceValue = EditorGUILayout.ObjectField("Enter Hover Sound", enterHover.objectReferenceValue, typeof(AudioClip), false);
		SerializedProperty exitHover = serializedItem.FindProperty("exitHover");
		exitHover.objectReferenceValue = EditorGUILayout.ObjectField("Exit Hover Sound", exitHover.objectReferenceValue, typeof(AudioClip), false);

		serializedItem.ApplyModifiedProperties();
		if (item.objectReferenceValue != null)
		{
			if (GUILayout.Button("Setup Held Position"))
			{
				HeldPositionWindow newWindow = (HeldPositionWindow)EditorWindow.GetWindow(typeof(HeldPositionWindow), true, "Held Position", true);
				newWindow.interactableItem = interactableItem;
				newWindow.Init();
			}
		}
	}
}