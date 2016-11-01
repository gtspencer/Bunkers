using UnityEngine;
using System.Collections;

public class VRSpawnItemControls : MonoBehaviour {

	public GameObject spawnItem;
	public float spawnDelay = 1f;

	private float spawnDelayTimer;
	VRInteractor interactor;

	void Start()
	{
		interactor = GetComponent<VRInteractor>();
		if (interactor == null) Debug.LogWarning("Default controls require an interactor to work");
	}

	virtual public void SPAWNING()
	{
		if (interactor.heldItem == null && spawnDelayTimer < Time.time)
		{
			GameObject spawnItemInstance = GameObject.Instantiate(spawnItem);
			spawnItemInstance.transform.position = transform.position;
			VRInteractableItem interactableItem = spawnItemInstance.GetComponentInChildren<VRInteractableItem>();
			interactor.hoverItem = interactableItem;
			interactor.TryPickup();
			spawnDelayTimer = Time.time + spawnDelay;
		}
	}

	virtual public void SPAWNINGReleased()
	{
		if (interactor.heldItem != null)
		{
			if (!interactor.heldItem.toggleToPickup)
				interactor.Drop();
		}
	}
}
