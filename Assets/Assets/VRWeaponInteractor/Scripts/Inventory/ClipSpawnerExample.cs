using UnityEngine;
using System.Collections;

public class ClipSpawnerExample : MonoBehaviour {

	public GameObject clipPrefab;
	public Vector3 localPosition;
	public Vector3 localRotation;

	VRInventorySlot slot;

	void Start()
	{
		slot = GetComponent<VRInventorySlot>();
		if (slot == null)
		{
			Debug.LogWarning("No inventory slot found. Make sure this script is on the slot object", gameObject);
			return;
		}
		if (clipPrefab == null)
		{
			Debug.LogWarning("No prefab provided", gameObject);
			return;
		}
		SpawnNewClip();
	}

	public void ClipRemoved(VRInventoryItem newWeapon)
	{
		newWeapon.item.hoverSlot = null;
		SpawnNewClip();
	}

	void SpawnNewClip()
	{
		GameObject clipInstance = (GameObject)Instantiate(clipPrefab);
		clipInstance.transform.parent = transform;
		clipInstance.transform.localPosition = localPosition;
		clipInstance.transform.localRotation = Quaternion.Euler(localRotation);
		VRInteractableItem.FreezeItem(clipInstance, false);

		VRInventoryItem inventItem = clipInstance.GetComponentInChildren<VRInventoryItem>();
		if (inventItem == null)
		{
			Debug.LogWarning("Prefab does not have an inventory item attached", gameObject);
			return;
		}
		slot.hoverItem = inventItem;
		inventItem.item.hoverSlot = slot;
		slot.AddItem(inventItem);
	}
}
