using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VRMagazine))]
public class VRMagazineEditor : VRInteractableItemEditor
{
	// target component
	public VRMagazine magazine = null;
	SerializedObject serializedMagazine;

	override public void OnEnable()
	{
		base.OnEnable();
		magazine = (VRMagazine)target;
		serializedMagazine = new SerializedObject(magazine);
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		serializedMagazine.Update();

		SerializedProperty bulletId = serializedMagazine.FindProperty("bulletId");
		bulletId.intValue = EditorGUILayout.IntField("Bullet Id", bulletId.intValue);

		SerializedProperty magazineId = serializedMagazine.FindProperty("magazineId");
		magazineId.intValue = EditorGUILayout.IntField("Magazine Id", magazineId.intValue);

		serializedMagazine.ApplyModifiedProperties();
	}
}
