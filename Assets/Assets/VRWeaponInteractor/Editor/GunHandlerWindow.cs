using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class GunHandlerWindow : EditorWindow 
{
	public enum WeaponTab
	{
		MAIN,
		TRIGGER,
		SLIDE,
		MAGAZINE
	}
	public WeaponTab weaponTab = WeaponTab.MAIN;

	//	Main
	public VRGunHandler gunHandler;
	public SerializedObject serializedGunHandler;
	static bool gunPositionFoldout = false;

	static bool raycastLayerFoldout = false;

	GameObject muzzleFlashInstance;
	static bool muzzleFoldout = false;
	GameObject smokeInstance;
	static bool smokeFoldout = false;
	GameObject bulletInstance;
	static bool bulletFoldout = false;

	GameObject shootOriginInstance;
	GameObject shootDirectionInstance;

	GameObject ejectionOriginInstance;
	GameObject ejectionDestinationInstance;

	GameObject laserPointerOriginInstance;

	//	Trigger
	public VRGunTrigger gunTrigger;
	public SerializedObject serializedTrigger;

	//	Slide
	public VRGunSlide gunSlide;
	public SerializedObject serializedSlide;

	//	Magazine
	public VRMagazine magazine;
	public SerializedObject serializedMagazine;
	GameObject magazineInstance;
	bool magazineLoaded;
	List<GameObject> bulletInstances = new List<GameObject>();
	static bool magBulletFoldout = false;
	bool triggerPulled = false;
	bool slidePulled = false;

	public void Init()
	{
		if (gunHandler == null) return;
		serializedGunHandler = new SerializedObject(gunHandler);
		gunTrigger = gunHandler.trigger;
		if (gunTrigger != null) serializedTrigger = new SerializedObject(gunTrigger);
		gunSlide = gunHandler.slide;
		if (gunSlide != null) serializedSlide = new SerializedObject(gunSlide);
	}

	void OnGUI () 
	{
		if (gunHandler == null || serializedGunHandler == null)
		{
			GUILayout.Label("No Gun Handler Referenced");
			gunHandler = (VRGunHandler)EditorGUILayout.ObjectField("Gun Handler", gunHandler, typeof(VRGunHandler), true);
			if (gunHandler != null)
			{
				Init();
				weaponTab = WeaponTab.MAIN;
			} else return;
		}
		GUILayout.BeginHorizontal();
		switch(weaponTab)
		{
		case WeaponTab.MAIN:
			GUILayout.Box("Main", GUILayout.ExpandWidth(true));
			if (GUILayout.Button("Slide"))
				weaponTab = WeaponTab.SLIDE;
			if (GUILayout.Button("Trigger"))
				weaponTab = WeaponTab.TRIGGER;
			if (GUILayout.Button("Magazine"))
				weaponTab = WeaponTab.MAGAZINE;
			break;
		case WeaponTab.TRIGGER:
			if (GUILayout.Button("Main"))
				weaponTab = WeaponTab.MAIN;
			if (GUILayout.Button("Slide"))
				weaponTab = WeaponTab.SLIDE;
			GUILayout.Box("Trigger", GUILayout.ExpandWidth(true));
			if (GUILayout.Button("Magazine"))
				weaponTab = WeaponTab.MAGAZINE;
			break;
		case WeaponTab.SLIDE:
			if (GUILayout.Button("Main"))
				weaponTab = WeaponTab.MAIN;
			GUILayout.Box("Slide", GUILayout.ExpandWidth(true));
			if (GUILayout.Button("Trigger"))
				weaponTab = WeaponTab.TRIGGER;
			if (GUILayout.Button("Magazine"))
				weaponTab = WeaponTab.MAGAZINE;
			break;
		case WeaponTab.MAGAZINE:
			if (GUILayout.Button("Main"))
				weaponTab = WeaponTab.MAIN;
			if (GUILayout.Button("Slide"))
				weaponTab = WeaponTab.SLIDE;
			if (GUILayout.Button("Trigger"))
				weaponTab = WeaponTab.TRIGGER;
			GUILayout.Box("Magazine", GUILayout.ExpandWidth(true));
			break;
		}
		GUILayout.EndHorizontal();
		switch(weaponTab)
		{
		case WeaponTab.MAIN:
			EditorGUI.indentLevel++;
			GUILayout.Label("Main", EditorStyles.boldLabel);
			EditorGUI.indentLevel--;
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			serializedGunHandler.Update();
			if (serializedMagazine != null) serializedMagazine.Update();
			MainSection();
			PrefabSection();
			SoundsSection();
			EditorGUILayout.EndScrollView();
			serializedGunHandler.ApplyModifiedProperties();
			if (serializedMagazine != null) serializedMagazine.ApplyModifiedProperties();
			break;
		case WeaponTab.SLIDE:
			SlideSection();
			break;
		case WeaponTab.TRIGGER:
			TriggerSection();
			break;
		case WeaponTab.MAGAZINE:
			MagazineSection();
			break;
		}
	}

	private void MainSection()
	{
		var oldGunHandler = gunHandler;
		gunHandler = (VRGunHandler)EditorGUILayout.ObjectField("Gun Handler", gunHandler, typeof(VRGunHandler), true);
		if (gunHandler == null)
			return;
		if (oldGunHandler != gunHandler)
		{
			Init();
		}
		gunPositionFoldout = EditorGUILayout.Foldout(gunPositionFoldout, "Weapon Position");
		if (gunPositionFoldout)
		{
			EditorGUI.indentLevel++;
			SerializedProperty defaultPosition = serializedGunHandler.FindProperty("defaultPosition");
			defaultPosition.vector3Value = EditorGUILayout.Vector3Field("Default Position", defaultPosition.vector3Value);
			SerializedProperty defaultRotation = serializedGunHandler.FindProperty("defaultRotation");
			Quaternion tempDefaultRotation = defaultRotation.quaternionValue;
			tempDefaultRotation.eulerAngles = EditorGUILayout.Vector3Field("Default Rotation", tempDefaultRotation.eulerAngles);
			defaultRotation.quaternionValue = tempDefaultRotation;
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Set Current To Default"))
			{
				defaultPosition.vector3Value = gunHandler.transform.localPosition;
				defaultRotation.quaternionValue = gunHandler.transform.localRotation;
			}
			if (GUILayout.Button("Move To Default"))
			{
				Undo.RecordObject(gunHandler.transform, "Move To Default");
				gunHandler.transform.localPosition = defaultPosition.vector3Value;
				gunHandler.transform.localRotation = defaultRotation.quaternionValue;
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.HelpBox("The gun meshes position relative to the gun model parent. Can usually be left at the default", MessageType.Info);
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUI.indentLevel--;

		}
		if (GUILayout.Button("Setup Weapon Held Position"))
		{
			HeldPositionWindow newWindow = (HeldPositionWindow)EditorWindow.GetWindow(typeof(HeldPositionWindow), true, "Held Position", true);
			newWindow.interactableItem = gunHandler;
			newWindow.Init();
		}


		SerializedProperty secondHeldCollider = serializedGunHandler.FindProperty("secondHeld");
		secondHeldCollider.objectReferenceValue = EditorGUILayout.ObjectField("Second Held Collider", secondHeldCollider.objectReferenceValue, typeof(VRSecondHeld), true);

		if (secondHeldCollider.objectReferenceValue == null)
		{
			if (GUILayout.Button("Add Second Held Collider"))
			{
				GameObject secondHeldObject = new GameObject("Second Held Collider");
				secondHeldObject.transform.parent = gunHandler.item;
				secondHeldObject.transform.localPosition = Vector3.zero;
				Undo.RegisterCreatedObjectUndo(secondHeldObject, "Create second held collider");
				BoxCollider newCollider = secondHeldObject.AddComponent<BoxCollider>();
				newCollider.isTrigger = true;
				VRSecondHeld secondHeld = secondHeldObject.AddComponent<VRSecondHeld>();
				secondHeld.item = secondHeldObject.transform;
				secondHeld.parentItem = gunHandler;
				secondHeldCollider.objectReferenceValue = secondHeld;
			}
		}
		EditorGUILayout.HelpBox("The second held collider can be used on a rifle to hold with both hands and improve accuracy.", MessageType.Info);

		EditorGUI.indentLevel++;
		GUILayout.Label("Weapon Firing", EditorStyles.boldLabel);
		EditorGUI.indentLevel--;

		SerializedProperty fireModeEnum = serializedGunHandler.FindProperty("firingMode");
		fireModeEnum.intValue = (int)(VRGunHandler.FiringMode)EditorGUILayout.EnumPopup("Firing Mode", (VRGunHandler.FiringMode)fireModeEnum.intValue);
		VRGunHandler.FiringMode fireMode = (VRGunHandler.FiringMode)fireModeEnum.intValue;
		switch(fireMode)
		{
		case VRGunHandler.FiringMode.SEMI_AUTOMATIC:
			EditorGUILayout.HelpBox("Single fire, prepares next bullet if available. Weapon example: Pistol or Rifle", MessageType.Info);
			break;
		case VRGunHandler.FiringMode.FULLY_AUTOMATIC:
			EditorGUILayout.HelpBox("Fire bullets until empty for as long as the trigger is held. Weapon example: Assault Rifle", MessageType.Info);
			SerializedProperty fireRate = serializedGunHandler.FindProperty("fireRate");
			fireRate.floatValue = EditorGUILayout.FloatField("Fire Rate", fireRate.floatValue);
			break;
		case VRGunHandler.FiringMode.PUMP_OR_BOLT_ACTION:
			EditorGUILayout.HelpBox("Slide needs to be pulled after each shot to load the next bullet/shell. Weapon example: Shotgun or Rifle", MessageType.Info);
			break;
		}

		SerializedProperty damageAmount = serializedGunHandler.FindProperty("damage");
		damageAmount.intValue = EditorGUILayout.IntField("Damage", damageAmount.intValue);

		SerializedProperty bulletForce = serializedGunHandler.FindProperty("bulletForce");
		bulletForce.floatValue = EditorGUILayout.FloatField("Bullet Force", bulletForce.floatValue);

		SerializedProperty shootOrigin = serializedGunHandler.FindProperty("shootOrigin");
		SerializedProperty shootDirection = serializedGunHandler.FindProperty("shootDirection");
		shootOrigin.vector3Value = EditorGUILayout.Vector3Field("Shoot Origin", shootOrigin.vector3Value);
		shootDirection.vector3Value = EditorGUILayout.Vector3Field("Shoot Direction", shootDirection.vector3Value);

		if (shootOriginInstance == null || shootDirectionInstance == null)
		{
			if (GUILayout.Button("Setup Shoot Direction"))
			{
				shootOriginInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				shootOriginInstance.name = "Shoot Origin";
				Undo.RegisterCreatedObjectUndo(shootOriginInstance, "Setup Shoot Direction");
				shootOriginInstance.transform.SetParent(gunHandler.item);
				shootOriginInstance.transform.localPosition = shootOrigin.vector3Value;
				shootOriginInstance.transform.localScale *= gunHandler.item.localScale.magnitude*0.01f;
				shootDirectionInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				shootDirectionInstance.name = "Shoot Direction";
				Undo.RegisterCreatedObjectUndo(shootDirectionInstance, "Setup Shoot Direction");
				shootDirectionInstance.transform.SetParent(gunHandler.item);
				shootDirectionInstance.transform.localPosition = shootOrigin.vector3Value + (shootDirection.vector3Value*0.1f);
				shootDirectionInstance.transform.localScale = shootOriginInstance.transform.localScale;
				Selection.activeGameObject = shootDirectionInstance;
			}
		} else
		{

			SerializedProperty shotModeEnum = serializedGunHandler.FindProperty("shotMode");
			shotModeEnum.intValue = (int)(VRGunHandler.ShotMode)EditorGUILayout.EnumPopup("Shot Mode", (VRGunHandler.ShotMode)shotModeEnum.intValue);
			VRGunHandler.ShotMode shotMode = (VRGunHandler.ShotMode)shotModeEnum.intValue;
			switch(shotMode)
			{
			case VRGunHandler.ShotMode.SINGLE_SHOT:
				break;
			case VRGunHandler.ShotMode.SHOTGUN_SPREAD:
				{
					SerializedProperty bulletsPerShot = serializedGunHandler.FindProperty("bulletsPerShot");
					bulletsPerShot.intValue = EditorGUILayout.IntField("Bullets Per Shot", bulletsPerShot.intValue);
					SerializedProperty coneSize = serializedGunHandler.FindProperty("coneSize");
					coneSize.floatValue = EditorGUILayout.FloatField("Cone Size", coneSize.floatValue);

					if (GUILayout.Button("Show Test Ray"))
					{
						Vector3 direction = (shootDirectionInstance.transform.position - shootOriginInstance.transform.position)*10;
						for(int i=0; i<bulletsPerShot.intValue; i++) Debug.DrawRay(shootOriginInstance.transform.position, VRUtils.GetConeDirection(direction, coneSize.floatValue));
						SceneView.RepaintAll();
					}
				}
				break;
			case VRGunHandler.ShotMode.MACHINE_GUN_SPREAD:
				{
					SerializedProperty coneSize = serializedGunHandler.FindProperty("coneSize");
					coneSize.floatValue = EditorGUILayout.FloatField("Cone Size", coneSize.floatValue);

					if (GUILayout.Button("Show Test Ray"))
					{
						Vector3 direction = (shootDirectionInstance.transform.position - shootOriginInstance.transform.position)*10;
						Debug.DrawRay(shootOriginInstance.transform.position, VRUtils.GetConeDirection(direction, coneSize.floatValue));
						SceneView.RepaintAll();
					}
				}
				break;
			}
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Save"))
			{
				shootOrigin.vector3Value = shootOriginInstance.transform.localPosition;
				shootDirection.vector3Value = (shootDirectionInstance.transform.localPosition - shootOriginInstance.transform.localPosition)*10;
				Undo.DestroyObjectImmediate(shootOriginInstance);
				Undo.DestroyObjectImmediate(shootDirectionInstance);
			}
			if (GUILayout.Button("Select Origin"))
			{
				Selection.activeGameObject = shootOriginInstance;
			}
			if (GUILayout.Button("Select Destination"))
			{
				Selection.activeGameObject = shootDirectionInstance;
			}
			if (GUILayout.Button("Cancel"))
			{
				Undo.DestroyObjectImmediate(shootOriginInstance);
				Undo.DestroyObjectImmediate(shootDirectionInstance);
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.HelpBox("Move the origin sphere to where the bullet will fire from, in a pistol this would be the end of the gun shaft. Then move the destination" +
				" sphere further along in the direction the bullet should go.", MessageType.Info);
		}

		raycastLayerFoldout = EditorGUILayout.Foldout(raycastLayerFoldout, "Damage Raycast Layers");
		if (raycastLayerFoldout)
		{
			EditorGUI.indentLevel++;
			SerializedProperty raycastLayers = serializedGunHandler.FindProperty("shootLayers");
			SerializedProperty ignoreRaycast = serializedGunHandler.FindProperty("ignoreShootLayers");
			raycastLayers.arraySize = EditorGUILayout.IntField("Size", raycastLayers.arraySize);
			for(int i=0 ; i<raycastLayers.arraySize ; i++)
			{
				SerializedProperty raycastLayer = raycastLayers.GetArrayElementAtIndex(i);
				raycastLayer.stringValue = EditorGUILayout.TextField("Element "+i, raycastLayer.stringValue);
			}
			EditorGUILayout.HelpBox("Leave raycast layers empty to collide with everything", MessageType.Info);
			if (raycastLayers.arraySize > 0)
			{
				ignoreRaycast.boolValue = EditorGUILayout.Toggle("Ignore raycast layers", ignoreRaycast.boolValue);
				EditorGUILayout.HelpBox("Ignore raycast layers True: Ignore anything on the layers specified. False: Ignore anything on layers not specified", MessageType.Info);
			}
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUI.indentLevel--;
		}

		EditorGUI.indentLevel++;
		GUILayout.Label("Hover", EditorStyles.boldLabel);
		EditorGUI.indentLevel--;

		SerializedProperty hover = serializedGunHandler.FindProperty("hover");
		hover.objectReferenceValue = EditorGUILayout.ObjectField("Hover", hover.objectReferenceValue, typeof(Renderer), true);

		SerializedProperty hoverMode = serializedGunHandler.FindProperty("hoverMode");
		hoverMode.intValue = (int)(VRInteractableItem.HoverMode)EditorGUILayout.EnumPopup("Hover Mode", (VRInteractableItem.HoverMode)hoverMode.intValue);
		VRInteractableItem.HoverMode hoverModeEnum = (VRInteractableItem.HoverMode)hoverMode.intValue;
		switch(hoverModeEnum)
		{
		case VRInteractableItem.HoverMode.SHADER:
			EditorGUILayout.HelpBox("Leave null to use current materials shader", MessageType.Info);
			SerializedProperty defaultShader = serializedGunHandler.FindProperty("defaultShader");
			defaultShader.objectReferenceValue = EditorGUILayout.ObjectField("Default Shader", defaultShader.objectReferenceValue, typeof(Shader), false);
			EditorGUILayout.HelpBox("Hover Default is Unlit/Texture", MessageType.Info);
			SerializedProperty hoverShader = serializedGunHandler.FindProperty("hoverShader");
			hoverShader.objectReferenceValue = EditorGUILayout.ObjectField("Hover Shader", hoverShader.objectReferenceValue, typeof(Shader), false);
			break;
		case VRInteractableItem.HoverMode.MATERIAL:
			EditorGUILayout.HelpBox("Leave null to use current material", MessageType.Info);
			SerializedProperty defaultMat = serializedGunHandler.FindProperty("defaultMat");
			defaultMat.objectReferenceValue = EditorGUILayout.ObjectField("Default Material", defaultMat.objectReferenceValue, typeof(Material), false);
			EditorGUILayout.HelpBox("Hover Default is Unlit/Texture", MessageType.Info);
			SerializedProperty hoverMat = serializedGunHandler.FindProperty("hoverMat");
			hoverMat.objectReferenceValue = EditorGUILayout.ObjectField("Hover Material", hoverMat.objectReferenceValue, typeof(Material), false);
			break;
		}

		SerializedProperty enterHover = serializedGunHandler.FindProperty("enterHover");
		enterHover.objectReferenceValue = EditorGUILayout.ObjectField("Enter Hover Sound", enterHover.objectReferenceValue, typeof(AudioClip), false);
		SerializedProperty exitHover = serializedGunHandler.FindProperty("exitHover");
		exitHover.objectReferenceValue = EditorGUILayout.ObjectField("Exit Hover Sound", exitHover.objectReferenceValue, typeof(AudioClip), false);
	}

	private void PrefabSection()
	{
		EditorGUI.indentLevel++;
		GUILayout.Label("Prefabs", EditorStyles.boldLabel);
		EditorGUI.indentLevel--;

		SerializedProperty bulletDecalPrefab = serializedGunHandler.FindProperty("bulletDecalPrefab");
		bulletDecalPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Bullet Decal Prefab", bulletDecalPrefab.objectReferenceValue, typeof(GameObject), false);

		SerializedProperty muzzleFlashPrefab = serializedGunHandler.FindProperty("muzzleFlashPrefab");
		muzzleFlashPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Muzzle Flash Prefab", muzzleFlashPrefab.objectReferenceValue, typeof(GameObject), false);
		if (muzzleFlashPrefab.objectReferenceValue != null)
		{
			muzzleFoldout = EditorGUILayout.Foldout(muzzleFoldout, "Muzzle Flash Settings");
			if (muzzleFoldout)
			{
				ReferencePositionConfig(serializedGunHandler, "muzzleFlashPosition", "muzzleFlashRotation", (GameObject)muzzleFlashPrefab.objectReferenceValue, ref muzzleFlashInstance);
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			}
		}
		SerializedProperty smokePrefab = serializedGunHandler.FindProperty("smokePrefab");
		smokePrefab.objectReferenceValue = EditorGUILayout.ObjectField("Smoke Prefab", smokePrefab.objectReferenceValue, typeof(GameObject), false);
		if (smokePrefab.objectReferenceValue != null)
		{
			smokeFoldout = EditorGUILayout.Foldout(smokeFoldout, "Smoke Settings");
			if (smokeFoldout)
			{
				ReferencePositionConfig(serializedGunHandler, "smokePosition", "smokeRotation", (GameObject)smokePrefab.objectReferenceValue, ref smokeInstance);
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			}
		}

		EditorGUI.indentLevel++;
		GUILayout.Label("Bullets", EditorStyles.boldLabel);
		EditorGUI.indentLevel--;

		SerializedProperty loadedBullet = serializedGunHandler.FindProperty("loadedBulletPrefab");
		var oldLoadedBullet = loadedBullet.objectReferenceValue;
		loadedBullet.objectReferenceValue = EditorGUILayout.ObjectField("Bullet Prefab", loadedBullet.objectReferenceValue, typeof(GameObject), false);
		SerializedProperty spentBullet = serializedGunHandler.FindProperty("spentBulletPrefab");
		spentBullet.objectReferenceValue = EditorGUILayout.ObjectField("Spent Bullet Prefab", spentBullet.objectReferenceValue, typeof(GameObject), false);

		SerializedProperty integratedMagazine = serializedGunHandler.FindProperty("integratedMagazine");
		if (oldLoadedBullet != loadedBullet.objectReferenceValue)
		{
			if (integratedMagazine.objectReferenceValue != null)
			{
				if (serializedMagazine == null)
				{
					serializedMagazine = new SerializedObject(integratedMagazine.objectReferenceValue);
					serializedMagazine.Update();
				}
				SerializedProperty magBullet = serializedMagazine.FindProperty("bulletPrefab");
				magBullet.objectReferenceValue = loadedBullet.objectReferenceValue;
			}
		}

		if (loadedBullet.objectReferenceValue != null)
		{
			bulletFoldout = EditorGUILayout.Foldout(bulletFoldout, "Bullet Settings");
			if (bulletFoldout)
			{
				ReferencePositionConfig(serializedGunHandler, "loadedBulletPosition", "loadedBulletRotation", (GameObject)loadedBullet.objectReferenceValue, ref bulletInstance);
				EditorGUILayout.HelpBox("The bullet position is where the bullet prefab will sit when loaded.", MessageType.Info);
				EditorGUI.indentLevel++;
				SerializedProperty bulletPosition = serializedGunHandler.FindProperty("loadedBulletPosition");
				SerializedProperty ejectDirection = serializedGunHandler.FindProperty("bulletEjectionDirection");
				ejectDirection.vector3Value = EditorGUILayout.Vector3Field("Bullet Eject Direction", ejectDirection.vector3Value);
				if (ejectionOriginInstance == null || ejectionDestinationInstance == null)
				{
					if (GUILayout.Button("Setup Ejection Direction"))
					{
						ejectionOriginInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
						ejectionOriginInstance.name = "Bullet Origin";
						Undo.RegisterCreatedObjectUndo(ejectionOriginInstance, "Setup Ejection");
						ejectionOriginInstance.transform.SetParent(gunHandler.item);
						ejectionOriginInstance.transform.localPosition = bulletPosition.vector3Value;
						ejectionOriginInstance.transform.localScale *= gunHandler.item.localScale.magnitude*0.01f;
						ejectionDestinationInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
						ejectionDestinationInstance.name = "Bullet Direction";
						Undo.RegisterCreatedObjectUndo(ejectionDestinationInstance, "Setup Ejection");
						ejectionDestinationInstance.transform.SetParent(gunHandler.item);
						ejectionDestinationInstance.transform.localPosition = bulletPosition.vector3Value + (ejectDirection.vector3Value*0.1f);
						ejectionDestinationInstance.transform.localScale = ejectionOriginInstance.transform.localScale;
						Selection.activeGameObject = ejectionDestinationInstance;
					}
				} else
				{
					EditorGUILayout.HelpBox("If you need to move the origin position cancel this, change the bullet loaded position, then come back and set the ejection direction", MessageType.Info);
					if (GUILayout.Button("Save"))
					{
						ejectDirection.vector3Value = (ejectionDestinationInstance.transform.localPosition - ejectionOriginInstance.transform.localPosition)*10;
						Undo.DestroyObjectImmediate(ejectionOriginInstance);
						Undo.DestroyObjectImmediate(ejectionDestinationInstance);
					}
					if (GUILayout.Button("Select Destination"))
					{
						Selection.activeGameObject = ejectionDestinationInstance;
					}
					if (GUILayout.Button("Cancel"))
					{
						Undo.DestroyObjectImmediate(ejectionOriginInstance);
						Undo.DestroyObjectImmediate(ejectionDestinationInstance);
					}
					EditorGUILayout.HelpBox("The ejection origin will always be the bullet position. Drag the destination sphere in the direction the bullet should eject towards." +
						"The further away you move it the more force will be applied when ejecting.", MessageType.Info);
				}
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				EditorGUI.indentLevel--;
			}
		}

		EditorGUILayout.HelpBox("The bullet receiver is a collider that allows you to load a bullet directly into the gun." +
			"It should be placed around where the bullet will sit in the gun", MessageType.Info);

		SerializedProperty bulletReceiver = null;
		SerializedProperty magBulletId = null;
		SerializedProperty bulletId = serializedGunHandler.FindProperty("bulletId");
		if (integratedMagazine.objectReferenceValue != null)
		{
			if (serializedMagazine == null)
			{
				serializedMagazine = new SerializedObject(integratedMagazine.objectReferenceValue);
				serializedMagazine.Update();
			}

			bulletReceiver = serializedMagazine.FindProperty("bulletReceiver");
			magBulletId = serializedMagazine.FindProperty("bulletId");
		} else
			bulletReceiver = serializedGunHandler.FindProperty("bulletReceiver");

		bulletReceiver.objectReferenceValue = EditorGUILayout.ObjectField("Bullet Receiver", bulletReceiver.objectReferenceValue, typeof(BoxCollider), true);
		if (bulletReceiver.objectReferenceValue == null)
		{
			if (GUILayout.Button("Create Bullet Receiver"))
			{
				GameObject bulletEntryColliderObj = new GameObject("BulletReceiver");
				Undo.RegisterCreatedObjectUndo(bulletEntryColliderObj, "Create Bullet Receiver");
				bulletEntryColliderObj.transform.SetParent(gunHandler.item);
				bulletEntryColliderObj.transform.localPosition = Vector3.zero;
				bulletEntryColliderObj.transform.localRotation = Quaternion.identity;
				bulletReceiver.objectReferenceValue = bulletEntryColliderObj.AddComponent<BoxCollider>();
				((BoxCollider)bulletReceiver.objectReferenceValue).size = new Vector3(0.05f*gunHandler.item.localScale.magnitude, 0.001f*gunHandler.item.localScale.magnitude, 0.05f*gunHandler.item.localScale.magnitude);
				((BoxCollider)bulletReceiver.objectReferenceValue).isTrigger = true;
				VRBulletReceiver bulletReceiverScript = bulletEntryColliderObj.AddComponent<VRBulletReceiver>();
				if (integratedMagazine.objectReferenceValue != null)
					bulletReceiverScript.magazine = (VRMagazine)integratedMagazine.objectReferenceValue;
				else
					bulletReceiverScript.gunHandler = gunHandler;
				Selection.activeGameObject = bulletEntryColliderObj;
			}
		} else
		{
			EditorGUILayout.HelpBox("When a bullet matching the bullet id touches the collider it will attempt to load", MessageType.Info);
			var oldBulletId = bulletId.intValue;
			bulletId.intValue = EditorGUILayout.IntField("Bullet Id", bulletId.intValue);
			if (oldBulletId != bulletId.intValue && magBulletId != null)
				magBulletId.intValue = bulletId.intValue;
			if (GUILayout.Button("Select Bullet Receiver"))
			{
				Selection.activeGameObject = ((BoxCollider)bulletReceiver.objectReferenceValue).gameObject;
			}
		}

		SerializedProperty laserPointMat = serializedGunHandler.FindProperty("laserPointerMat");
		laserPointMat.objectReferenceValue = EditorGUILayout.ObjectField("Laser Pointer Material", laserPointMat.objectReferenceValue, typeof(Material), false);
		if (laserPointMat.objectReferenceValue != null)
		{
			SerializedProperty lineWidth = serializedGunHandler.FindProperty("lineWidth");
			lineWidth.floatValue = EditorGUILayout.FloatField("Line Width", lineWidth.floatValue);
			SerializedProperty laserPointerOrigin = serializedGunHandler.FindProperty("laserPointerOrigin");
			laserPointerOrigin.vector3Value = EditorGUILayout.Vector3Field("Laser Pointer Origin", laserPointerOrigin.vector3Value);
			if (laserPointerOriginInstance == null)
			{
				if (GUILayout.Button("Setup Laser Origin Position"))
				{
					laserPointerOriginInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					laserPointerOriginInstance.name = "Laser Pointer Origin";
					Undo.RegisterCreatedObjectUndo(laserPointerOriginInstance, "Setup Laser Pointer");
					laserPointerOriginInstance.transform.SetParent(gunHandler.item);
					laserPointerOriginInstance.transform.localPosition = laserPointerOrigin.vector3Value;
					laserPointerOriginInstance.transform.localScale *= gunHandler.item.localScale.magnitude*0.01f;
					Selection.activeGameObject = laserPointerOriginInstance;
				}
			} else
			{
				if (GUILayout.Button("Save"))
				{
					laserPointerOrigin.vector3Value = laserPointerOriginInstance.transform.localPosition;
					Undo.DestroyObjectImmediate(laserPointerOriginInstance);
				}
				if (GUILayout.Button("Select Destination"))
				{
					Selection.activeGameObject = laserPointerOriginInstance;
				}
				if (GUILayout.Button("Cancel"))
				{
					Undo.DestroyObjectImmediate(laserPointerOriginInstance);
				}
			}
		}
	}

	private void SoundsSection()
	{
		EditorGUI.indentLevel++;
		GUILayout.Label("Sounds", EditorStyles.boldLabel);
		EditorGUI.indentLevel--;

		SerializedProperty fireSound = serializedGunHandler.FindProperty("fireSound");
		fireSound.objectReferenceValue = EditorGUILayout.ObjectField("Fire", fireSound.objectReferenceValue, typeof(AudioClip), false);
		SerializedProperty dryFireSound = serializedGunHandler.FindProperty("dryFireSound");
		dryFireSound.objectReferenceValue = EditorGUILayout.ObjectField("Dry Fire", dryFireSound.objectReferenceValue, typeof(AudioClip), false);

		if (gunSlide != null)
		{
			SerializedProperty slidePulled = serializedGunHandler.FindProperty("slidePulled");
			slidePulled.objectReferenceValue = EditorGUILayout.ObjectField("Slide Pulled", slidePulled.objectReferenceValue, typeof(AudioClip), false);
			SerializedProperty slideReleased = serializedGunHandler.FindProperty("slideRelease");
			slideReleased.objectReferenceValue = EditorGUILayout.ObjectField("Slide Released", slideReleased.objectReferenceValue, typeof(AudioClip), false);
		}

		SerializedProperty integratedMagazine = serializedGunHandler.FindProperty("integratedMagazine");
		if (integratedMagazine.objectReferenceValue == null)
		{
			SerializedProperty loadMagazine = serializedGunHandler.FindProperty("loadMagazine");
			loadMagazine.objectReferenceValue = EditorGUILayout.ObjectField("Load Magazine", loadMagazine.objectReferenceValue, typeof(AudioClip), false);
			SerializedProperty unloadMagazine = serializedGunHandler.FindProperty("unloadMagazine");
			unloadMagazine.objectReferenceValue = EditorGUILayout.ObjectField("Unload Magazine", unloadMagazine.objectReferenceValue, typeof(AudioClip), false);
		}
	}

	void TriggerSection()
	{
		EditorGUI.indentLevel++;
		GUILayout.Label("Trigger", EditorStyles.boldLabel);
		EditorGUI.indentLevel--;
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		if (serializedTrigger != null)
			serializedTrigger.Update();
		var oldGunTrigger = gunTrigger;
		gunTrigger = (VRGunTrigger)EditorGUILayout.ObjectField("Gun Trigger", gunTrigger, typeof(VRGunTrigger), true);
		if (gunTrigger == null)
		{
			EditorGUILayout.EndScrollView();
			return;
		}
		if (oldGunTrigger != gunTrigger)
		{
			if (serializedGunHandler == null) serializedGunHandler = new SerializedObject(gunHandler);
			serializedGunHandler.Update();
			SerializedProperty gunHandlerTrigger = serializedGunHandler.FindProperty("trigger");
			gunHandlerTrigger.objectReferenceValue = gunTrigger;
			serializedGunHandler.ApplyModifiedProperties();
			serializedTrigger = new SerializedObject(gunTrigger);
			SerializedProperty slideGunHandler = serializedTrigger.FindProperty("gunHandler");
			slideGunHandler.objectReferenceValue = gunHandler;
			weaponTab = WeaponTab.TRIGGER;
		}
		SerializedProperty defaultPosition = serializedTrigger.FindProperty("defaultTriggerPosition");
		defaultPosition.vector3Value = EditorGUILayout.Vector3Field("Default Position", defaultPosition.vector3Value);
		SerializedProperty defaultRotation = serializedTrigger.FindProperty("defaultTriggerRotation");
		Quaternion tempDefaultRotation = defaultRotation.quaternionValue;
		tempDefaultRotation.eulerAngles = EditorGUILayout.Vector3Field("Default Rotation", tempDefaultRotation.eulerAngles);
		defaultRotation.quaternionValue = tempDefaultRotation;

		SerializedProperty pulledPosition = serializedTrigger.FindProperty("pulledTriggerPosition");
		pulledPosition.vector3Value = EditorGUILayout.Vector3Field("Pulled Position", pulledPosition.vector3Value);
		SerializedProperty pulledRotation = serializedTrigger.FindProperty("pulledTriggerRotation");
		Quaternion tempPulledRotation = pulledRotation.quaternionValue;
		tempPulledRotation.eulerAngles = EditorGUILayout.Vector3Field("Pulled Rotation", tempPulledRotation.eulerAngles);
		pulledRotation.quaternionValue = tempPulledRotation;

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Set Current To Default"))
		{
			triggerPulled = false;
			defaultPosition.vector3Value = gunTrigger.transform.localPosition;
			defaultRotation.quaternionValue = gunTrigger.transform.localRotation;
		}
		if (GUILayout.Button("Set Current To Pulled"))
		{
			triggerPulled = true;
			pulledPosition.vector3Value = gunTrigger.transform.localPosition;
			pulledRotation.quaternionValue = gunTrigger.transform.localRotation;
		}
		GUILayout.EndHorizontal();
		EditorGUILayout.HelpBox("Move the object in the scene and save the positions using the buttons above. Use the toggle button to switch between the two positions", MessageType.Info);

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Toggle Trigger"))
		{
			if (triggerPulled)
			{
				gunTrigger.transform.localPosition = gunTrigger.defaultTriggerPosition;
				gunTrigger.transform.localRotation = gunTrigger.defaultTriggerRotation;
			} else
			{
				gunTrigger.transform.localPosition = gunTrigger.pulledTriggerPosition;
				gunTrigger.transform.localRotation = gunTrigger.pulledTriggerRotation;
			}
			triggerPulled = !triggerPulled;
		}
		if (GUILayout.Button("Select Trigger"))
		{
			Selection.activeGameObject = ((VRGunTrigger)serializedTrigger.targetObject).gameObject;
		}

		if (GUILayout.Button("Create Pivot Object"))
		{
			//Create pivot
			GameObject newPivot = new GameObject("Trigger");
			newPivot.transform.SetParent(gunTrigger.transform.parent);
			newPivot.transform.localPosition = gunTrigger.transform.localPosition;
			newPivot.transform.localRotation = gunTrigger.transform.localRotation;
			newPivot.transform.localScale = Vector3.one;
			Vector3 oldScale = gunTrigger.transform.localScale;
			gunTrigger.transform.SetParent(newPivot.transform);
			gunTrigger.transform.localScale = oldScale;

			VRGunTrigger newTrigger = VRUtils.CopyComponent<VRGunTrigger>(gunTrigger, newPivot);
			if (serializedGunHandler != null)
			{
				serializedGunHandler.Update();
				SerializedProperty gunHandlerTrigger = serializedGunHandler.FindProperty("trigger");
				gunHandlerTrigger.objectReferenceValue = newTrigger;
				serializedGunHandler.ApplyModifiedProperties();
			}

			Undo.DestroyObjectImmediate(gunTrigger);
			gunTrigger = newTrigger;
			serializedTrigger = new SerializedObject(gunTrigger);
			Selection.activeGameObject = newPivot;
		}
		GUILayout.EndHorizontal();
		EditorGUILayout.HelpBox("Creating a pivot creates a new object as a parent that can be used to change the pivot point. Once made" +
			"you will be able to move the child of the newly created pivot object. The pivot position is used when finding what the controller" +
			"is closest to, so if all the pivot points are in the same place by default you will need to make you own. Note that the collider has" +
			"to be on the parent object and so will need to be manually adjusted.", MessageType.Info);

		EditorGUILayout.EndScrollView();
		serializedTrigger.ApplyModifiedProperties();
	}

	void SlideSection()
	{
		EditorGUI.indentLevel++;
		GUILayout.Label("Slide", EditorStyles.boldLabel);
		EditorGUI.indentLevel--;
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		if (serializedSlide != null) serializedSlide.Update();
		if (serializedGunHandler != null) serializedGunHandler.Update();
		var oldGunSlide = gunSlide;
		gunSlide = (VRGunSlide)EditorGUILayout.ObjectField("Gun Slide", gunSlide, typeof(VRGunSlide), true);
		if (gunSlide == null)
		{
			EditorGUILayout.EndScrollView();
			return;
		}

		if (oldGunSlide != gunSlide)
		{
			if (serializedGunHandler == null)
			{
				serializedGunHandler = new SerializedObject(gunHandler);
				serializedGunHandler.Update();
			}
			serializedSlide = new SerializedObject(gunSlide);
			SerializedProperty slideGunHandler = serializedSlide.FindProperty("gunHandler");
			slideGunHandler.objectReferenceValue = gunHandler;
			SerializedProperty gunHandlerSlide = serializedGunHandler.FindProperty("slide");
			gunHandlerSlide.objectReferenceValue = gunSlide;
			weaponTab = WeaponTab.SLIDE;
		}
		SerializedProperty slideItem = serializedSlide.FindProperty("item");
		slideItem.objectReferenceValue = EditorGUILayout.ObjectField("Item", slideItem.objectReferenceValue, typeof(Transform), true);

		if (slideItem.objectReferenceValue == null)
		{
			EditorGUILayout.EndScrollView();
			return;
		}

		SerializedProperty breakLimit = serializedSlide.FindProperty("breakLimit");
		breakLimit.floatValue = EditorGUILayout.FloatField("Break Limit", breakLimit.floatValue);

		EditorGUILayout.HelpBox("Break limit is how far the object has to be from the controller to automatically drop the item", MessageType.Info);

		SerializedProperty animateSlide = serializedSlide.FindProperty("animateSlide");
		animateSlide.boolValue = EditorGUILayout.Toggle("Animate When Firing", animateSlide.boolValue);

		SerializedProperty useAsSecondHeld = serializedSlide.FindProperty("useAsSecondHeld");
		var oldUseAsSecondHeld = useAsSecondHeld.boolValue;
		useAsSecondHeld.boolValue = EditorGUILayout.Toggle("Second Held Position", useAsSecondHeld.boolValue);
		if (useAsSecondHeld.boolValue != oldUseAsSecondHeld)
		{
			SerializedProperty secondHeld = serializedGunHandler.FindProperty("secondHeld");
			secondHeld.objectReferenceValue = useAsSecondHeld.boolValue ? gunSlide : null;
		}

		if (useAsSecondHeld.boolValue)
		{
			SerializedProperty toggleToPickup = serializedSlide.FindProperty("toggleToPickup");
			toggleToPickup.boolValue = EditorGUILayout.Toggle("Toggle To Pickup", toggleToPickup.boolValue);
		}

		if (animateSlide.boolValue && useAsSecondHeld.boolValue)
		{
			EditorGUILayout.HelpBox("The slide animation will conflict when holding the slide with the second held position and firing", MessageType.Warning);
		}

		SerializedProperty defaultPosition = serializedSlide.FindProperty("defaultPosition");
		defaultPosition.vector3Value = EditorGUILayout.Vector3Field("Default Position", defaultPosition.vector3Value);
		SerializedProperty defaultRotation = serializedSlide.FindProperty("defaultRotation");

		SerializedProperty pulledPosition = serializedSlide.FindProperty("pulledPosition");
		pulledPosition.vector3Value = EditorGUILayout.Vector3Field("Pulled Position", pulledPosition.vector3Value);

		Quaternion tempDefaultRotation = defaultRotation.quaternionValue;
		tempDefaultRotation.eulerAngles = EditorGUILayout.Vector3Field("Default Rotation", tempDefaultRotation.eulerAngles);
		defaultRotation.quaternionValue = tempDefaultRotation;

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Set Current To Default"))
		{
			slidePulled = false;
			defaultPosition.vector3Value = gunSlide.transform.localPosition;
			defaultRotation.quaternionValue = gunSlide.transform.localRotation;
		}
		if (GUILayout.Button("Set Current To Pulled"))
		{
			slidePulled = true;
			pulledPosition.vector3Value = gunSlide.transform.localPosition;
			defaultRotation.quaternionValue = gunSlide.transform.localRotation;
		}
		GUILayout.EndHorizontal();

		EditorGUILayout.HelpBox("Move the object in the scene and save the positions using the buttons above. Use the toggle button to switch between the two positions", MessageType.Info);

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Toggle Slide"))
		{
			if (slidePulled)
			{
				gunSlide.transform.localPosition = gunSlide.defaultPosition;
				gunSlide.transform.localRotation = gunSlide.defaultRotation;
			} else
			{
				gunSlide.transform.localPosition = gunSlide.pulledPosition;
				gunSlide.transform.localRotation = gunSlide.defaultRotation;
			}
			slidePulled = !slidePulled;
		}
		if (GUILayout.Button("Select Slide"))
		{
			Selection.activeGameObject = ((VRGunSlide)serializedSlide.targetObject).gameObject;
		}
		if (GUILayout.Button("Create Pivot Object"))
		{
			//Create pivot
			GameObject newPivot = new GameObject("Slide");
			newPivot.transform.SetParent(gunSlide.transform.parent);
			newPivot.transform.localPosition = gunSlide.transform.localPosition;
			newPivot.transform.localRotation = gunSlide.transform.localRotation;
			newPivot.transform.localScale = Vector3.one;
			Vector3 oldScale = gunSlide.transform.localScale;
			gunSlide.transform.SetParent(newPivot.transform);
			gunSlide.transform.localScale = oldScale;

			VRGunSlide newSlide = VRUtils.CopyComponent<VRGunSlide>(gunSlide, newPivot);
			newSlide.item = newPivot.transform;
			if (serializedGunHandler != null)
			{
				SerializedProperty gunHandlerSlide = serializedGunHandler.FindProperty("slide");
				gunHandlerSlide.objectReferenceValue = newSlide;
			}

			BoxCollider oldTriggerCollider = gunSlide.gameObject.GetComponent<BoxCollider>();
			VRGunWizard.ResizedCollider(newPivot, oldTriggerCollider);
			Undo.DestroyObjectImmediate(oldTriggerCollider);
			Undo.DestroyObjectImmediate(gunSlide);
			gunSlide = newSlide;
			serializedSlide = new SerializedObject(gunSlide);
			Selection.activeGameObject = newPivot;
		}
		Renderer meshRenderer = gunSlide.item.GetComponentInChildren<Renderer>();
		if (meshRenderer != null)
		{
			if (meshRenderer.gameObject != gunSlide.item.gameObject)
			{
				if (GUILayout.Button("Fix Pivot Collider"))
				{
					Undo.DestroyObjectImmediate(gunSlide.item.GetComponent<Collider>());
					BoxCollider rendererCollider = meshRenderer.gameObject.AddComponent<BoxCollider>();
					VRGunWizard.ResizedCollider(gunSlide.item.gameObject, rendererCollider);
				}
			}
		}
		GUILayout.EndHorizontal();
		EditorGUILayout.HelpBox("Creating a pivot creates a new object as a parent that can be used to change the pivot point. Once made" +
			"you will be able to move the child of the newly created pivot object. The pivot position is used when finding what the controller" +
			"is closest to, so if all the pivot points are in the same place by default you will need to make you own. Note that the collider has" +
			"to be on the parent object and can be fixed with the fix pivot collider button.", MessageType.Info);

		EditorGUI.indentLevel++;
		GUILayout.Label("Hover", EditorStyles.boldLabel);
		EditorGUI.indentLevel--;

		SerializedProperty hover = serializedSlide.FindProperty("hover");
		hover.objectReferenceValue = EditorGUILayout.ObjectField("Hover", hover.objectReferenceValue, typeof(Renderer), true);

		SerializedProperty hoverMode = serializedSlide.FindProperty("hoverMode");
		hoverMode.intValue = (int)(VRInteractableItem.HoverMode)EditorGUILayout.EnumPopup("Hover Mode", (VRInteractableItem.HoverMode)hoverMode.intValue);
		VRInteractableItem.HoverMode hoverModeEnum = (VRInteractableItem.HoverMode)hoverMode.intValue;
		switch(hoverModeEnum)
		{
		case VRInteractableItem.HoverMode.SHADER:
			EditorGUILayout.HelpBox("Leave null to use current materials shader", MessageType.Info);
			SerializedProperty defaultShader = serializedSlide.FindProperty("defaultShader");
			defaultShader.objectReferenceValue = EditorGUILayout.ObjectField("Default Shader", defaultShader.objectReferenceValue, typeof(Shader), false);
			EditorGUILayout.HelpBox("Hover Default is Unlit/Texture", MessageType.Info);
			SerializedProperty hoverShader = serializedSlide.FindProperty("hoverShader");
			hoverShader.objectReferenceValue = EditorGUILayout.ObjectField("Hover Shader", hoverShader.objectReferenceValue, typeof(Shader), false);
			break;
		case VRInteractableItem.HoverMode.MATERIAL:
			EditorGUILayout.HelpBox("Leave null to use current material", MessageType.Info);
			SerializedProperty defaultMat = serializedSlide.FindProperty("defaultMat");
			defaultMat.objectReferenceValue = EditorGUILayout.ObjectField("Default Material", defaultMat.objectReferenceValue, typeof(Material), false);
			EditorGUILayout.HelpBox("Hover Default is Unlit/Texture", MessageType.Info);
			SerializedProperty hoverMat = serializedSlide.FindProperty("hoverMat");
			hoverMat.objectReferenceValue = EditorGUILayout.ObjectField("Hover Material", hoverMat.objectReferenceValue, typeof(Material), false);
			break;
		}
		SerializedProperty enterHover = serializedSlide.FindProperty("enterHover");
		enterHover.objectReferenceValue = EditorGUILayout.ObjectField("Enter Hover Sound", enterHover.objectReferenceValue, typeof(AudioClip), false);
		SerializedProperty exitHover = serializedSlide.FindProperty("exitHover");
		exitHover.objectReferenceValue = EditorGUILayout.ObjectField("Exit Hover Sound", exitHover.objectReferenceValue, typeof(AudioClip), false);

		EditorGUILayout.EndScrollView();
		serializedSlide.ApplyModifiedProperties();
		if (serializedGunHandler != null) serializedGunHandler.ApplyModifiedProperties();
	}

	Vector2 scrollPos;

	void MagazineSection()
	{
		EditorGUI.indentLevel++;
		GUILayout.Label("Magazine", EditorStyles.boldLabel);
		EditorGUI.indentLevel--;

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		serializedGunHandler.Update();

		SerializedProperty integratedMagazine = serializedGunHandler.FindProperty("integratedMagazine");
		var oldIntegatedMagazine = integratedMagazine.objectReferenceValue;
		integratedMagazine.objectReferenceValue = EditorGUILayout.ObjectField("Integrated Magazine", integratedMagazine.objectReferenceValue, typeof(VRMagazine), true);
		if (integratedMagazine.objectReferenceValue != null)
		{
			if (oldIntegatedMagazine != integratedMagazine.objectReferenceValue)
				serializedMagazine = new SerializedObject(integratedMagazine.objectReferenceValue);
			
			serializedMagazine.Update();
			SerializedProperty magazineItem = serializedMagazine.FindProperty("item");
			magazineItem.objectReferenceValue = EditorGUILayout.ObjectField("Item", magazineItem.objectReferenceValue, typeof(Transform), true);
			SerializedProperty gunMagazineId = serializedGunHandler.FindProperty("magazineId");
			SerializedProperty magMagazineId = serializedMagazine.FindProperty("magazineId");
			gunMagazineId.intValue = EditorGUILayout.IntField("Magazine Id", gunMagazineId.intValue);
			magMagazineId.intValue = gunMagazineId.intValue;
			SerializedProperty infiniteAmmo = serializedMagazine.FindProperty("infiniteAmmo");
			infiniteAmmo.boolValue = EditorGUILayout.Toggle("Infinite Ammo", infiniteAmmo.boolValue);

			EditorGUI.indentLevel++;
			GUILayout.Label("Bullets", EditorStyles.boldLabel);
			EditorGUI.indentLevel--;

			SerializedProperty clipSize = serializedMagazine.FindProperty("clipSize");
			var oldClipSize = clipSize.intValue;
			clipSize.intValue = EditorGUILayout.IntField("Clip Size", clipSize.intValue);
			SerializedProperty loadedBullet = serializedGunHandler.FindProperty("loadedBulletPrefab");
			loadedBullet.objectReferenceValue = EditorGUILayout.ObjectField("Bullet Prefab", loadedBullet.objectReferenceValue, typeof(GameObject), false);
			SerializedProperty bulletPrefab = serializedMagazine.FindProperty("bulletPrefab");
			bulletPrefab.objectReferenceValue = loadedBullet.objectReferenceValue;

			if (bulletPrefab.objectReferenceValue != null)
			{
				magBulletFoldout = EditorGUILayout.Foldout(magBulletFoldout, "Magazine Bullet Settings");
				if (magBulletFoldout)
				{
					EditorGUI.indentLevel++;
					SerializedProperty bulletVisible = serializedMagazine.FindProperty("bulletVisible");
					SerializedProperty bulletPositions = serializedMagazine.FindProperty("bulletPositions");
					SerializedProperty bulletRotations = serializedMagazine.FindProperty("bulletRotations");
					if (oldClipSize != clipSize.intValue || bulletVisible.arraySize != clipSize.intValue ||
						bulletPositions.arraySize !=  clipSize.intValue || bulletRotations.arraySize != clipSize.intValue)
					{
						if (clipSize.intValue < oldClipSize)
						{
							bulletVisible.ClearArray();
							bulletPositions.ClearArray();
							bulletRotations.ClearArray();
						}
						bulletVisible.arraySize = bulletPositions.arraySize = bulletRotations.arraySize = clipSize.intValue;
					}

					EditorGUILayout.HelpBox("Bullet one goes at the top of the clip aka closest to the loaded position", MessageType.Info);

					for(int i=0; i<clipSize.intValue; i++)
					{
						SerializedProperty visible = bulletVisible.GetArrayElementAtIndex(i);
						visible.boolValue = EditorGUILayout.Toggle("Bullet "+(i+1)+" Visible", visible.boolValue);
						if (visible.boolValue)
						{
							SerializedProperty bulletPosition = bulletPositions.GetArrayElementAtIndex(i);
							EditorGUILayout.LabelField("Position: " + bulletPosition.vector3Value.ToString("G4"));
							SerializedProperty bulletRotation = bulletRotations.GetArrayElementAtIndex(i);
							EditorGUILayout.LabelField("Rotation: " + bulletRotation.quaternionValue.eulerAngles.ToString("G4"));
							if (bulletInstances.Count < clipSize.intValue)
							{
								int extraNeeded = clipSize.intValue-bulletInstances.Count;
								for(int j=0; j<extraNeeded; j++)
									bulletInstances.Add(null);
							} else if (bulletInstances.Count > clipSize.intValue)
							{
								int toRemove = bulletInstances.Count - clipSize.intValue;
								for(int j=bulletInstances.Count-toRemove; j<toRemove; j++)
								{
									Undo.DestroyObjectImmediate(bulletInstances[i]);
								}
								bulletInstances.RemoveRange(bulletInstances.Count-toRemove, toRemove);
							}
							if (bulletInstances[i] == null)
							{
								if (GUILayout.Button("Spawn Reference"))
								{
									GameObject referenceBullet = (GameObject)Instantiate(bulletPrefab.objectReferenceValue, Vector3.zero, Quaternion.identity);
									Undo.RegisterCreatedObjectUndo(referenceBullet, "Create reference bullet");
									referenceBullet.transform.SetParent(((VRMagazine)integratedMagazine.objectReferenceValue).item);
									referenceBullet.transform.localPosition = bulletPosition.vector3Value;
									referenceBullet.transform.localRotation = bulletRotation.quaternionValue;
									bulletInstances.Insert(i, referenceBullet);
									Selection.activeGameObject = referenceBullet;
								}
							} else
							{
								GUILayout.BeginHorizontal();
								if (GUILayout.Button("Select Bullet"))
								{
									Selection.activeGameObject = bulletInstances[i];
								}
								if (GUILayout.Button("Save Current Position"))
								{
									bulletPosition.vector3Value = bulletInstances[i].transform.localPosition;
									bulletRotation.quaternionValue = bulletInstances[i].transform.localRotation;
								}
								if(GUILayout.Button("Destory Bullet"))
								{
									Undo.DestroyObjectImmediate(bulletInstances[i]);
								}
								GUILayout.EndHorizontal();
							}
						}
					}
					EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
					EditorGUI.indentLevel--;
				}
			}

			SerializedProperty bulletReceiver = serializedMagazine.FindProperty("bulletReceiver");
			bulletReceiver.objectReferenceValue = EditorGUILayout.ObjectField("Bullet Receiver", bulletReceiver.objectReferenceValue, typeof(BoxCollider), true);
			if (bulletReceiver.objectReferenceValue == null)
			{
				if (GUILayout.Button("Create Bullet Receiver"))
				{
					GameObject bulletEntryColliderObj = new GameObject("BulletReceiver");
					Undo.RegisterCreatedObjectUndo(bulletEntryColliderObj, "Create Bullet Receiver");
					bulletEntryColliderObj.transform.SetParent(((VRMagazine)integratedMagazine.objectReferenceValue).item);
					bulletEntryColliderObj.transform.localPosition = Vector3.zero;
					bulletEntryColliderObj.transform.localRotation = Quaternion.identity;
					bulletReceiver.objectReferenceValue = bulletEntryColliderObj.AddComponent<BoxCollider>();
					((BoxCollider)bulletReceiver.objectReferenceValue).size = new Vector3(0.05f*((VRMagazine)integratedMagazine.objectReferenceValue).item.localScale.magnitude, 0.001f*((VRMagazine)integratedMagazine.objectReferenceValue).item.localScale.magnitude, 0.05f*((VRMagazine)integratedMagazine.objectReferenceValue).item.localScale.magnitude);
					((BoxCollider)bulletReceiver.objectReferenceValue).isTrigger = true;
					VRBulletReceiver bulletReceiverScript = bulletEntryColliderObj.AddComponent<VRBulletReceiver>();
					bulletReceiverScript.magazine = (VRMagazine)integratedMagazine.objectReferenceValue;
					Selection.activeGameObject = bulletEntryColliderObj;
				}
			} else
			{
				EditorGUILayout.HelpBox("When a bullet matching the bullet id touches the collider it will attempt to load", MessageType.Info);
				SerializedProperty bulletId = serializedMagazine.FindProperty("bulletId");
				var oldBulletId = bulletId.intValue;
				bulletId.intValue = EditorGUILayout.IntField("Bullet Id", bulletId.intValue);
				if (oldBulletId != bulletId.intValue)
				{
					SerializedProperty gunBulletId = serializedGunHandler.FindProperty("bulletId");
					gunBulletId.intValue = bulletId.intValue;
				}
				if (GUILayout.Button("Select Bullet Receiver"))
				{
					Selection.activeGameObject = ((BoxCollider)bulletReceiver.objectReferenceValue).gameObject;
				}
			}

			serializedMagazine.ApplyModifiedProperties();

		} else
		{
			if (oldIntegatedMagazine != null)
				serializedMagazine = null;

			SerializedProperty startLoaded = serializedGunHandler.FindProperty("startLoaded");
			startLoaded.boolValue = EditorGUILayout.Toggle("Start Loaded", startLoaded.boolValue);
			SerializedProperty magazinePrefab = serializedGunHandler.FindProperty("magazinePrefab");
			magazinePrefab.objectReferenceValue = EditorGUILayout.ObjectField("Magazine Prefab", magazinePrefab.objectReferenceValue, typeof(GameObject), false);
			if (magazinePrefab.objectReferenceValue != null)
			{
				if (magazineInstance == null)
				{
					if (GUILayout.Button("Spawn Magazine"))
					{
						magazineInstance = (GameObject)Instantiate(magazinePrefab.objectReferenceValue, Vector3.zero, Quaternion.identity);
						magazine = magazineInstance.GetComponentInChildren<VRMagazine>();
						if (magazine == null)
						{
							if (magazine != null && magazine.item == null)
							{
								Debug.LogError("Magazine Script does not have a reference to item");
							} else
							{
								Debug.LogError("Magazine Prefab has no VRMagazine script");
							}
							DestroyImmediate(magazineInstance);
							return;
						}
						Undo.RegisterCreatedObjectUndo(magazineInstance, "Edit Magazine");
						serializedMagazine = new SerializedObject(magazine);
						magazineInstance.transform.SetParent(gunHandler.item);
						magazineInstance.transform.localPosition = magazine.defaultLoadedPosition;
						magazineInstance.transform.localRotation = magazine.defaultRotation;
						magazineInstance.transform.localScale = ((GameObject)magazinePrefab.objectReferenceValue).transform.localScale;
						magazineLoaded = true;
					}
				} else
				{
					if (magazine == null)
						magazine = magazineInstance.GetComponentInChildren<VRMagazine>();
					if (serializedMagazine == null)
						serializedMagazine = new SerializedObject(magazine);
					serializedMagazine.Update();
					bool changed = false;

					SerializedProperty magazineItem = serializedMagazine.FindProperty("item");
					magazineItem.objectReferenceValue = EditorGUILayout.ObjectField("Item", magazineItem.objectReferenceValue, typeof(Transform), true);

					SerializedProperty autoLoad = serializedMagazine.FindProperty("autoLoad");
					var oldAutoLoad = autoLoad.boolValue;
					autoLoad.boolValue = EditorGUILayout.Toggle("Auto Load", autoLoad.boolValue);
					if (oldAutoLoad != autoLoad.boolValue) changed = true;

					if (autoLoad.boolValue)
					{
						SerializedProperty autoLoadSpeed = serializedMagazine.FindProperty("autoLoadSpeed");
						var oldAutoLoadSpeed = autoLoadSpeed.floatValue;
						autoLoadSpeed.floatValue = EditorGUILayout.FloatField("Auto Load Speed", autoLoadSpeed.floatValue);
						if (oldAutoLoadSpeed != autoLoadSpeed.floatValue) changed = true;
					}

					SerializedProperty infiniteAmmo = serializedMagazine.FindProperty("infiniteAmmo");
					var oldInfiniteAmmo = infiniteAmmo.boolValue;
					infiniteAmmo.boolValue = EditorGUILayout.Toggle("Infinite Ammo", infiniteAmmo.boolValue);
					if (oldInfiniteAmmo != infiniteAmmo.boolValue) changed = true;
					SerializedProperty gunMagazineId = serializedGunHandler.FindProperty("magazineId");
					SerializedProperty magMagazineId = serializedMagazine.FindProperty("magazineId");
					gunMagazineId.intValue = EditorGUILayout.IntField("Magazine Id", gunMagazineId.intValue);
					var oldMagazineId = magMagazineId.intValue;
					magMagazineId.intValue = gunMagazineId.intValue;
					if (oldMagazineId != magMagazineId.intValue) changed = true;

					SerializedProperty bulletId = serializedMagazine.FindProperty("bulletId");
					var oldBulletId = bulletId.intValue;
					bulletId.intValue = EditorGUILayout.IntField("Bullet Id", bulletId.intValue);
					if (oldBulletId != bulletId.intValue)
					{
						SerializedProperty gunBulletId = serializedGunHandler.FindProperty("bulletId");
						gunBulletId.intValue = bulletId.intValue;
						changed = true;
					}

					if (GUILayout.Button("Setup Magazine Held Position"))
					{
						HeldPositionWindow magHeldWindow = (HeldPositionWindow)EditorWindow.GetWindow(typeof(HeldPositionWindow), true, "Magazine Held Position", true);
						magHeldWindow.interactableItem = magazine;
						magHeldWindow.gunHandlerWindow = this;
						magHeldWindow.magazinePrefab = (GameObject)magazinePrefab.objectReferenceValue;
						magHeldWindow.Init();
					}

					EditorGUI.indentLevel++;
					GUILayout.Label("Magazine Position", EditorStyles.boldLabel);
					EditorGUI.indentLevel--;

					SerializedProperty defaultMagPosition = serializedMagazine.FindProperty("defaultLoadedPosition");
					var oldDefaultMagPosition = defaultMagPosition.vector3Value;
					defaultMagPosition.vector3Value = EditorGUILayout.Vector3Field("Default Magazine Position", defaultMagPosition.vector3Value);
					if (oldDefaultMagPosition != defaultMagPosition.vector3Value) changed = true;
					SerializedProperty defaultRotation = serializedMagazine.FindProperty("defaultRotation");
					SerializedProperty entryMagPosition = serializedMagazine.FindProperty("entryPosition");
					var oldEntryPosition = entryMagPosition.vector3Value;
					entryMagPosition.vector3Value = EditorGUILayout.Vector3Field("Entry Magazine Position", entryMagPosition.vector3Value);
					if (oldEntryPosition != entryMagPosition.vector3Value) changed = true;
					Quaternion tempRotation = defaultRotation.quaternionValue;
					var oldTempRotation = tempRotation.eulerAngles;
					tempRotation.eulerAngles = EditorGUILayout.Vector3Field("Default Magazine Rotation", tempRotation.eulerAngles);
					if (oldTempRotation != tempRotation.eulerAngles) changed = true;
					defaultRotation.quaternionValue = tempRotation;

					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Assign Current To Default"))
					{
						defaultMagPosition.vector3Value = magazine.item.localPosition;
						defaultRotation.quaternionValue = magazine.item.localRotation;
						magazineLoaded = true;
						changed = true;
					}
					if (GUILayout.Button("Assign Current To Entry"))
					{
						entryMagPosition.vector3Value = magazine.item.localPosition;
						defaultRotation.quaternionValue = magazine.item.localRotation;
						magazineLoaded = false;
						changed = true;
					}
					GUILayout.EndHorizontal();

					EditorGUILayout.HelpBox("Move the magazine in the scene and save the positions using the buttons above. Use the toggle button to switch between the two positions", MessageType.Info);

					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Toggle Magazine Position"))
					{
						if (magazineLoaded)
						{
							Undo.RecordObject(magazine.item, "Toggle Magazine Position");
							magazine.item.localPosition = magazine.entryPosition;
							magazine.item.localRotation = magazine.defaultRotation;
						} else
						{
							Undo.RecordObject(magazine.item, "Toggle Magazine Position");
							magazine.item.localPosition = magazine.defaultLoadedPosition;
							magazine.item.localRotation = magazine.defaultRotation;
						}
						magazineLoaded = !magazineLoaded;
					}
					if (GUILayout.Button("Select Magazine"))
					{
						Selection.activeGameObject = magazineInstance;
					}
					bool destroyMag = false;
					if (GUILayout.Button("Destroy Magazine"))
					{
						destroyMag = true;
					}
					if (GUILayout.Button("Create Pivot Object"))
					{
						//Create pivot
						GameObject newPivot = new GameObject("Magazine");
						newPivot.transform.SetParent(magazine.item.parent);
						newPivot.transform.localPosition = magazine.item.localPosition;
						newPivot.transform.localRotation = magazine.item.localRotation;
						newPivot.transform.localScale = Vector3.one;
						Rigidbody oldRigidBody = magazine.item.GetComponent<Rigidbody>();
						if (oldRigidBody != null)
						{
							VRUtils.CopyComponent<Rigidbody>(oldRigidBody, newPivot);
							Undo.DestroyObjectImmediate(oldRigidBody);
						}
						BoxCollider oldCollider = magazine.item.GetComponent<BoxCollider>();
						if (oldCollider != null)
						{
							VRGunWizard.ResizedCollider(newPivot, oldCollider);
							Undo.DestroyObjectImmediate(oldCollider);
						}
						Vector3 oldScale = magazine.item.localScale;
						magazine.item.SetParent(newPivot.transform);
						magazine.item.localScale = oldScale;
						magazine.item.name = "Pivot";
						Selection.activeGameObject = magazine.item.gameObject;
						magazine.item = newPivot.transform;
						magazineInstance = newPivot;
						magazineItem.objectReferenceValue = newPivot.transform;
						changed = true;
					}
					Renderer meshRenderer = magazineInstance.GetComponentInChildren<Renderer>();
					if (meshRenderer != null && meshRenderer.gameObject != magazineInstance && (meshRenderer.GetComponent<Collider>() == null || meshRenderer.GetComponent<Collider>().isTrigger))
					{
						if (GUILayout.Button("Fix Pivot Collider"))
						{
							Undo.DestroyObjectImmediate(magazineInstance.GetComponent<Collider>());
							BoxCollider rendererCollider = meshRenderer.GetComponent<BoxCollider>();
							if (rendererCollider == null) rendererCollider = meshRenderer.gameObject.AddComponent<BoxCollider>();
							rendererCollider.isTrigger = true;
							VRGunWizard.ResizedCollider(magazineInstance, rendererCollider);
							changed = true;
						}
					}
					GUILayout.EndHorizontal();
					EditorGUILayout.HelpBox("Creating a pivot creates a new object as a parent that can be used to change the pivot point. Once made" +
						"you will be able to move the child of the newly created pivot object. The pivot position is used when finding what the controller" +
						"is closest to, so if all the pivot points are in the same place by default you will need to make you own. Note that the collider has" +
						"to be on the parent object and can be fixed with the fix pivot collider button.", MessageType.Info);

					EditorGUI.indentLevel++;
					GUILayout.Label("Bullets", EditorStyles.boldLabel);
					EditorGUI.indentLevel--;

					SerializedProperty clipSize = serializedMagazine.FindProperty("clipSize");
					var oldClipSize = clipSize.intValue;
					clipSize.intValue = EditorGUILayout.IntField("Clip Size", clipSize.intValue);
					if (oldClipSize != clipSize.intValue) changed = true;
					SerializedProperty loadedBullet = serializedGunHandler.FindProperty("loadedBulletPrefab");
					loadedBullet.objectReferenceValue = EditorGUILayout.ObjectField("Bullet Prefab", loadedBullet.objectReferenceValue, typeof(GameObject), false);
					SerializedProperty bulletPrefab = serializedMagazine.FindProperty("bulletPrefab");
					var oldBulletPrefab = bulletPrefab.objectReferenceValue;
					bulletPrefab.objectReferenceValue = loadedBullet.objectReferenceValue;
					if (oldBulletPrefab != bulletPrefab.objectReferenceValue) changed = true;

					if (bulletPrefab.objectReferenceValue != null)
					{
						magBulletFoldout = EditorGUILayout.Foldout(magBulletFoldout, "Magazine Bullet Settings");
						if (magBulletFoldout)
						{
							EditorGUI.indentLevel++;
							SerializedProperty bulletVisible = serializedMagazine.FindProperty("bulletVisible");
							SerializedProperty bulletPositions = serializedMagazine.FindProperty("bulletPositions");
							SerializedProperty bulletRotations = serializedMagazine.FindProperty("bulletRotations");
							var oldBulletSize = bulletVisible.arraySize;
							bulletVisible.arraySize = bulletPositions.arraySize = bulletRotations.arraySize = clipSize.intValue;
							if (oldBulletSize != bulletVisible.arraySize)
								changed = true;

							EditorGUILayout.HelpBox("You can go through each bullet, first toggle whether or not it is visible, if it is spawn a reference position and" +
								"save. The reference can be destroyed once the position is saved. Bullet one goes at the top of the clip aka closest to the loaded position", MessageType.Info);

							for(int i=0; i<clipSize.intValue; i++)
							{
								SerializedProperty visible = bulletVisible.GetArrayElementAtIndex(i);
								var oldVisible = visible.boolValue;
								visible.boolValue = EditorGUILayout.Toggle("Bullet "+(i+1)+" Visible", visible.boolValue);
								if (oldVisible != visible.boolValue) changed = true;
								if (visible.boolValue)
								{
									SerializedProperty bulletPosition = bulletPositions.GetArrayElementAtIndex(i);
									var oldBulletPosition = bulletPosition.vector3Value;
									EditorGUILayout.LabelField("Position: " + bulletPosition.vector3Value.ToString("G4"));
									if (oldBulletPosition != bulletPosition.vector3Value) changed = true;
									SerializedProperty bulletRotation = bulletRotations.GetArrayElementAtIndex(i);
									EditorGUILayout.LabelField("Rotation: " + bulletRotation.quaternionValue.eulerAngles.ToString("G4"));
									if (bulletInstances.Count < clipSize.intValue)
									{
										int extraNeeded = clipSize.intValue-bulletInstances.Count;
										for(int j=0; j<extraNeeded; j++)
											bulletInstances.Add(null);
									} else if (bulletInstances.Count > clipSize.intValue)
									{
										int toRemove = bulletInstances.Count - clipSize.intValue;
										for(int j=bulletInstances.Count-toRemove; j<toRemove; j++)
										{
											Undo.DestroyObjectImmediate(bulletInstances[i]);
										}
										bulletInstances.RemoveRange(bulletInstances.Count-toRemove, toRemove);
									}
									if (bulletInstances[i] == null)
									{
										if (GUILayout.Button("Spawn Reference"))
										{
											GameObject referenceBullet = (GameObject)Instantiate(bulletPrefab.objectReferenceValue, Vector3.zero, Quaternion.identity);
											Undo.RegisterCreatedObjectUndo(referenceBullet, "Create reference bullet");
											referenceBullet.transform.SetParent(magazine.item);
											referenceBullet.transform.localPosition = bulletPosition.vector3Value;
											referenceBullet.transform.localRotation = bulletRotation.quaternionValue;
											bulletInstances.Insert(i, referenceBullet);
											Selection.activeGameObject = referenceBullet;
										}
									} else
									{
										GUILayout.BeginHorizontal();
										if (GUILayout.Button("Select Bullet"))
										{
											Selection.activeGameObject = bulletInstances[i];
										}
										if (GUILayout.Button("Save Current Position"))
										{
											bulletPosition.vector3Value = bulletInstances[i].transform.localPosition;
											bulletRotation.quaternionValue = bulletInstances[i].transform.localRotation;
											changed = true;
										}
										if(GUILayout.Button("Destory Bullet"))
										{
											Undo.DestroyObjectImmediate(bulletInstances[i]);
										}
										GUILayout.EndHorizontal();
									}
								}
							}
							EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
							EditorGUI.indentLevel++;
						}
					}

					SerializedProperty bulletReceiver = serializedMagazine.FindProperty("bulletReceiver");
					var oldBulletReceiver = bulletReceiver.objectReferenceValue;
					bulletReceiver.objectReferenceValue = EditorGUILayout.ObjectField("Bullet Receiver", bulletReceiver.objectReferenceValue, typeof(BoxCollider), true);
					if (oldBulletReceiver != bulletReceiver.objectReferenceValue) changed = true;
					if (bulletReceiver.objectReferenceValue == null)
					{
						if (GUILayout.Button("Create Bullet Receiver"))
						{
							GameObject bulletEntryColliderObj = new GameObject("BulletReceiver");
							Undo.RegisterCreatedObjectUndo(bulletEntryColliderObj, "Create Bullet Receiver");
							bulletEntryColliderObj.transform.SetParent(magazine.item);
							bulletEntryColliderObj.transform.localPosition = Vector3.zero;
							bulletEntryColliderObj.transform.localRotation = Quaternion.identity;
							bulletReceiver.objectReferenceValue = bulletEntryColliderObj.AddComponent<BoxCollider>();
							((BoxCollider)bulletReceiver.objectReferenceValue).size = new Vector3(0.05f*magazine.item.localScale.magnitude, 0.001f*magazine.item.localScale.magnitude, 0.05f*magazine.item.localScale.magnitude);
							((BoxCollider)bulletReceiver.objectReferenceValue).isTrigger = true;
							VRBulletReceiver bulletReceiverScript = bulletEntryColliderObj.AddComponent<VRBulletReceiver>();
							bulletReceiverScript.magazine = magazine;
							Selection.activeGameObject = bulletEntryColliderObj;
						}
					} else
					{
						/*EditorGUILayout.HelpBox("When a bullet matching the bullet id touches the collider it will attempt to load", MessageType.Info);
						SerializedProperty bulletId = serializedMagazine.FindProperty("bulletId");
						var oldBulletId = bulletId.intValue;
						bulletId.intValue = EditorGUILayout.IntField("Bullet Id", bulletId.intValue);
						if (oldBulletId != bulletId.intValue) changed = true;*/
						GUILayout.BeginHorizontal();
						if (GUILayout.Button("Select Bullet Receiver"))
						{
							Selection.activeGameObject = ((BoxCollider)bulletReceiver.objectReferenceValue).gameObject;
						}
						if (GUILayout.Button("Save"))
						{
							changed = true;
						}
						GUILayout.EndHorizontal();
					}

					EditorGUI.indentLevel++;
					GUILayout.Label("Hover", EditorStyles.boldLabel);
					EditorGUI.indentLevel--;

					SerializedProperty hover = serializedMagazine.FindProperty("hover");
					var oldHover = hover.objectReferenceValue;
					hover.objectReferenceValue = EditorGUILayout.ObjectField("Hover", hover.objectReferenceValue, typeof(Renderer), true);
					if (oldHover != hover.objectReferenceValue) changed = true;

					SerializedProperty hoverMode = serializedMagazine.FindProperty("hoverMode");
					var oldHoverMode = hoverMode.intValue;
					hoverMode.intValue = (int)(VRInteractableItem.HoverMode)EditorGUILayout.EnumPopup("Hover Mode", (VRInteractableItem.HoverMode)hoverMode.intValue);
					if (oldHoverMode != hoverMode.intValue) changed = true;
					VRInteractableItem.HoverMode hoverModeEnum = (VRInteractableItem.HoverMode)hoverMode.intValue;
					switch(hoverModeEnum)
					{
					case VRInteractableItem.HoverMode.SHADER:
						EditorGUILayout.HelpBox("Leave null to use current materials shader", MessageType.Info);
						SerializedProperty defaultShader = serializedMagazine.FindProperty("defaultShader");
						var oldDefaultShader = defaultShader.objectReferenceValue;
						defaultShader.objectReferenceValue = EditorGUILayout.ObjectField("Default Shader", defaultShader.objectReferenceValue, typeof(Shader), false);
						if (oldDefaultShader != defaultShader.objectReferenceValue) changed = true;
						EditorGUILayout.HelpBox("Hover Default is Unlit/Texture", MessageType.Info);
						SerializedProperty hoverShader = serializedMagazine.FindProperty("hoverShader");
						var oldHoverShader = hoverShader.objectReferenceValue;
						hoverShader.objectReferenceValue = EditorGUILayout.ObjectField("Hover Shader", hoverShader.objectReferenceValue, typeof(Shader), false);
						if (oldHoverShader != hoverShader.objectReferenceValue) changed = true;
						break;
					case VRInteractableItem.HoverMode.MATERIAL:
						EditorGUILayout.HelpBox("Leave null to use current material", MessageType.Info);
						SerializedProperty defaultMat = serializedMagazine.FindProperty("defaultMat");
						var oldDefaultMat = defaultMat.objectReferenceValue;
						defaultMat.objectReferenceValue = EditorGUILayout.ObjectField("Default Material", defaultMat.objectReferenceValue, typeof(Material), false);
						if (oldDefaultMat != defaultMat.objectReferenceValue) changed = true;
						EditorGUILayout.HelpBox("Hover Default is Unlit/Texture", MessageType.Info);
						SerializedProperty hoverMat = serializedMagazine.FindProperty("hoverMat");
						var oldHoverMat = hoverMat.objectReferenceValue;
						hoverMat.objectReferenceValue = EditorGUILayout.ObjectField("Hover Material", hoverMat.objectReferenceValue, typeof(Material), false);
						if (oldHoverMat != hoverMat.objectReferenceValue) changed = true;
						break;
					}

					serializedMagazine.ApplyModifiedProperties();
					if (changed)
					{
						SaveMagazinePrefab((GameObject)magazinePrefab.objectReferenceValue);
					}
					
					if (destroyMag) Undo.DestroyObjectImmediate(magazineInstance);
				}
			}

			SerializedProperty clipReceiver = serializedGunHandler.FindProperty("clipReceiver");
			clipReceiver.objectReferenceValue = EditorGUILayout.ObjectField("Clip Receiver", clipReceiver.objectReferenceValue, typeof(BoxCollider), true);
			if (clipReceiver.objectReferenceValue == null)
			{
				if (GUILayout.Button("Create Clip Receiver"))
				{
					GameObject magEntryColliderObj = new GameObject("ClipReceiver");
					Undo.RegisterCreatedObjectUndo(magEntryColliderObj, "Create Clip Receiver");
					magEntryColliderObj.transform.SetParent(gunHandler.item);
					magEntryColliderObj.transform.localPosition = Vector3.zero;
					magEntryColliderObj.transform.localRotation = Quaternion.identity;
					clipReceiver.objectReferenceValue = magEntryColliderObj.AddComponent<BoxCollider>();
					((BoxCollider)clipReceiver.objectReferenceValue).size = new Vector3(0.05f*gunHandler.item.localScale.magnitude, 0.001f*gunHandler.item.localScale.magnitude, 0.05f*gunHandler.item.localScale.magnitude);;
					((BoxCollider)clipReceiver.objectReferenceValue).isTrigger = true;
					VRClipReceiver clipReceiverScript = magEntryColliderObj.AddComponent<VRClipReceiver>();
					clipReceiverScript.gunHandler = gunHandler;
					Selection.activeGameObject = magEntryColliderObj;
				}
			} else
			{
				EditorGUILayout.HelpBox("When the magazine touches the clip receiver it will begin" +
					" sliding across the entry-loaded position line. Position so that it is some " +
					"way along that line. If there is flickering move the receiver closer to the loaded position", MessageType.Info);
				
				if (GUILayout.Button("Select Clip Receiver"))
				{
					Selection.activeGameObject = ((BoxCollider)clipReceiver.objectReferenceValue).gameObject;
				}
			}
		}
		serializedGunHandler.ApplyModifiedProperties();
		EditorGUILayout.EndScrollView();
	}

	public void SaveMagazinePrefab(GameObject magazinePrefab)
	{
		for(int i=0; i<bulletInstances.Count;i++)
		{
			if (bulletInstances[i] != null) bulletInstances[i].transform.SetParent(null);
		}
		PrefabUtility.ReplacePrefab(magazineInstance, magazinePrefab, ReplacePrefabOptions.ConnectToPrefab | ReplacePrefabOptions.ReplaceNameBased);
		for(int i=0; i<bulletInstances.Count;i++)
		{
			if (bulletInstances[i] != null) bulletInstances[i].transform.SetParent(magazine.item);
		}
	}

	void OnDestroy()
	{
		if (bulletInstance != null) Undo.DestroyObjectImmediate(bulletInstance);
		if (smokeInstance != null) Undo.DestroyObjectImmediate(smokeInstance);
		if (muzzleFlashInstance != null) Undo.DestroyObjectImmediate(muzzleFlashInstance);
		if (ejectionOriginInstance != null) Undo.DestroyObjectImmediate(ejectionOriginInstance);
		if (ejectionDestinationInstance != null) Undo.DestroyObjectImmediate(ejectionDestinationInstance);
		if (shootOriginInstance != null) Undo.DestroyObjectImmediate(shootOriginInstance);
		if (shootDirectionInstance != null) Undo.DestroyObjectImmediate(shootDirectionInstance);
		if (laserPointerOriginInstance != null) Undo.DestroyObjectImmediate(laserPointerOriginInstance);
		if (bulletInstances.Count != 0)
		{
			foreach(GameObject bullet in bulletInstances)
			{
				if (bullet == null) continue;
				Undo.DestroyObjectImmediate(bullet);
			}
		}
		if (magazineInstance != null) Undo.DestroyObjectImmediate(magazineInstance);
	}

	void ReferencePositionConfig(SerializedObject refObject, string positionVar, string rotationVar, GameObject objectPrefab, ref GameObject objectInstance)
	{
		EditorGUI.indentLevel++;
		SerializedProperty position = refObject.FindProperty(positionVar);
		position.vector3Value = EditorGUILayout.Vector3Field("Position", position.vector3Value);
		SerializedProperty rotation = refObject.FindProperty(rotationVar);
		Quaternion newRotation = rotation.quaternionValue;
		newRotation.eulerAngles = EditorGUILayout.Vector3Field("Rotation", newRotation.eulerAngles);
		rotation.quaternionValue = newRotation;

		if (objectInstance == null)
		{
			if (GUILayout.Button("Setup Position"))
			{
				objectInstance = (GameObject)Instantiate(objectPrefab, Vector3.zero, Quaternion.identity);
				Undo.RegisterCreatedObjectUndo(objectInstance, "Spawn Reference");
				SerializedProperty item = refObject.FindProperty("item");
				objectInstance.transform.SetParent((Transform)item.objectReferenceValue);
				objectInstance.transform.localPosition = position.vector3Value;
				objectInstance.transform.localRotation = rotation.quaternionValue;
				Selection.activeGameObject = objectInstance;
			}
		} else
		{
			EditorGUILayout.HelpBox("Move object into the default position", MessageType.Info);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Save"))
			{
				position.vector3Value = objectInstance.transform.localPosition;
				rotation.quaternionValue = objectInstance.transform.localRotation;
				Undo.DestroyObjectImmediate(objectInstance);
			}
			if (GUILayout.Button("Select Object"))
			{
				Selection.activeGameObject = objectInstance;
			}
			if (GUILayout.Button("Cancel"))
			{
				Undo.DestroyObjectImmediate(objectInstance);
			}
			GUILayout.EndHorizontal();
		}

		EditorGUI.indentLevel--;
	}
}