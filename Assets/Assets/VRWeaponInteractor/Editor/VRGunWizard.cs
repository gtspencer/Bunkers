using UnityEngine;
using UnityEditor;
using System.Collections;

public class VRGunWizard : EditorWindow 
{
	//References
	GameObject gunModel;
	GameObject gunMesh;
	GameObject slideMesh;
	GameObject triggerMesh;
	GameObject magMesh;

	string magazinePrefabPath;
	bool magazinePathValid = false;
	int magazineId;

	[MenuItem("VR Weapon Interactor/Weapon Wizard", false, 0)]
	public static void MenuInitAbout()
	{
		EditorWindow.GetWindow(typeof(VRGunWizard), true, "Weapon Wizard", true);
	}

	void OnGUI () 
	{
		GUILayout.Label ("Welcome To The VR Weapon Setup Wizard", EditorStyles.boldLabel);
		EditorGUILayout.HelpBox("First make sure the gun model has the correct format. The Animation Type in Rig should be set to none. When dragged into the hierarchy " +
			"the model parent should be an empty transform. It should have minimum one mesh," +
			" the body and optional meshes for the trigger, the slide and magazine. If the model isn't split into these seperate meshes " +
			"you can still make a working gun, it just won't have any moveable parts. See the example weapons for reference.", MessageType.Info);
	
		GUILayout.Label("Assigning models", EditorStyles.boldLabel);
		gunModel = (GameObject)EditorGUILayout.ObjectField("Gun model", gunModel, typeof(GameObject), true);
		gunMesh = (GameObject)EditorGUILayout.ObjectField("Gun mesh", gunMesh, typeof(GameObject), true);
		if (gunMesh != null)
		{
			Renderer gunMeshRenderer = gunMesh.GetComponentInChildren<Renderer>();
			if (gunMeshRenderer == null)
				EditorGUILayout.HelpBox("Gun mesh should contain a Renderer", MessageType.Warning);
		}
		slideMesh = (GameObject)EditorGUILayout.ObjectField("Slide mesh", slideMesh, typeof(GameObject), true);
		if (slideMesh != null)
		{
			Renderer slideMeshRenderer = slideMesh.GetComponentInChildren<Renderer>();
			if (slideMeshRenderer == null)
				EditorGUILayout.HelpBox("Slide mesh should contain a Renderer", MessageType.Warning);
		}
		triggerMesh = (GameObject)EditorGUILayout.ObjectField("Trigger mesh", triggerMesh, typeof(GameObject), true);
		if (triggerMesh != null)
		{
			Renderer triggerMeshRenderer = triggerMesh.GetComponentInChildren<Renderer>();
			if (triggerMeshRenderer == null)
				EditorGUILayout.HelpBox("Trigger mesh should contain a Renderer", MessageType.Warning);
		}
		var oldMagMesh = magMesh;
		magMesh = (GameObject)EditorGUILayout.ObjectField("Mag mesh", magMesh, typeof(GameObject), true);
		if (magMesh != null)
		{
			Renderer magMeshRenderer = magMesh.GetComponentInChildren<Renderer>();
			if (magMeshRenderer == null)
				EditorGUILayout.HelpBox("Magazine mesh should contain a Renderer", MessageType.Warning);
			if (oldMagMesh == null)
			{
				magazinePrefabPath = "Assets/"+magMesh.name+".prefab";
				magazinePathValid = true;
			}
			GUILayout.BeginHorizontal();
			var oldPath = magazinePrefabPath;
			magazinePrefabPath = EditorGUILayout.TextField(magazinePrefabPath);
			if (oldPath != magazinePrefabPath)
			{
				if (magazinePrefabPath.Contains("Assets") && magazinePrefabPath.Contains(".prefab"))
					magazinePathValid = true;
				else
					magazinePathValid = false;
			}
			if (GUILayout.Button("..."))
			{
				oldPath = magazinePrefabPath;
				magazinePrefabPath = EditorUtility.OpenFolderPanel("Magazine Prefab Save Location", "", "");
				if (oldPath != magazinePrefabPath)
				{
					magazinePrefabPath += "/"+magMesh.name+".prefab";

					if (!magazinePrefabPath.Contains("Assets"))
						magazinePathValid = false;
					else
					{
						int assetsIndex = magazinePrefabPath.IndexOf("Assets");
						magazinePrefabPath = magazinePrefabPath.Remove(0, assetsIndex);
						magazinePathValid = true;
					}
				}
			}
			GUILayout.EndHorizontal();
			magazineId = EditorGUILayout.IntField("Magazine Id", magazineId);
			if (!magazinePathValid)
			{
				EditorGUILayout.HelpBox("Path not valid", MessageType.Error);
			}
		}
		if (gunModel != null && gunMesh != null && (magMesh == null || (magMesh != null && magazinePathValid)))
		{
			if (GUILayout.Button("Setup"))
				CompleteSetup();
		}
	}

	private void CompleteSetup()
	{
		if (gunMesh == gunModel)
		{
			gunModel = new GameObject("GunModel");
			gunModel.transform.position = gunMesh.transform.position;
			gunModel.transform.rotation = gunMesh.transform.rotation;
			gunModel.transform.localScale = Vector3.one;
			gunMesh.transform.SetParent(gunModel.transform);
		}

		if (gunModel.GetComponent<Rigidbody>() != null)
			Undo.DestroyObjectImmediate(gunModel.GetComponent<Rigidbody>());
		if (gunModel.GetComponent<Collider>() != null)
			Undo.DestroyObjectImmediate(gunModel.GetComponent<Collider>());
		Rigidbody gunBody = Undo.AddComponent<Rigidbody>(gunModel);
		gunBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		BoxCollider gunModelCollider = Undo.AddComponent<BoxCollider>(gunModel);

		//Add gun handler script
		if (gunMesh.GetComponent<VRGunHandler>() != null) Undo.DestroyObjectImmediate(gunMesh.GetComponent<VRGunHandler>());
		VRGunHandler gunHandler = Undo.AddComponent<VRGunHandler>(gunMesh);
		gunHandler.item = gunModel.transform;
		gunHandler.hover = gunMesh.GetComponentInChildren<Renderer>();
		gunHandler.defaultPosition = gunHandler.transform.localPosition;
		gunHandler.defaultRotation = gunHandler.transform.localRotation;

		VRGunHandlerRef gunHandlerRef = Undo.AddComponent<VRGunHandlerRef>(gunModel);
		gunHandlerRef.gunHandler = gunHandler;

		if (gunMesh.GetComponent<Collider>() != null) DestroyImmediate(gunMesh.GetComponent<Collider>());
		Renderer gunMeshRenderer = gunMesh.GetComponentInChildren<Renderer>();
		BoxCollider gunMeshCollider = null;
		if (gunMeshRenderer != null)
		{
			BoxCollider gunMeshRendererCollider = Undo.AddComponent<BoxCollider>(gunMeshRenderer.gameObject);
			if (gunMeshRendererCollider.gameObject == gunMesh)
			{
				gunMeshCollider = gunMeshRendererCollider;
			} else
			{
				gunMeshCollider = ResizedCollider(gunMesh, gunMeshRendererCollider);
				Undo.DestroyObjectImmediate(gunMeshRendererCollider);
			}
		} else
		{
			Debug.LogWarning("Couldn't find mesh renderer on Gun Mesh. Box collider will likely need to be resized");
			gunMeshCollider = Undo.AddComponent<BoxCollider>(gunMesh);
		}

		gunMeshCollider.isTrigger = true;
		gunModelCollider.center = gunMesh.transform.localRotation * Vector3.Scale(gunMeshCollider.center, gunMesh.transform.localScale);
		Vector3 gunModelSize = gunMesh.transform.localRotation * Vector3.Scale(gunMeshCollider.size, gunMesh.transform.localScale);
		if (gunModelSize.x < 0) gunModelSize.x = -gunModelSize.x;
		if (gunModelSize.y < 0) gunModelSize.y = -gunModelSize.y;
		if (gunModelSize.z < 0) gunModelSize.z = -gunModelSize.z;
		gunModelCollider.size = gunModelSize;
		if (gunMesh.GetComponent<AudioSource>() != null) Undo.DestroyObjectImmediate(gunMesh.GetComponent<AudioSource>());
		Undo.AddComponent<AudioSource>(gunMesh);

		if (triggerMesh != null)
		{
			if (triggerMesh.GetComponent<VRGunTrigger>() != null) Undo.DestroyObjectImmediate(triggerMesh.GetComponent<VRGunTrigger>());
			VRGunTrigger trigger = Undo.AddComponent<VRGunTrigger>(triggerMesh);
			gunHandler.trigger = trigger;
			trigger.gunHandler = gunHandler;
			trigger.defaultTriggerPosition = trigger.pulledTriggerPosition = trigger.transform.localPosition;
			trigger.defaultTriggerRotation = trigger.pulledTriggerRotation = trigger.transform.localRotation;
		}

		if (slideMesh != null)
		{
			//Add slide script
			if (slideMesh.GetComponent<VRGunSlide>() != null) Undo.DestroyObjectImmediate(slideMesh.GetComponent<VRGunSlide>());
			VRGunSlide gunSlide = Undo.AddComponent<VRGunSlide>(slideMesh);
			gunHandler.slide = gunSlide;

			Renderer gunSlideMeshRenderer = slideMesh.GetComponentInChildren<Renderer>();
			BoxCollider gunSlideCollider = null;
			if (gunSlideMeshRenderer != null)
			{
				BoxCollider slideGunMeshRendererCollider = Undo.AddComponent<BoxCollider>(gunSlideMeshRenderer.gameObject);
				if (slideGunMeshRendererCollider.gameObject == slideMesh)
				{
					gunSlideCollider = slideGunMeshRendererCollider;
				} else
				{
					gunSlideCollider = ResizedCollider(slideMesh, slideGunMeshRendererCollider);
					Undo.DestroyObjectImmediate(slideGunMeshRendererCollider);
				}
			} else
			{
				Debug.LogWarning("Couldn't find mesh renderer on slide mesh. Box collider will likely need to be resized");
				gunSlideCollider = Undo.AddComponent<BoxCollider>(slideMesh);
			}

			gunSlideCollider.isTrigger = true;
			gunSlide.item = gunSlide.transform;
			gunSlide.parentItem = gunHandler;
			gunSlide.gunHandler = gunHandler;
			gunSlide.hover = slideMesh.GetComponentInChildren<Renderer>();
			gunSlide.defaultPosition = gunSlide.pulledPosition = gunSlide.transform.localPosition;
			gunSlide.defaultRotation = gunSlide.transform.localRotation;
		}

		if (magMesh != null)
		{
			//Add magazine script
			if (magMesh.GetComponent<Rigidbody>() != null) Undo.DestroyObjectImmediate(magMesh.GetComponent<Rigidbody>());
			if (magMesh.GetComponent<Collider>() != null) Undo.DestroyObjectImmediate(magMesh.GetComponent<Collider>());
			Rigidbody magBody = Undo.AddComponent<Rigidbody>(magMesh);
			magBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

			Renderer magMeshRenderer = magMesh.GetComponentInChildren<Renderer>();
			BoxCollider magMeshCollider = null;
			if (magMeshRenderer != null)
			{
				BoxCollider magMeshRendererCollider = Undo.AddComponent<BoxCollider>(magMeshRenderer.gameObject);
				if (magMeshRendererCollider.gameObject == magMesh)
				{
					magMeshCollider = magMeshRendererCollider;
				} else
				{
					magMeshCollider = ResizedCollider(magMesh, magMeshRendererCollider);
					Undo.DestroyObjectImmediate(magMeshRendererCollider);
				}
			} else
			{
				Debug.LogWarning("Couldn't find mesh renderer on magazine mesh. Box collider will likely need to be resized");
				magMeshCollider = Undo.AddComponent<BoxCollider>(magMesh);
			}

			VRMagazine oldAmmo = magMesh.GetComponentInChildren<VRMagazine>();
			GameObject magScript = null;
			if (oldAmmo != null)
			{
				magScript = oldAmmo.gameObject;
				Undo.DestroyObjectImmediate(oldAmmo);
			} else
			{
				magScript = new GameObject("Mag Script");
				Undo.RegisterCreatedObjectUndo(magScript, "Setup Weapon");
			}
			magScript.transform.SetParent(magMesh.transform);
			magScript.transform.localPosition = Vector3.zero;
			magScript.transform.localRotation = Quaternion.identity;
			magScript.transform.localScale = Vector3.one;
			VRMagazine ammo = magScript.AddComponent<VRMagazine>();
			ammo.item = magMesh.transform;
			ammo.hover = magMesh.GetComponentInChildren<Renderer>();
			ammo.integrated = false;
			ammo.defaultLoadedPosition = ammo.entryPosition = ammo.item.localPosition;
			ammo.defaultRotation = ammo.item.localRotation;
			ammo.magazineId = magazineId;
			if (magScript.GetComponent<BoxCollider>() != null) Undo.DestroyObjectImmediate(magScript.GetComponent<BoxCollider>());
			BoxCollider magScriptCollider = magScript.AddComponent<BoxCollider>();
			magScriptCollider.size = magMeshCollider.size;
			magScriptCollider.center = magMeshCollider.center;
			magScriptCollider.isTrigger = true;

			gunHandler.magazinePrefab = PrefabUtility.CreatePrefab(magazinePrefabPath, magMesh);
			gunHandler.startLoaded = true;
			gunHandler.magazineId = magazineId;
			DestroyImmediate(magMesh);
		} else
		{
			GameObject magObject = new GameObject("Integrated Magazine");
			magObject.transform.SetParent(gunModel.transform);
			magObject.transform.localPosition = Vector3.zero;
			magObject.transform.localRotation = Quaternion.identity;
			VRMagazine ammo = Undo.AddComponent<VRMagazine>(magObject);
			ammo.item = gunModel.transform;
			ammo.integrated = true;
			ammo.magazineId = magazineId;
			gunHandler.integratedMagazine = ammo;
			gunHandler.magazineId = magazineId;
		}

		GunHandlerWindow newWindow = (GunHandlerWindow)EditorWindow.GetWindow(typeof(GunHandlerWindow), true, "Gun Handler", true);
		newWindow.gunHandler = gunHandler;
		newWindow.Init();
		newWindow.weaponTab = GunHandlerWindow.WeaponTab.MAIN;

		Close();
	}

	static public BoxCollider ResizedCollider(GameObject targetObject, BoxCollider colliderReference)
	{
		BoxCollider newCollider = Undo.AddComponent<BoxCollider>(targetObject);
		newCollider.center = (colliderReference.transform.localRotation * Vector3.Scale(colliderReference.center, colliderReference.transform.localScale)) + colliderReference.transform.localPosition;
		Vector3 newSize = colliderReference.transform.localRotation * Vector3.Scale(colliderReference.size, colliderReference.transform.localScale);
		if (newSize.x < 0) newSize.x = -newSize.x;
		if (newSize.y < 0) newSize.y = -newSize.y;
		if (newSize.z < 0) newSize.z = -newSize.z;
		newCollider.size = newSize;
		return newCollider;
	}
}
