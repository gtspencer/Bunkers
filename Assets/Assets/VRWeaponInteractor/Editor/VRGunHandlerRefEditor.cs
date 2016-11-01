using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VRGunHandlerRef))]
public class VRGunHandlerRefEditor : Editor
{
	// target component
	public VRGunHandlerRef gunHandlerRef = null;
	SerializedObject serializedRef;

	public void OnEnable()
	{
		gunHandlerRef = (VRGunHandlerRef)target;
		serializedRef = new SerializedObject(gunHandlerRef);
	}

	public override void OnInspectorGUI()
	{
		serializedRef.Update();
		SerializedProperty currentGun = serializedRef.FindProperty("gunHandler");
		currentGun.objectReferenceValue = EditorGUILayout.ObjectField("Gun Handler", currentGun.objectReferenceValue, typeof(VRGunHandler), true);
		serializedRef.ApplyModifiedProperties();

		if (GUILayout.Button("Open Gun Handler Editor"))
		{
			GunHandlerWindow newWindow = (GunHandlerWindow)EditorWindow.GetWindow(typeof(GunHandlerWindow), true, "Gun Handler", true);
			newWindow.gunHandler = (VRGunHandler)currentGun.objectReferenceValue;
			newWindow.Init();
			newWindow.weaponTab = GunHandlerWindow.WeaponTab.MAIN;
		}
		if (GUILayout.Button("Select Gun Handler"))
		{
			Selection.activeGameObject = ((VRGunHandler)currentGun.objectReferenceValue).gameObject;
		}
	}
}
