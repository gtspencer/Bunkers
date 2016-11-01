using UnityEngine;
using System.Collections;

public class CollisionForwarder : MonoBehaviour {

	public class Info
	{
		public enum CollisionType
		{
			ENTER,
			STAY,
			EXIT
		}

		public string objectName;
		public CollisionType collisionType;
		public Collision collision;
		public Collider collider;
	}

	public GameObject target;
	public string methodName;
	public bool disableEnter;
	public bool disableStay;
	public bool disableExit;

	void OnCollisionEnter(Collision collision)
	{
		if (disableEnter) return;
		OnCollision(collision, Info.CollisionType.ENTER);
	}

	void OnCollisionStay(Collision collision)
	{
		if (disableStay) return;
		OnCollision(collision, Info.CollisionType.STAY);
	}

	void OnCollisionExit(Collision collision)
	{
		if (disableExit) return;
		OnCollision(collision, Info.CollisionType.EXIT);
	}

	void OnCollision(Collision collision, Info.CollisionType collisionType)
	{
		Info newInfo = new Info();
		newInfo.objectName = name;
		newInfo.collisionType = collisionType;
		newInfo.collision = collision;
		target.SendMessage(methodName, newInfo, SendMessageOptions.DontRequireReceiver);
	}

	void OnTriggerEnter(Collider collider)
	{
		if (disableEnter) return;
		OnTrigger(collider, Info.CollisionType.ENTER);
	}

	void OnTriggerStay(Collider collider)
	{
		if (disableStay) return;
		OnTrigger(collider, Info.CollisionType.STAY);
	}

	void OnTriggerExit(Collider collider)
	{
		if (disableExit) return;
		OnTrigger(collider, Info.CollisionType.EXIT);
	}

	void OnTrigger(Collider collider, Info.CollisionType triggerType)
	{
		Info newInfo = new Info();
		newInfo.objectName = name;
		newInfo.collisionType = triggerType;
		newInfo.collider = collider;
		target.SendMessage(methodName, newInfo, SendMessageOptions.DontRequireReceiver);
	}
}
