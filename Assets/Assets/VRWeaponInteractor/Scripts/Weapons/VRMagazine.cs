using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VRMagazine : VRInteractableItem {

	//Settings
	public bool integrated = false;
	public bool infiniteAmmo = false;
	public int clipSize = 16;
	public int magazineId;
	public int bulletId;
	public bool autoLoad = false;
	public float autoLoadSpeed = 1.0f;

	//References
	public BoxCollider bulletReceiver;

	//Variables
	public Vector3 defaultLoadedPosition = Vector3.zero;
	public Vector3 entryPosition = Vector3.zero;
	public Quaternion defaultRotation = Quaternion.identity;

	public GameObject bulletPrefab;
	public List<bool> bulletVisible = new List<bool>();
	public List<Vector3> bulletPositions = new List<Vector3>();
	public List<Quaternion> bulletRotations = new List<Quaternion>();

	//References
	private List<GameObject> loadedBullets = new List<GameObject>();

	//Variables
	private bool autoLoading;
	private bool loaded = false;
	private VRGunHandler _currentGun;
	private int _bulletCount;
	public int bulletCount
	{
		get 
		{
			if (infiniteAmmo) return 1;
			return _bulletCount;
		}
	}
	public VRGunHandler currentGun
	{
		get { return _currentGun; }
		set { _currentGun = value; }
	}

	void Start () 
	{
		Init();
	}

	void Update()
	{
		Step();
	}

	override protected void Init()
	{
		if (!integrated)
		{
			base.Init();
			//	Make sure gun is my parent
			if (currentGun != null && item.parent != currentGun.item)
				currentGun = null;

			//	Lock the magazine to the gun or activate physics
			if (currentGun != null)
			{
				item.SetParent(currentGun.item);
				item.localPosition = defaultLoadedPosition;
				item.localRotation = defaultRotation;
				parentItem = currentGun;
				VRInteractableItem.FreezeItem(item.gameObject, true);
				loaded = true;
			}
		}
		_bulletCount = clipSize;
		if (bulletPrefab != null)
		{
			//	Spawn bullets
			for(int i=0; i<bulletVisible.Count;i++)
			{
				if (!bulletVisible[i])
				{
					loadedBullets.Add(null);
					continue;
				}
				if (bulletPositions.Count <= i) break;
				if (bulletRotations.Count <= i) break;

				GameObject newBullet = (GameObject)Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
				newBullet.transform.SetParent(item);
				newBullet.transform.localPosition = bulletPositions[i];
				newBullet.transform.localRotation = bulletRotations[i];
				if (newBullet.GetComponentInChildren<VRInteractableItem>() != null) Destroy(newBullet.GetComponentInChildren<VRInteractableItem>());
				VRInteractableItem.FreezeItem(newBullet, true);
				loadedBullets.Add(newBullet);
			}
		}
	}

	override protected void Step()
	{
		if (integrated) return;
		if (currentGun == null)
		{
			base.Step();
		} else
		{
			if (heldBy != null)
			{
				Vector3 currentPos = ControllerPositionToLocal();
				item.localPosition = VRUtils.ClosestPointOnLine(entryPosition, defaultLoadedPosition, currentPos);
			} else if (!loaded)
			{
				if (autoLoading)
				{
					// Auto moving to loaded position
					item.localPosition = Vector3.MoveTowards(item.localPosition, defaultLoadedPosition, autoLoadSpeed);
				} else
				{
					//	Using gravity to slide magazine
					Vector3 direction = defaultLoadedPosition - entryPosition;
					float gravity = currentGun.item.TransformPoint(entryPosition).y - currentGun.item.TransformPoint(defaultLoadedPosition).y;
					item.localPosition = VRUtils.ClosestPointOnLine(entryPosition, defaultLoadedPosition, item.localPosition+(direction*gravity));
				}
			}

			if (loaded && item.localPosition != defaultLoadedPosition)
			{
				Eject();
			}
			if (item.localPosition == entryPosition)
			{
				DetatchFromGun();
			} else if (item.localPosition == defaultLoadedPosition && !loaded)
			{
				if (heldBy != null) heldBy.Drop();
				autoLoading = false;
				LoadIntoGun();
			}
		}
	}

	private Vector3 ControllerPositionToLocal()
	{
		if (heldBy == null || currentGun == null) return Vector3.zero;
		Vector3 localPosition =  currentGun.item.InverseTransformPoint(heldBy.transform.position);
		Vector3 rotatedOffset = Quaternion.Inverse(heldRotation) * defaultRotation * heldPosition;
		Vector3 scaledOffset = new Vector3(rotatedOffset.x/currentGun.item.localScale.x, rotatedOffset.y/currentGun.item.localScale.y, rotatedOffset.z/currentGun.item.localScale.z);
		return localPosition + scaledOffset;
	}

	override public bool Pickup(VRInteractor hand)
	{
		if (integrated) return false;
		if (currentGun != null && currentGun.currentMagazine == this)
		{
			//Grabbed magazine while in gun
			loaded = true;
			heldBy = hand;
		} else if (currentGun != null && currentGun.currentMagazine == null)
		{
			//Grabbed magazine while on the track
			heldBy = hand;
		} else
		{
			if (currentGun != null && currentGun.currentMagazine != this) currentGun = null; //current gun has a reference to a magazine that is not this
			//Grabbed magazine with no gun
			base.Pickup(hand);
		}
		return true;
	}

	override public void Drop(SteamVR_TrackedObject trackedObj)
	{
		if (integrated) return;
		if (currentGun != null)
		{
			heldBy = null;
			return;
		}
		base.Drop(trackedObj);
	}

	override public void EnableHover()
	{
		if (integrated) return;
		base.EnableHover();
	}

	override public void DisableHover()
	{
		if (integrated) return;
		base.DisableHover();
	}

	protected void LoadIntoGun()
	{
		if (integrated) return;
		if (currentGun == null)
		{
			Debug.LogError("Tried to load into null gun");
			return;
		}
		if (currentGun.loadMagazine != null) currentGun.PlaySound(currentGun.loadMagazine);
		item.localPosition = defaultLoadedPosition;
		item.localRotation = defaultRotation;
		parentItem = currentGun;
		currentGun.currentMagazine = this;
		loaded = true;
	}

	public bool LoadBullet(VRLoadableBullet bullet)
	{
		if (bullet == null) return false;
		if (bulletId != bullet.bulletId) return false;
		if (integrated && _currentGun != null && _currentGun.LoadBullet(bullet))
			return false;
		if (bulletCount >= clipSize) return false;
		if (bullet.heldBy != null) bullet.heldBy.Drop();
		_bulletCount++;

		if (bulletPrefab != null)
		{
			if (bulletVisible.Count <= bulletCount-1 || !bulletVisible[bulletCount-1] || bulletPositions.Count <= bulletCount-1 || bulletRotations.Count <= bulletCount-1)
			{
				Destroy(bullet.item.gameObject);
			} else
			{
				bullet.item.SetParent(item);
				bullet.item.localPosition = bulletPositions[bulletCount-1];
				bullet.item.localRotation = bulletRotations[bulletCount-1];
				if (bullet.item.GetComponentInChildren<VRInteractableItem>() != null) Destroy(bullet.item.GetComponentInChildren<VRInteractableItem>());
				VRInteractableItem.FreezeItem(bullet.item.gameObject, true);
				loadedBullets.Insert(bulletCount-1, bullet.item.gameObject);
			}
		} else
		{
			Destroy(bullet.item.gameObject);
		}
		return true;
	}

	public void Eject()
	{
		if (integrated) return;
		if (currentGun == null || currentGun.currentMagazine != this) return;
		if (currentGun != null && currentGun.unloadMagazine != null) currentGun.PlaySound(currentGun.unloadMagazine);
		loaded = false;
		parentItem = null;
		currentGun.currentMagazine = null;
		if (heldBy == null)
		{
			loaded = false;
			Vector3 direction = defaultLoadedPosition - entryPosition;
			float gravity = currentGun.item.TransformPoint(entryPosition).y - currentGun.item.TransformPoint(defaultLoadedPosition).y;
			item.localPosition = VRUtils.ClosestPointOnLine(entryPosition, defaultLoadedPosition, item.localPosition+(direction*gravity));
		}
	}

	protected void DetatchFromGun()
	{
		if (integrated) return;
		if (heldBy == null)
		{
			hoverSlot = null;
			Drop();
		} else
		{
			item.SetParent(heldBy.transform);
		}
		currentGun = null;
	}

	public bool TakeBullet()
	{
		if (infiniteAmmo) return true;

		if (bulletCount <= 0) 
			return false;

		if (bulletVisible.Count >= bulletCount && bulletVisible[bulletCount-1])
			Destroy(loadedBullets[bulletCount-1]);

		_bulletCount--;
		return true;
	}

	void OnTriggerStay(Collider col)
	{
		if (currentGun != null || integrated) return;
		VRClipReceiver clipReceiver = col.GetComponent<VRClipReceiver>();
		if (clipReceiver == null) return;

		currentGun = clipReceiver.gunHandler;
		if (currentGun == null) currentGun = col.transform.parent.GetComponentInChildren<VRGunHandler>();
		if (currentGun == null)
		{
			Debug.LogError("Found a clip receiver but no gun handler");
			return;
		}
		if (currentGun.magazineId != magazineId || currentGun.currentMagazine != null || currentGun.item.GetComponentInChildren<VRMagazine>() != null)
		{
			currentGun = null;
			return;
		}

		//Controller position or if not held current position
		Vector3 currentPosition = Vector3.zero;
		if (heldBy != null)
			currentPosition = ControllerPositionToLocal();
		else
			currentPosition = currentGun.item.InverseTransformPoint(item.position);
			Vector3 position = VRUtils.ClosestPointOnLine(entryPosition, defaultLoadedPosition, currentPosition);

		//Ignore collision if below entry point
		if (position == entryPosition && defaultLoadedPosition != entryPosition)
		{
			DetatchFromGun();
			return;
		}
			
		if (hoverSlot != null && inventoryItem != null && hoverSlot.currentItem == inventoryItem)
		{
			hoverSlot.RemoveItem(inventoryItem);
		}

		if (autoLoad) 
		{
			autoLoading = true;
		}

		//Attach to gun and start along entry path
		item.SetParent(currentGun.item);
		item.localPosition = position;
		item.localRotation = defaultRotation;
		VRInteractableItem.FreezeItem(item.gameObject, true);
	}
}
