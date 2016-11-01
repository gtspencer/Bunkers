using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VRGunHandler))]
public class VRGunHandlerEditor : VRInteractableItemEditor
{
	// target component
	public VRGunHandler gunHandler = null;

	override public void OnEnable()
	{
		gunHandler = (VRGunHandler)target;
		base.OnEnable();
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("Open Gun Handler Editor"))
		{
			GunHandlerWindow newWindow = (GunHandlerWindow)EditorWindow.GetWindow(typeof(GunHandlerWindow), true, "Gun Handler", true);
			newWindow.gunHandler = gunHandler;
			newWindow.Init();
			newWindow.weaponTab = GunHandlerWindow.WeaponTab.MAIN;
		}
	}
}
