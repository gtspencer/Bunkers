using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VRGunSlide))]
public class VRGunSlideEditor : VRInteractableItemEditor 
{
	// target component
	public VRGunSlide gunSlide = null;
	public SerializedObject serializedGunSlide;

	override public void OnEnable()
	{
		gunSlide = (VRGunSlide)target;
		serializedGunSlide = new SerializedObject(gunSlide);
		base.OnEnable();
	}

	public override void OnInspectorGUI()
	{
		SerializedProperty gunHandler = serializedGunSlide.FindProperty("gunHandler");
		if (gunHandler.objectReferenceValue == null)
		{
			serializedGunSlide.Update();
			gunHandler.objectReferenceValue = EditorGUILayout.ObjectField("Gun Handler", gunHandler.objectReferenceValue, typeof(VRGunHandler), true);
			if (gunHandler.objectReferenceValue != null)
			{
				SerializedObject serializedGunHandler = new SerializedObject(gunHandler.objectReferenceValue);
				serializedGunHandler.Update();
				SerializedProperty gunHandlersGunSlide = serializedGunHandler.FindProperty("slide");
				gunHandlersGunSlide.objectReferenceValue = gunSlide;
				serializedGunHandler.ApplyModifiedProperties();
			}

			SerializedProperty item = serializedGunSlide.FindProperty("item");
			item.objectReferenceValue = gunSlide.transform;

			Renderer renderer = gunSlide.GetComponent<Renderer>();
			if (renderer != null)
			{
				SerializedProperty hover = serializedGunSlide.FindProperty("hover");
				hover.objectReferenceValue = renderer;
			}


			serializedGunSlide.ApplyModifiedProperties();
		} else
		{
			if (GUILayout.Button("Open Gun Handler Editor"))
			{
				GunHandlerWindow newWindow = (GunHandlerWindow)EditorWindow.GetWindow(typeof(GunHandlerWindow), true, "Gun Handler", true);
				newWindow.gunHandler = gunSlide.gunHandler;
				newWindow.Init();
				newWindow.weaponTab = GunHandlerWindow.WeaponTab.SLIDE;
			}
		}
	}
}