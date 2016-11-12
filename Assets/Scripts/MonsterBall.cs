using UnityEngine;
using System.Collections;

public class MonsterBall : MonoBehaviour {

    private Vector3 spawnPoint;
    public GameObject enemy;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        //spawnPoint = collision.ContactPoint;
        //Instantiate(enemy, spawnPoint, Quaternion.identity);
    }
}
