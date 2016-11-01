using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VRInteractor))]
public class VRInteractorEditor : Editor {

	// target component
	public VRInteractor interactor = null;
	SerializedObject serializedInteractor;

	bool badWeaponReference = false;
	static bool editActionsFoldout;
	string newActionName = "";

	public void OnEnable()
	{
		interactor = (VRInteractor)target;
		serializedInteractor = new SerializedObject(interactor);
	}

	public override void OnInspectorGUI()
	{
		serializedInteractor.Update();

		if (interactor.VRActions == null || GUILayout.Button("Reset Actions To Default"))
		{
			string[] premadeActions = {"NONE", "ACTION", "PICKUP_DROP", "EJECT"};
			interactor.triggerKey = 1;
			interactor.padTop = 2;
			interactor.padLeft = 2;
			interactor.padRight = 2;
			interactor.padBottom = 3;
			interactor.padCentre = 2;
			interactor.gripKey = 0;
			interactor.menuKey = 0;
			interactor.VRActions = premadeActions;
			EditorUtility.SetDirty(interactor);
		}

		editActionsFoldout = EditorGUILayout.Foldout(editActionsFoldout, "Edit Actions");
		if (editActionsFoldout)
		{
			for(int i=0; i<interactor.VRActions.Length; i++)
			{
				EditorGUILayout.BeginHorizontal();
				interactor.VRActions[i] = EditorGUILayout.TextField(interactor.VRActions[i]);
				if (GUILayout.Button("X"))
				{
					string[] newActions = new string[interactor.VRActions.Length-1];
					int offset = 0;
					for(int j=0; j<newActions.Length; j++)
					{
						if (i == j) offset = 1;
						newActions[j] = interactor.VRActions[j+offset];
					}
					interactor.VRActions = newActions;

					if (interactor.triggerKey > i)
						interactor.triggerKey -= 1;
					else if (interactor.triggerKey == i)
						interactor.triggerKey = 0;
					if (interactor.padTop > i)
						interactor.padTop -= 1;
					else if (interactor.padTop == i)
						interactor.padTop = 0;
					if (interactor.padLeft > i)
						interactor.padLeft -= 1;
					else if (interactor.padLeft == i)
						interactor.padLeft = 0;
					if (interactor.padRight > i)
						interactor.padRight -= 1;
					else if (interactor.padRight == i)
						interactor.padRight = 0;
					if (interactor.padBottom > i)
						interactor.padBottom -= 1;
					else if (interactor.padBottom == i)
						interactor.padBottom = 0;
					if (interactor.padCentre > i)
						interactor.padCentre -= 1;
					else if (interactor.padCentre == i)
						interactor.padCentre = 0;
					if (interactor.gripKey > i)
						interactor.gripKey -= 1;
					else if (interactor.gripKey == i)
						interactor.gripKey = 0;
					if (interactor.menuKey > i)
						interactor.menuKey -= 1;
					else if (interactor.menuKey == i)
						interactor.menuKey = 0;

					EditorUtility.SetDirty(interactor);
					break;
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.BeginHorizontal();
			newActionName = EditorGUILayout.TextField(newActionName);
			GUI.enabled = (newActionName != "");
			if (GUILayout.Button("Add Action"))
			{
				string[] newActions = new string[interactor.VRActions.Length+1];
				for(int i=0; i<newActions.Length; i++)
				{
					if (i == interactor.VRActions.Length)
					{
						newActions[i] = newActionName;
						break;
					}
					newActions[i] = interactor.VRActions[i];
				}
				interactor.VRActions = newActions;
				newActionName = "";
				EditorUtility.SetDirty(interactor);
			}
			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();
		}


		SerializedProperty triggerKey = serializedInteractor.FindProperty("triggerKey");
		SerializedProperty padTop = serializedInteractor.FindProperty("padTop");
		SerializedProperty padLeft = serializedInteractor.FindProperty("padLeft");
		SerializedProperty padRight = serializedInteractor.FindProperty("padRight");
		SerializedProperty padBottom = serializedInteractor.FindProperty("padBottom");
		SerializedProperty padCentre = serializedInteractor.FindProperty("padCentre");
		SerializedProperty gripKey = serializedInteractor.FindProperty("gripKey");
		SerializedProperty menuKey = serializedInteractor.FindProperty("menuKey");

		triggerKey.intValue = EditorGUILayout.Popup("Trigger Key", triggerKey.intValue, interactor.VRActions);
		padTop.intValue = EditorGUILayout.Popup("Pad Top Key", padTop.intValue, interactor.VRActions);
		padLeft.intValue = EditorGUILayout.Popup("Pad Left Key", padLeft.intValue, interactor.VRActions);
		padRight.intValue = EditorGUILayout.Popup("Pad Right Key", padRight.intValue, interactor.VRActions);
		padBottom.intValue = EditorGUILayout.Popup("Pad Bottom Key", padBottom.intValue, interactor.VRActions);
		padCentre.intValue = EditorGUILayout.Popup("Pad Centre Key", padCentre.intValue, interactor.VRActions);
		gripKey.intValue = EditorGUILayout.Popup("Grip Key", gripKey.intValue, interactor.VRActions);
		menuKey.intValue = EditorGUILayout.Popup("Menu Key", menuKey.intValue, interactor.VRActions);

		SerializedProperty ejectKeyCanPickup = serializedInteractor.FindProperty("ejectKeyCanPickup");
		ejectKeyCanPickup.boolValue = EditorGUILayout.Toggle("Eject Can Pickup", ejectKeyCanPickup.boolValue);

		SerializedProperty actionKeyCanPickup = serializedInteractor.FindProperty("actionKeyCanPickup");
		actionKeyCanPickup.boolValue = EditorGUILayout.Toggle("Action Can Pickup", actionKeyCanPickup.boolValue);

		EditorGUILayout.HelpBox("The eject and action booleans will allow you to pick up when not holding anything", MessageType.Info);

		SerializedProperty interactorSphereSize = serializedInteractor.FindProperty("interactorSphereSize");
		interactorSphereSize.floatValue = EditorGUILayout.FloatField("Interactor Sphere Size", interactorSphereSize.floatValue);

		SerializedProperty referenceWeapon = serializedInteractor.FindProperty("weaponReference");
		referenceWeapon.objectReferenceValue = EditorGUILayout.ObjectField("Weapon Reference", referenceWeapon.objectReferenceValue, typeof(GameObject), true);
		if (badWeaponReference) EditorGUILayout.HelpBox("Weapon reference must be an instance in the scene and must have a Weapon Interactor script attached", MessageType.Warning);
		if (referenceWeapon.objectReferenceValue != null)
		{
			badWeaponReference = false;
			PrefabType weaponType = PrefabUtility.GetPrefabType(referenceWeapon.objectReferenceValue);
			VRGunHandler gunHandler = ((GameObject)referenceWeapon.objectReferenceValue).GetComponentInChildren<VRGunHandler>();
			if (weaponType == PrefabType.ModelPrefab || weaponType == PrefabType.Prefab || gunHandler == null)
			{
				badWeaponReference = true;
				referenceWeapon.objectReferenceValue = null;
			}
		}

		SerializedProperty debugMode = serializedInteractor.FindProperty("debugMode");
		debugMode.boolValue = EditorGUILayout.Toggle("Debug Mode", debugMode.boolValue);

		serializedInteractor.ApplyModifiedProperties();
	}
}
