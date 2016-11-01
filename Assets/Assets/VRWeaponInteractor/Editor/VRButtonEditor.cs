using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VRButtonExample))]
public class VRButtonEditor : Editor 
{
	// target component
	public VRButtonExample button = null;
	public SerializedObject serializedButton;

	private bool pressed;

	public void OnEnable()
	{
		button = (VRButtonExample)target;
		serializedButton = new SerializedObject(button);
	}

	public override void OnInspectorGUI()
	{
		serializedButton.Update();

		SerializedProperty targetObject = serializedButton.FindProperty("targetObject");
		targetObject.objectReferenceValue = EditorGUILayout.ObjectField("Target Object", targetObject.objectReferenceValue, typeof(GameObject), true);

		SerializedProperty useLocal = serializedButton.FindProperty("useLocal");
		useLocal.boolValue = EditorGUILayout.Toggle("Use Local", useLocal.boolValue);

		SerializedProperty defaultPosition = serializedButton.FindProperty("defaultPosition");
		defaultPosition.vector3Value = EditorGUILayout.Vector3Field("Default Position", defaultPosition.vector3Value);

		if (GUILayout.Button("Set Current to Default"))
		{
			if (useLocal.boolValue)
				defaultPosition.vector3Value = button.transform.localPosition;
			else
				defaultPosition.vector3Value = button.transform.position;
			pressed = false;
		}

		SerializedProperty pressedPosition = serializedButton.FindProperty("pressedPosition");
		pressedPosition.vector3Value = EditorGUILayout.Vector3Field("Pressed Position", pressedPosition.vector3Value);

		if (GUILayout.Button("Set Current to Pressed"))
		{
			if (useLocal.boolValue)
				pressedPosition.vector3Value = button.transform.localPosition;
			else
				pressedPosition.vector3Value = button.transform.position;
			pressed = true;
		}

		if (GUILayout.Button("Toggle"))
		{
			if (pressed)
			{
				if (useLocal.boolValue)
					button.transform.localPosition = defaultPosition.vector3Value;
				else
					button.transform.position = defaultPosition.vector3Value;
			} else
			{
				if (useLocal.boolValue)
					button.transform.localPosition = pressedPosition.vector3Value;
				else
					button.transform.position = pressedPosition.vector3Value;
			}
			pressed = !pressed;
		}

		serializedButton.ApplyModifiedProperties();
	}
}
