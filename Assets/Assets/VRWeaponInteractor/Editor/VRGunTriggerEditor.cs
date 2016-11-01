using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VRGunTrigger))]
public class VRGunTriggerEditor : Editor {

	// target component
	public VRGunTrigger m_Component = null;

	public void OnEnable()
	{
		m_Component = (VRGunTrigger)target;
	}

	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Open Gun Handler Editor"))
		{
			GunHandlerWindow newWindow = (GunHandlerWindow)EditorWindow.GetWindow(typeof(GunHandlerWindow), true, "Gun Handler", true);
			newWindow.gunHandler = m_Component.gunHandler;
			newWindow.Init();
			newWindow.weaponTab = GunHandlerWindow.WeaponTab.TRIGGER;
		}
	}
}
