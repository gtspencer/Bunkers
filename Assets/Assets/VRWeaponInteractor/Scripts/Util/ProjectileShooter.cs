using UnityEngine;
using System.Collections;

public class ProjectileShooter : MonoBehaviour {

	public GameObject projectilePrefab;

	public void Activate()
	{
		GameObject projectileInstance = (GameObject)Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity);
		projectileInstance.transform.position = transform.position+(transform.forward*0.1f);
		Mover projectile = projectileInstance.GetComponent<Mover>();
		if (projectile != null) projectile.direction = transform.forward;
	}
}
