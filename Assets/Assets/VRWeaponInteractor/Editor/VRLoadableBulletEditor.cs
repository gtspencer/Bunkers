using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VRLoadableBullet))]
public class VRLoadableBulletEditor : VRInteractableItemEditor {

	// target component
	public VRLoadableBullet loadableBullet = null;
	SerializedObject serializedBullet;

	public override void OnEnable()
	{
		loadableBullet = (VRLoadableBullet)target;
		serializedBullet = new SerializedObject(loadableBullet);
		base.OnEnable();
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		serializedBullet.Update();
		SerializedProperty bulletId = serializedBullet.FindProperty("bulletId");
		bulletId.intValue = EditorGUILayout.IntField("Bullet Id", bulletId.intValue);
	}
}
