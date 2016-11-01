using UnityEngine;
using System.Collections;

public class GrenadeScript : MonoBehaviour {

	[Header("Timer")]
	//Time before the grenade explodes
	public float grenadeTimer = 5.0f;

	[Header("Explosion Prefabs")]
	//All explosion prefabs
	public Transform explosionMetalPrefab;
	public Transform explosionConcretePrefab;
	public Transform explosionDirtPrefab;
	public Transform explosionWoodPrefab;
	
	[Header("Impact Tags")]
	//Default impact tags
	public string metalImpactStaticTag = "Metal (Static)";
	public string metalImpactTag = "Metal";
	public string woodImpactStaticTag = "Wood (Static)";
	public string woodImpactTag = "Wood";
	public string concreteImpactStaticTag = "Concrete (Static)";
	public string concreteImpactTag = "Concrete";
	public string dirtImpactStaticTag = "Dirt (Static)";
	public string dirtImpactTag = "Dirt";

	[Header("Explosion Options")]
	//Radius of the explosion
	public float radius = 25.0F;
	//Intensity of the explosion
	public float power = 350.0F;

	[Header("Audio")]
	public AudioSource impactSound;

	//Used to check what surface the 
	//grenade is exploding on
	string groundTag;

	void Start () {
		//Start the explosion timer
		StartCoroutine (ExplosionTimer ());
	}

	void OnCollisionEnter (Collision collision) {
		//Play the impact sound on every collision
		impactSound.Play ();
	}

	IEnumerator ExplosionTimer () {
		//Wait set amount of time
		yield return new WaitForSeconds(grenadeTimer);

		//Raycast downwards to check the ground tag
		RaycastHit checkGround;
		if (Physics.Raycast(transform.position, Vector3.down, out checkGround, 50))
		{
			//Set the ground tag to whatever the raycast hit
			groundTag = checkGround.collider.tag;
		}

		//If ground tag is Metal or Metal(Static)
		if (groundTag == metalImpactTag || groundTag == metalImpactStaticTag)
		{
			//Instantiate metal explosion prefab
			Instantiate (explosionMetalPrefab, checkGround.point, 
			             Quaternion.FromToRotation (Vector3.forward, checkGround.normal)); 
		}
		
		//If ground tag is Concrete or Concrete(Static)
		if (groundTag == concreteImpactTag || groundTag == concreteImpactStaticTag)
		{
			//Instantiate concrete explosion prefab
			Instantiate (explosionConcretePrefab, checkGround.point, 
			             Quaternion.FromToRotation (Vector3.forward, checkGround.normal)); 
		}
		
		//If ground tag is Wood or Wood(Static)
		if (groundTag == woodImpactTag || groundTag == woodImpactStaticTag)
		{
			//Instantiate wood explosion prefab
			Instantiate (explosionWoodPrefab, checkGround.point, 
			             Quaternion.FromToRotation (Vector3.forward, checkGround.normal)); 
		}
		
		//If ground tag is Dirt or Dirt(Static)
		if (groundTag == dirtImpactTag || groundTag == dirtImpactStaticTag)
		{
			//Instantiate dirt explosion prefab
			Instantiate (explosionDirtPrefab, checkGround.point, 
			             Quaternion.FromToRotation (Vector3.forward, checkGround.normal)); 
		}

		//Explosion force
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		foreach (Collider hit in colliders) {
			Rigidbody rb = hit.GetComponent<Rigidbody> ();

			//Add force to nearby rigidbodies
			if (rb != null)
				rb.AddExplosionForce (power, explosionPos, radius, 3.0F);
			
			//********** USED IN THE DEMO SCENES **********
			//If the explosion hit the tags "Target", and if "isHit" 
			//is false on the target
			if (hit.GetComponent<Collider>().tag == "Target" 
			    	&& hit.gameObject.GetComponent<TargetScript>().isHit == false) {
				
				//Animate the target 
				hit.gameObject.GetComponent<Animation> ().Play("target_down");
				//Toggle the isHit bool on the target
				hit.gameObject.GetComponent<TargetScript>().isHit = true;
			}
		}

		//Destroy the grenade on explosion
		Destroy (gameObject);
	}
}