using UnityEngine;
using System.Collections;

public class VRInventorySlot : MonoBehaviour {

	public VRInventory inventory;
	VRInventoryItem _hoverItem;
	public VRInventoryItem hoverItem
	{
		get { return _hoverItem; }
		set { _hoverItem = value; }
	}
	VRInventoryItem _currentItem;
	public VRInventoryItem currentItem
	{
		get { return _currentItem; }
		set { _currentItem = value; }
	}

	public void AddItem(VRInventoryItem newItem)
	{
		if (newItem == null || newItem.slotMethodName == "") return;
		currentItem = newItem;
		SendMessage(newItem.slotMethodName, newItem, SendMessageOptions.DontRequireReceiver);
	}

	public void RemoveItem(VRInventoryItem item)
	{
		if (item == null || item.slotMethodName == "") return;
		currentItem = null;
		SendMessage(item.slotMethodName+"Removed", item, SendMessageOptions.DontRequireReceiver);
	}

	void OnTriggerEnter(Collider col)
	{
		if (currentItem != null) return;
		VRInventoryItem inventItem = col.GetComponent<VRInventoryItem>();
		if (inventItem != null)
		{
			if (inventItem.item != null) 
			{
				if (inventItem.item.heldBy == null) return;
				inventItem.item.hoverSlot = this;
			}

			if (hoverItem != null && hoverItem.item != null) hoverItem.item.hoverSlot = null;
			hoverItem = inventItem;
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (currentItem != null) return;
		VRInventoryItem inventItem = col.GetComponent<VRInventoryItem>();
		if (inventItem != null && inventItem == hoverItem)
		{
			if (hoverItem != null && hoverItem.item != null) hoverItem.item.hoverSlot = null;
			hoverItem = null;
		}
	}
}
