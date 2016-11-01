using UnityEngine;
using System.Collections;

public class VRInventoryItem : MonoBehaviour 
{
	public VRInteractableItem item;
	public string slotMethodName = "";
	public string id;

	void Start()
	{
		if (item != null) item.inventoryItem = this;
	}
}
