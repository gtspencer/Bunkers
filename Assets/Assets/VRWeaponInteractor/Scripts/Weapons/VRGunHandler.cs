using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VRGunHandler : VRInteractableItem {

	public enum FiringMode
	{
		SEMI_AUTOMATIC,
		FULLY_AUTOMATIC,
		PUMP_OR_BOLT_ACTION
	}

	public enum ShotMode
	{
		SINGLE_SHOT,
		SHOTGUN_SPREAD,
		MACHINE_GUN_SPREAD
	}

	//Settings
	public FiringMode firingMode;
	public ShotMode shotMode;
	public bool startLoaded = false;
	public Vector3 defaultPosition;
	public Quaternion defaultRotation;
	public int magazineId;
	public int bulletId;
	public int damage = 10;
	public float bulletForce = 10;
	public List<string> shootLayers = new List<string>();
	public bool ignoreShootLayers;

	//References
	public VRGunTrigger trigger;
	public VRGunSlide slide;
	public BoxCollider clipReceiver;
	public VRMagazine integratedMagazine;
	public BoxCollider bulletReceiver;
	public VRInteractableItem secondHeld;

	//Prefabs
	public GameObject magazinePrefab;
	public GameObject bulletDecalPrefab;
	public GameObject muzzleFlashPrefab;
	public GameObject smokePrefab;
	public GameObject loadedBulletPrefab;
	public GameObject spentBulletPrefab;
	public Material laserPointerMat;

	//Variables
	public float fireRate = 0.1f; //Seconds between shots when automatic
	public int bulletsPerShot = 6;
	public float coneSize = 2.5f;
	public Vector3 shootOrigin;
	public Vector3 shootDirection;
	public Vector3 loadedBulletPosition;
	public Quaternion loadedBulletRotation;
	public Vector3 bulletEjectionDirection;
	public Vector3 firingOrigin;
	public Vector3 firingDirection;
	public Vector3 muzzleFlashPosition;
	public Quaternion muzzleFlashRotation;
	public Vector3 smokePosition;
	public Quaternion smokeRotation;
	public Vector3 laserPointerOrigin;
	public float lineWidth = 0.01f;

	//Sounds
	public AudioClip fireSound;
	public AudioClip dryFireSound;
	public AudioClip slidePulled;
	public AudioClip slideRelease;
	public AudioClip loadMagazine;
	public AudioClip unloadMagazine;

	//Instances
	private GameObject loadedBulletInstance;
	private VRMagazine magazine;
	private LineRenderer laserPointerLine;
	public VRMagazine currentMagazine
	{
		get { return magazine; }
		set 
		{
			magazine = value; 
			if (magazine != null) LoadBullet(false);
		}
	}

	private bool _usingSecondHeld;
	public bool usingSecondHeld
	{
		get { return _usingSecondHeld; }
		set 
		{ 
			_usingSecondHeld = value; 
			if (heldBy != null)
			{
				if (_usingSecondHeld) item.SetParent(heldBy.transform.parent);
				else item.SetParent(heldBy.transform);
			}
		}
	}

	private bool _hasBullet = false;
	public bool hasBullet
	{
		get { return _hasBullet; }
	}

	//Private variables
	private bool triggerHeld = false;
	private float elapsedTime;
	private float lastFired;

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
		if (startLoaded && magazinePrefab != null)
		{
			GameObject magazineInstance = (GameObject)Instantiate(magazinePrefab, Vector3.zero, Quaternion.identity);

			magazineInstance.transform.SetParent(item);
			magazineInstance.transform.localScale = magazinePrefab.transform.localScale;
			magazine = magazineInstance.GetComponentInChildren<VRMagazine>();
			if (magazine != null || magazine.item != null)
			{
				magazine.item.localPosition = magazine.defaultLoadedPosition;
				magazine.item.localRotation = magazine.defaultRotation;
				magazine.currentGun = this;
			} else
			{
				if (magazine != null && magazine.item == null)
					Debug.LogError("Magazine Script does not have a reference to item");
				else
					Debug.LogError("Weapon magazine prefab has no VRMagazine script attached");
			}
			if (loadedBulletPrefab != null)
			{
				loadedBulletInstance = (GameObject)Instantiate(loadedBulletPrefab, Vector3.zero, Quaternion.identity);
				loadedBulletInstance.transform.SetParent(item);
				loadedBulletInstance.transform.localPosition = loadedBulletPosition;
				loadedBulletInstance.transform.localRotation = loadedBulletRotation;
				VRInteractableItem loadedBulletItem = loadedBulletInstance.GetComponentInChildren<VRInteractableItem>();
				if (loadedBulletItem != null) loadedBulletItem.enabled = false;
				VRInteractableItem.FreezeItem(loadedBulletInstance, true);
			}
			_hasBullet = true;
		} else if (integratedMagazine != null)
		{
			integratedMagazine.currentGun = this;
			if (loadedBulletPrefab != null)
			{
				loadedBulletInstance = (GameObject)Instantiate(loadedBulletPrefab, Vector3.zero, Quaternion.identity);
				loadedBulletInstance.transform.SetParent(item);
				loadedBulletInstance.transform.localPosition = loadedBulletPosition;
				loadedBulletInstance.transform.localRotation = loadedBulletRotation;
				VRInteractableItem loadedBulletItem = loadedBulletInstance.GetComponentInChildren<VRInteractableItem>();
				if (loadedBulletItem != null) loadedBulletItem.enabled = false;
				VRInteractableItem.FreezeItem(loadedBulletInstance, true);
			}
			_hasBullet = true;
		} else if (startLoaded && magazinePrefab == null)
			Debug.LogError("Start loaded is true but no magazine prefab provided");

		if (clipReceiver != null)
			clipReceiver.isTrigger = true;
		base.Init();
	}

	override protected void Step()
	{
		if (heldBy == null || !canBeHeld) return;
		if (usingSecondHeld && secondHeld != null && secondHeld.heldBy != null)
		{
			Vector3 controllerDirection = secondHeld.heldBy.transform.position - heldBy.transform.position;
			Quaternion newRotation = Quaternion.LookRotation(controllerDirection, heldBy.transform.forward);
			item.rotation = newRotation;

			Vector3 dir = heldBy.transform.position - (heldBy.transform.position - heldPosition);
			dir = heldBy.transform.rotation * dir;
			dir = dir * heldBy.transform.lossyScale.x;
			item.position = heldBy.transform.position + dir;
		} else
			base.Step();

		if (laserPointerMat != null)
		{
			Ray ray = new Ray(item.TransformPoint(shootOrigin), item.TransformDirection(shootDirection));
			RaycastHit hit;
			bool hitSomething = false;
			if (shootLayers == null || shootLayers.Count == 0)
				hitSomething = Physics.Raycast(ray, out hit, 100);
			else
			{
				LayerMask raycastLayerMask = 1 << LayerMask.NameToLayer(shootLayers[0]);
				for (int i=1 ; i<shootLayers.Count ; i++)
				{
					raycastLayerMask |= 1 << LayerMask.NameToLayer(shootLayers[i]);
				}
				if (ignoreShootLayers) raycastLayerMask = ~raycastLayerMask;
				hitSomething = Physics.Raycast(ray, out hit, 100, raycastLayerMask);
			}
			if (laserPointerLine == null)
			{
				laserPointerLine = gameObject.AddComponent<LineRenderer>();
				laserPointerLine.material = laserPointerMat;
				laserPointerLine.SetWidth(lineWidth, lineWidth);
				laserPointerLine.SetVertexCount(2);
			}
			Vector3 endPoint = Vector3.zero;
			if (hitSomething)
			{
				endPoint = hit.point;
			} else
			{
				endPoint = ray.origin+(ray.direction*100);
			}
			laserPointerLine.SetPosition(0, item.TransformPoint(laserPointerOrigin));
			laserPointerLine.SetPosition(1, endPoint);
		}

		switch(firingMode)
		{
		case FiringMode.FULLY_AUTOMATIC:
			if (heldBy.ActionPressed("ACTION") && triggerHeld)
			{
				elapsedTime += Time.deltaTime;
				if (elapsedTime > lastFired)
				{
					lastFired = elapsedTime + fireRate;
					Shoot();
				}
			}
			break;
		}
	}

	override public void ActionPressed()
	{
		switch(firingMode)
		{
		case FiringMode.SEMI_AUTOMATIC:
		case FiringMode.PUMP_OR_BOLT_ACTION:
			Shoot();
			break;
		case FiringMode.FULLY_AUTOMATIC:
			triggerHeld = true;
			elapsedTime = 0;
			lastFired = 0;
			break;
		}
	}

	override public void ActionRelease()
	{
		switch(firingMode)
		{
		case FiringMode.FULLY_AUTOMATIC:
			triggerHeld = false;
			break;
		}
	}

	override public void Action2Pressed()
	{
		if (currentMagazine == null && heldBy != null)
			heldBy.Drop();
		else if (currentMagazine != null)
			currentMagazine.Eject();
	}

	override public bool Pickup(VRInteractor hand)
	{
		if (laserPointerLine != null) laserPointerLine.enabled = true;
		return base.Pickup(hand);
	}

	override public void Drop(SteamVR_TrackedObject trackedObj)
	{
		if (laserPointerLine != null) laserPointerLine.enabled = false;
		if (secondHeld != null && secondHeld.heldBy != null) secondHeld.heldBy.Drop();

		base.Drop(trackedObj);

		if (currentMagazine != null && hoverSlot == null && currentMagazine.hoverSlot != null)
		{
			currentMagazine.hoverSlot.AddItem(inventoryItem);
			hoverSlot = currentMagazine.hoverSlot;
			hoverSlot.hoverItem = inventoryItem;
			currentMagazine.hoverSlot = null;
		}
	}

	override public void Drop()
	{
		if (laserPointerLine != null) laserPointerLine.enabled = false;

		base.Drop();

		if (currentMagazine != null && hoverSlot == null && currentMagazine.hoverSlot != null)
		{
			currentMagazine.hoverSlot.AddItem(inventoryItem);
			hoverSlot = currentMagazine.hoverSlot;
			hoverSlot.hoverItem = inventoryItem;
			currentMagazine.hoverSlot = null;
		}
	}

	virtual public void Shoot()
	{
		if (!_hasBullet) 
		{
			if (dryFireSound != null)
			{
				PlaySound(dryFireSound);
			}
			return;
		}
		if (loadedBulletInstance != null) Destroy(loadedBulletInstance);
		_hasBullet = false;
		if (fireSound != null)
		{
			PlaySound(fireSound);
		}
		ActivateSmoke();
		ActivateFlash();
		EjectCasing();
		LoadBullet(false);

		if (slide != null) slide.Shoot(hasBullet);

		FireRaycast();
	}

	public void SlidePulled()
	{
		if (_hasBullet && loadedBulletInstance != null)
		{
			//Eject bullet
			loadedBulletInstance.transform.SetParent(null);
			VRInteractableItem loadedBulletItem = loadedBulletInstance.GetComponentInChildren<VRInteractableItem>();
			if (loadedBulletItem != null) loadedBulletItem.enabled = true;
			VRInteractableItem.UnFreezeItem(loadedBulletInstance);
			Rigidbody bulletBody = loadedBulletInstance.GetComponentInChildren<Rigidbody>();
			if (bulletBody != null) bulletBody.velocity = item.TransformVector(bulletEjectionDirection);
			loadedBulletInstance = null;
			_hasBullet = false;
		}
		LoadBullet(true);
	}

	virtual protected void LoadBullet(bool slidePulled)
	{
		if (hasBullet || (firingMode == FiringMode.PUMP_OR_BOLT_ACTION && !slidePulled)) return;
		if (integratedMagazine != null)
		{
			if (integratedMagazine.bulletCount <= 0) return;
			_hasBullet = integratedMagazine.TakeBullet();
		} else
		{
			if (magazine == null) return;
			if (magazine.bulletCount <= 0) return;
			_hasBullet = magazine.TakeBullet();
		}
		if (_hasBullet)
		{
			if (!slidePulled && slide != null && slide.pulled) slide.Release();
			if (loadedBulletPrefab != null)
			{
				loadedBulletInstance = (GameObject)Instantiate(loadedBulletPrefab, Vector3.zero, Quaternion.identity);
				loadedBulletInstance.transform.SetParent(item);
				loadedBulletInstance.transform.localPosition = loadedBulletPosition;
				loadedBulletInstance.transform.localRotation = loadedBulletRotation;
				VRInteractableItem loadedBulletItem = loadedBulletInstance.GetComponentInChildren<VRInteractableItem>();
				if (loadedBulletItem != null) loadedBulletItem.enabled = false;
				VRInteractableItem.FreezeItem(loadedBulletInstance, true);
			}
		}
	}

	virtual public bool LoadBullet(VRLoadableBullet bullet)
	{
		if (bullet == null || bulletId != bullet.bulletId) return false;
		if (hasBullet)
		{
			if (currentMagazine != null && !currentMagazine.integrated) currentMagazine.LoadBullet(bullet);
			return false;
		}

		if (bullet.heldBy != null) bullet.heldBy.Drop();
		_hasBullet = true;
		if (slide != null && slide.pulled) slide.Release();
		loadedBulletInstance = bullet.item.gameObject;
		loadedBulletInstance.transform.SetParent(item);
		loadedBulletInstance.transform.localPosition = loadedBulletPosition;
		loadedBulletInstance.transform.localRotation = loadedBulletRotation;
		VRInteractableItem loadedBulletItem = loadedBulletInstance.GetComponentInChildren<VRInteractableItem>();
		if (loadedBulletItem != null) loadedBulletItem.enabled = false;
		VRInteractableItem.FreezeItem(loadedBulletInstance, true);
		return true;
	}

	//Fire and forget. Smoke needs to deactivate itself when it is done
	virtual protected void ActivateSmoke()
	{
		if (smokePrefab == null) return;
		GameObject smokeInstance = (GameObject)Instantiate(smokePrefab, Vector3.zero, Quaternion.identity);
		smokeInstance.transform.SetParent(item);
		smokeInstance.transform.localPosition = smokePosition;
		smokeInstance.transform.localRotation = smokeRotation;
		smokeInstance.SetActive(true);
	}

	//Fire and forget. Flash needs to deactivate itself when it is done
	virtual protected void ActivateFlash()
	{
		if (muzzleFlashPrefab == null) return;
		GameObject muzzleFlashInstance = (GameObject)Instantiate(muzzleFlashPrefab, Vector3.zero, Quaternion.identity);
		muzzleFlashInstance.transform.SetParent(item);
		muzzleFlashInstance.transform.localPosition = muzzleFlashPosition;
		muzzleFlashInstance.transform.localRotation = muzzleFlashRotation;
		muzzleFlashInstance.SetActive(true);
	}

	virtual protected void EjectCasing()
	{
		if (spentBulletPrefab == null) return;
		GameObject casing = (GameObject)Instantiate(spentBulletPrefab, Vector3.zero, Quaternion.identity);
		casing.transform.SetParent(item);
		casing.transform.localPosition = loadedBulletPosition;
		casing.transform.localRotation = loadedBulletRotation;
		casing.transform.SetParent(null);
		VRInteractableItem.UnFreezeItem(casing);

		Rigidbody casingBody = casing.GetComponentInChildren<Rigidbody>();
		if (casingBody != null)
		{
			casingBody.velocity = item.TransformVector(bulletEjectionDirection);
		}
	}

	virtual protected void FireRaycast()
	{
		//If shoot direction is zero it has not been setup yet.
		if (shootDirection == Vector3.zero) 
		{
			Debug.LogWarning("Shoot direction not setup. Setup shoot direction in the gun handler editor");
			return;
		}

		List<Ray> rays = new List<Ray>();
		switch(shotMode)
		{
		case ShotMode.SINGLE_SHOT:
			rays.Add(new Ray(item.TransformPoint(shootOrigin), item.TransformDirection(shootDirection)));
			break;
		case ShotMode.SHOTGUN_SPREAD:
			{
				Vector3 direction = item.TransformDirection(shootDirection);
				Vector3 originPosition = item.TransformPoint(shootOrigin);
				for(int i=0; i<bulletsPerShot; i++)
				{
					rays.Add(new Ray(originPosition, VRUtils.GetConeDirection(direction, coneSize)));
				}
			}
			break;
		case ShotMode.MACHINE_GUN_SPREAD:
			{
				Vector3 direction = item.TransformDirection(shootDirection);
				Vector3 originPosition = item.TransformPoint(shootOrigin);
				rays.Add(new Ray(originPosition, VRUtils.GetConeDirection(direction, coneSize)));
			}
			break;
		}

		foreach(Ray ray in rays)
		{
			RaycastHit hit;
			bool hitSomething = false;
			if (shootLayers == null || shootLayers.Count == 0)
				hitSomething = Physics.Raycast(ray, out hit, 100);
			else
			{
				LayerMask raycastLayerMask = 1 << LayerMask.NameToLayer(shootLayers[0]);
				for (int i=1 ; i<shootLayers.Count ; i++)
				{
					raycastLayerMask |= 1 << LayerMask.NameToLayer(shootLayers[i]);
				}
				if (ignoreShootLayers) raycastLayerMask = ~raycastLayerMask;
				hitSomething = Physics.Raycast(ray, out hit, 100, raycastLayerMask);
			}
			Debug.DrawRay(ray.origin, ray.direction*10, Color.red, 5);
			if (hitSomething)
			{
				Rigidbody hitBody = hit.transform.GetComponentInChildren<Rigidbody>();
				if (hitBody != null)
				{
					hitBody.AddForce(ray.direction*bulletForce*100);
				}

				if (bulletDecalPrefab != null)
				{
					//Draw decal on surface hit
					GameObject decal = (GameObject)Instantiate(bulletDecalPrefab, Vector3.zero, Quaternion.identity);

					DecalChanger decalChanger = decal.GetComponentInChildren<DecalChanger>();
					if (decalChanger != null) decalChanger.SetMaterialTo(hit.transform.tag); 

					Vector3 oldScale = decal.transform.localScale;
					decal.transform.parent = hit.transform;
					decal.transform.position = hit.point+(hit.normal*0.005f);
					decal.transform.LookAt(hit.point-hit.normal);
					if (hit.transform.lossyScale != Vector3.one)
					{
						decal.transform.parent = null;
						decal.transform.localScale = oldScale;
						decal.transform.parent = hit.transform;
					}
				}
				//Send damage info to hit object (Check ExampleEnemy.cs for reference)
				DamageInfo damageInfo = new DamageInfo();
				damageInfo.damage = damage;
				damageInfo.weapon = this;
				damageInfo.hitInfo = hit;
				hit.transform.SendMessageUpwards("Damage", damageInfo, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	override public void EnableHover()
	{
		base.EnableHover();
		if (slide != null) slide.EnableHover();
		if (currentMagazine != null) currentMagazine.EnableHover();
	}

	override public void DisableHover()
	{
		base.DisableHover();
		if (slide != null) slide.DisableHover();
		if (currentMagazine != null) currentMagazine.DisableHover();
	}
}
