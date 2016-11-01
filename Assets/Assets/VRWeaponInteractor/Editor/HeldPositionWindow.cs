using UnityEngine;
using UnityEditor;
using System.Collections;

public class HeldPositionWindow : EditorWindow 
{
	public VRInteractableItem interactableItem;
	public SerializedObject serializedItem;

	//For Applying magazine held position
	public GunHandlerWindow gunHandlerWindow;
	public GameObject magazinePrefab;

	static GameObject viveController;

	Vector3 oldPosition;
	Quaternion oldRotation;
	Transform oldParent;

	public void Init()
	{
		if (interactableItem == null) return;
		if (serializedItem == null)
			serializedItem = new SerializedObject(interactableItem);
	}

	void OnGUI()
	{
		var oldInteractableItem = interactableItem;
		interactableItem = (VRInteractableItem)EditorGUILayout.ObjectField("Interactive Item", interactableItem, typeof(VRInteractableItem), true);
		if (interactableItem == null) return;
		if (oldInteractableItem != interactableItem || serializedItem == null)
			Init();

		serializedItem.Update();

		SerializedProperty item = serializedItem.FindProperty("item");
		if (item.objectReferenceValue == null)
		{
			item.objectReferenceValue = EditorGUILayout.ObjectField("Item", item.objectReferenceValue, typeof(Transform), true);
			if (item.objectReferenceValue == null) return;
		}

		SerializedProperty heldPosition = serializedItem.FindProperty("heldPosition");
		heldPosition.vector3Value = EditorGUILayout.Vector3Field("Held Position", heldPosition.vector3Value);
		SerializedProperty heldRotation = serializedItem.FindProperty("heldRotation");
		Quaternion tempHeldRotation = heldRotation.quaternionValue;
		tempHeldRotation.eulerAngles = EditorGUILayout.Vector3Field("Held Rotation", tempHeldRotation.eulerAngles);
		heldRotation.quaternionValue = tempHeldRotation;
		bool updatePrefab = false;
		if (viveController == null)
		{
			if (GUILayout.Button("Create Reference Controller"))
			{
				GameObject viveControllerPrefab = Resources.Load<GameObject>("ViveController");
				if (viveControllerPrefab != null)
				{
					viveController = (GameObject)Instantiate(viveControllerPrefab, Vector3.zero, Quaternion.identity);
					Undo.RegisterCreatedObjectUndo(viveController, "Create Reference Controller");
					oldPosition = interactableItem.item.position;
					oldRotation = interactableItem.item.rotation;
					oldParent = interactableItem.item.parent;
					interactableItem.item.SetParent(viveController.transform);
					interactableItem.item.localPosition = heldPosition.vector3Value;
					interactableItem.item.localRotation = heldRotation.quaternionValue;
					Vector3 diff = oldPosition - interactableItem.item.position;
					viveController.transform.position = diff+heldPosition.vector3Value;
					viveController.transform.rotation = oldRotation;
					Selection.activeGameObject = interactableItem.item.gameObject;
				} else
				{
					Debug.LogError("Can't find ViveController in resources");
				}
			}
		} else
		{
			EditorGUILayout.HelpBox("Make sure to move the target item and not just the object with this script on", MessageType.Info);
			if (GUILayout.Button("Save"))
			{
				heldPosition.vector3Value = interactableItem.item.localPosition;
				heldRotation.quaternionValue = interactableItem.item.localRotation;
				Undo.SetTransformParent(interactableItem.item, oldParent, "Save Changes");
				Undo.RecordObject(interactableItem.item, "Save Changes");
				interactableItem.item.position = oldPosition;
				interactableItem.item.rotation = oldRotation;
				Undo.DestroyObjectImmediate(viveController);
				if (magazinePrefab != null && gunHandlerWindow != null)
					updatePrefab = true;
			}
			if (GUILayout.Button("Select Controller"))
			{
				Selection.activeGameObject = viveController;
			}
			if (GUILayout.Button("Select Item"))
			{
				Selection.activeGameObject = interactableItem.item.gameObject;
			}
			if (GUILayout.Button("Cancel"))
			{
				Undo.SetTransformParent(interactableItem.item, oldParent, "Cancel");
				Undo.RecordObject(interactableItem.item, "Save Changes");
				interactableItem.item.position = oldPosition;
				interactableItem.item.rotation = oldRotation;
				Undo.DestroyObjectImmediate(viveController);
			}
		}
		serializedItem.ApplyModifiedProperties();
		if (updatePrefab && magazinePrefab != null && gunHandlerWindow != null)
			gunHandlerWindow.SaveMagazinePrefab(magazinePrefab);

	}

	void OnDestroy()
	{
		if (interactableItem == null || viveController == null) return;
		Undo.SetTransformParent(interactableItem.item, oldParent, "Close");
		Undo.RecordObject(interactableItem.item, "Close");
		interactableItem.item.position = oldPosition;
		interactableItem.item.rotation = oldRotation;
		Undo.DestroyObjectImmediate(viveController);
	}
}
