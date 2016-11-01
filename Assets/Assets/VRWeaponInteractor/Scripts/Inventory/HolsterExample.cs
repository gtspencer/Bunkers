using UnityEngine;
using System.Collections;

public class HolsterExample : MonoBehaviour {

	public string id;
	public Vector3 localPosition;
	public Vector3 localRotation;
	VRInventoryItem currentPistol;

	public void Weapon(VRInventoryItem newWeapon)
	{
		if (currentPistol != null || newWeapon == null) return;
		if (newWeapon.id != id) return;
		if (newWeapon.item == null)
		{
			Debug.LogError("Inventory Item must reference corresponding Interactable Item", newWeapon.gameObject);
			return;
		}

		newWeapon.item.item.parent = transform;
		newWeapon.item.item.localPosition = localPosition;
		newWeapon.item.item.localRotation = Quaternion.Euler(localRotation);
		VRInteractableItem.FreezeItem(newWeapon.item.item.gameObject, false);
		currentPistol = newWeapon;
	}

	public void WeaponRemoved(VRInventoryItem newPistol)
	{
		currentPistol = null;
	}
}