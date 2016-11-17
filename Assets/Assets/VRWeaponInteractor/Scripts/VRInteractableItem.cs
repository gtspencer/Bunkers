using UnityEngine;
using System.Collections;

public class VRInteractableItem : MonoBehaviour {

	public enum HoverMode
	{
		SHADER,
		MATERIAL
	}

	//References
	public VRInventoryItem inventoryItem;
	public Transform item;
	public Renderer hover;

	//Set parent if this item can't be interacted with unless the parent is being held
	public VRInteractableItem parentItem;

	//Variables
	public bool canBeHeld = true;
	public Vector3 heldPosition = Vector3.zero;
	public Quaternion heldRotation = Quaternion.identity;
	public HoverMode hoverMode = HoverMode.SHADER;
	public Shader defaultShader;
	public Shader hoverShader;
	public Material defaultMat;
	public Material hoverMat;
	public float heldMovementSpeed = 0.01f;
	public float heldRotationSpeed = 10f;
	public float breakLimit = 0.5f;
	public bool toggleToPickup = true;

	//Sounds
	protected AudioSource soundSource;
	public AudioClip enterHover;
	public AudioClip exitHover;

	//protected string originalShaderName;
	protected Rigidbody selfBody;
	protected Collider itemCollider;
	protected bool activeHover = false;

	VRInventorySlot _hoverSlot;
	public VRInventorySlot hoverSlot
	{
		get { return _hoverSlot; }
		set { _hoverSlot = value; }
	}

	VRInteractor _heldBy;
	public VRInteractor heldBy
	{
		get { return _heldBy; }
		set { _heldBy = value; }
	}

	void Start()
	{
		Init();
	}

	void Update()
	{
		Step();
	}

	virtual protected void Init()
	{
		selfBody = item.GetComponent<Rigidbody>();
		itemCollider = item.GetComponent<Collider>();
		soundSource = GetComponent<AudioSource>();
		if (hover != null)
		{
			switch(hoverMode)
			{
			case HoverMode.SHADER:
				if (hover.material == null) break;
				if (hoverShader == null) hoverShader = Shader.Find("Unlit/Texture");
				if (defaultShader == null) defaultShader = hover.material.shader;
				else hover.material.shader = defaultShader;
				break;
			case HoverMode.MATERIAL:
				if (hoverMat == null)
				{
					hoverMat = new Material(hover.material);
					hoverMat.shader = Shader.Find("Unlit/Texture");
				}
				if (defaultMat == null) defaultMat = hover.material;
				else hover.material = defaultMat;
				break;
			}
		}
	}

	virtual protected void Step()
	{
		if (heldBy == null || !canBeHeld) return;
		item.localPosition = Vector3.MoveTowards(item.localPosition, heldPosition, heldMovementSpeed);
		item.localRotation = Quaternion.RotateTowards(item.localRotation, heldRotation, heldRotationSpeed);
	}

	virtual public bool Pickup(VRInteractor hand)
	{
		if (canBeHeld)
		{
			item.SetParent(hand.transform);
			VRInteractableItem.FreezeItem(item.gameObject, false);
			if (hoverSlot != null && inventoryItem != null && hoverSlot.currentItem == inventoryItem)
			{
				hoverSlot.RemoveItem(inventoryItem);
			}
		}
		heldBy = hand;
		return true;
	}

	virtual public void Drop(SteamVR_TrackedObject trackedObj)
	{
		if (canBeHeld)
		{
			item.parent = null;
			VRInteractableItem.UnFreezeItem(item.gameObject);
			if (selfBody != null)
			{
				var device = SteamVR_Controller.Input((int)trackedObj.index);
				var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
				if (origin != null)
				{
					selfBody.velocity = origin.TransformVector(device.velocity);
					selfBody.angularVelocity = origin.TransformVector(device.angularVelocity);
				}
				else
				{
					selfBody.velocity = device.velocity;
					selfBody.angularVelocity = device.angularVelocity;
				}
				selfBody.maxAngularVelocity = selfBody.angularVelocity.magnitude;
			}
			if (hoverSlot != null && inventoryItem != null)
			{
				hoverSlot.AddItem(inventoryItem);
			}
		}
		heldBy = null;
	}

	virtual public void Drop()
	{
		if (canBeHeld)
		{
			item.SetParent(null);
			VRInteractableItem.UnFreezeItem(item.gameObject);
			if (hoverSlot != null && inventoryItem != null)
			{
				hoverSlot.AddItem(inventoryItem);
			}
		}
		heldBy = null;
	}

	//Inherited and used by other subclasses
	virtual public void ActionPressed()
	{
	}

	//Inherited and used by other subclasses
	virtual public void ActionRelease()
	{
	}

    virtual public void addPressed()
    {
    }

    virtual public void addReleased()
    {
    }

    virtual public void flattenPressed()
    {
    }

    virtual public void flattenReleased()
    {
    }

    //Used as the Eject action
    virtual public void Action2Pressed()
	{
		if (heldBy != null) heldBy.Drop();
	}

	virtual public void Action2Release()
	{
	}
		
	virtual public void EnableHover()
	{
		if (hover == null || hover.material == null || activeHover) return;
		activeHover = true;
		if (soundSource != null && enterHover != null) PlaySound(enterHover);
		switch(hoverMode)
		{
		case HoverMode.SHADER:
			hover.material.shader = hoverShader;
			break;
		case HoverMode.MATERIAL:
			hover.material = hoverMat;
			break;
		}
	}

	virtual public void DisableHover()
	{
		if (hover == null || hover.material == null || !activeHover) return;
		activeHover = false;
		if (soundSource != null && exitHover != null) PlaySound(exitHover);
		switch(hoverMode)
		{
		case HoverMode.SHADER:
			hover.material.shader = defaultShader;
			break;
		case HoverMode.MATERIAL:
			hover.material = defaultMat;
			break;
		}
	}

	//Disable unity physics and collision. For moving item through code
	static public void FreezeItem(GameObject item, bool disableCollider)
	{
		Collider itemCollider = item.GetComponentInChildren<Collider>();
		Rigidbody itemBody = item.GetComponentInChildren<Rigidbody>();
		if (itemCollider != null) itemCollider.enabled = !disableCollider;
		if (itemBody != null)
		{
			itemBody.velocity = Vector3.zero;
			itemBody.useGravity = false;
			itemBody.isKinematic = true;
			itemBody.constraints = RigidbodyConstraints.FreezeAll;
		}
	}

	//Enable unity physics
	static public void UnFreezeItem(GameObject item)
	{
		Collider itemCollider = item.GetComponentInChildren<Collider>();
		Rigidbody itemBody = item.GetComponentInChildren<Rigidbody>();
		if (itemCollider != null) itemCollider.enabled = true;
		if (itemBody != null)
		{
			itemBody.useGravity = true;
			itemBody.isKinematic = false;
			itemBody.constraints = RigidbodyConstraints.None;
		}
	}

	public void PlaySound(AudioClip clip)
	{
		if (soundSource == null) return;
		soundSource.clip = clip;
		soundSource.Play();
	}
	
	void OnDestroy()
	{
		if (heldBy != null) heldBy.Drop();
	}
}
