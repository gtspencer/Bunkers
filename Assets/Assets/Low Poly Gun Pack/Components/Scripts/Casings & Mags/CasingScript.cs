using UnityEngine;
using System.Collections;

public class CasingScript : MonoBehaviour {

	[Header("Only used for the sawn off shotgun casing")]
	public bool SawnOffShotgunCasing;

	[Header("Force X")]
	public float minimumXForce;					
	public float maximumXForce;
	[Header("Force Y")]
	public float minimumYForce;
	public float maximumYForce;
	[Header("Rotation Force")]
	public float minimumRotation;
	public float maximumRotation;
	[Header("Despawn Time")]
	public float despawnTime;

	[Header("Audio")]
	public AudioClip[] casingSounds;
	public AudioSource audioSource;

	//Launch the casing at start
	void Awake () {
		//Random rotation of the casing
		GetComponent<Rigidbody>().AddRelativeTorque (
				Random.Range(minimumRotation, maximumRotation), //X Axis
				Random.Range(minimumRotation, maximumRotation), //Y Axis
			    Random.Range(minimumRotation, maximumRotation)  //Z Axis
				* Time.deltaTime);

		if (!SawnOffShotgunCasing) {
			//Random direction the casing will be ejected in
			GetComponent<Rigidbody>().AddRelativeForce (
				 Random.Range (minimumXForce, maximumXForce), //X Axis
	             Random.Range (minimumYForce, maximumYForce), //Y Axis
				 Random.Range (0, 0)); 						  //Z Axis

		} else {
			//Only for the sawn off shotgun casing
			GetComponent<Rigidbody>().AddRelativeForce (
				Random.Range (minimumXForce, maximumXForce), 
				Random.Range (minimumYForce, maximumYForce), 
				//Throws the casing backwards/on the z axis
				Random.Range (-40, -80)); 				     
		}
	}

	void Start () {
		//Start the remove coroutine
		StartCoroutine (RemoveCasing ());
	}

	void OnCollisionEnter (Collision collision) {

		//Get a random casing sound from the array every collision
		audioSource.clip = casingSounds
			[Random.Range(0, casingSounds.Length)];
	    //Play the random casing sound
	    audioSource.Play();
	}

	IEnumerator RemoveCasing () {
		//Destroy the casing after set amount of seconds
		yield return new WaitForSeconds (despawnTime);
		Destroy (gameObject);
	}
}