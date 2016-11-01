using UnityEngine;
using System.Collections;

public class VRLoadableBullet : VRInteractableItem 
{
	public int bulletId;

	void OnTriggerEnter(Collider col)
	{
		if (!enabled) return;
		VRBulletReceiver bulletReceiver = col.GetComponent<VRBulletReceiver>();
		if (bulletReceiver != null)
		{
			if (bulletReceiver.gunHandler != null)
				bulletReceiver.gunHandler.LoadBullet(this);
			else if (bulletReceiver.magazine != null)
				bulletReceiver.magazine.LoadBullet(this);
		}
	}
}
