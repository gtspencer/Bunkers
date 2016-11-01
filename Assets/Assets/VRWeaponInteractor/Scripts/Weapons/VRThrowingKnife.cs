using UnityEngine;
using System.Collections;

public class VRThrowingKnife : VRInteractableItem 
{
	public bool guided = true;
	public string knifeLayer;
	bool beingGuided;

	override public void Drop(SteamVR_TrackedObject trackedObj)
	{
		base.Drop(trackedObj);

		if (!guided) return;

		var device = SteamVR_Controller.Input((int)trackedObj.index);
		var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
		Vector3 initVel;
		if (origin != null)
			initVel = origin.TransformVector(device.velocity);
		else
			initVel = device.velocity;

		if (initVel.magnitude < 2) return;

		StartCoroutine(GuideToTarget(initVel));
	}

	IEnumerator GuideToTarget(Vector3 initVel)
	{
		if (beingGuided) yield break;
		beingGuided = true;

		selfBody.useGravity = false;
		selfBody.constraints = RigidbodyConstraints.FreezePosition;

		Vector3 target;
		int layerInt = LayerMask.NameToLayer(knifeLayer);
		LayerMask knifeLayerMask = 1 << layerInt;

		float angle = Vector3.Angle(Camera.main.transform.forward, initVel);
		Ray ray;
		if (angle > 50)
			ray = new Ray(item.position, initVel);
		else
			ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 10, knifeLayerMask))
			target = hit.point;
		else
			target = ray.origin + (ray.direction*10);

		float dist = Vector3.Distance(item.position, target);
		float speed = 10;
		Vector3 currentVel = initVel;
		Vector3 lastPos = item.position;
		while(dist > 0.5f)
		{
			if (!beingGuided) yield break;
			float stepSpeed = speed * Time.deltaTime;
			currentVel = item.position - lastPos;
			item.position = Vector3.MoveTowards(item.position, target, stepSpeed);
			lastPos = item.position;

			ray = new Ray(item.position, item.position - lastPos);
			if (Physics.Raycast(ray, out hit, 10, knifeLayerMask))
				target = hit.point;

			dist = Vector3.Distance(item.position, target);

			yield return null;
		}

		selfBody.useGravity = true;
		selfBody.constraints = RigidbodyConstraints.None;
		selfBody.velocity = currentVel*5;
		beingGuided = false;
	}

	public void OnTrigger(CollisionForwarder.Info info)
	{
		if (info == null) 
		{
			Debug.LogWarning("info is null");
			return;
		}
		if (info.collider != null)
		{
			if (info.collisionType == CollisionForwarder.Info.CollisionType.ENTER &&
				info.collider.transform.GetComponent<KnifeCollider>() != null &&
				heldBy == null)
			{
				beingGuided = false;
				FreezeItem(item.gameObject, true);
			}
		} else if (info.collision != null)
		{
			if (info.collisionType == CollisionForwarder.Info.CollisionType.ENTER &&
				heldBy == null && beingGuided)
			{
				beingGuided = false;
				UnFreezeItem(item.gameObject);
			}
		}
	}
}
