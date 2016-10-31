using UnityEngine;
using System.Collections;

public class MagazineScript : MonoBehaviour {

	[Header("Customizable Options")]
	//How long before the prefab is destroyed
	public float despawnTimer = 15.0f;
	public float ejectForce;

	[Header("Audio")]
	public AudioSource impactSound;
	
	void Awake () {
		//Eject magazine downwards with set amount of force
		//Useful so the mag doesnt get stuck inside the gun
		GetComponent<Rigidbody>().AddRelativeForce (0,ejectForce,0); 
	}

	void Start () {
		//Start the remove coroutine
		StartCoroutine(DespawnTimer());
	}

	void OnCollisionEnter (Collision collision) {
		//Play the impact sound on every collision
		impactSound.Play ();
	}

	IEnumerator DespawnTimer () {
		//Destroy the magazine after set amount of time	
		yield return new WaitForSeconds(despawnTimer);
		Destroy (gameObject);
	}
}