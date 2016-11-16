using UnityEngine;
using System.Collections;

public class RoofBehaviour : MonoBehaviour {

    public GameObject wall1;
    public GameObject wall2;
    public GameObject wall3;
    public GameObject wall4;
	
	// Update is called once per frame
	void FixedUpdate () {
	    if (wall1 == null && wall2 == null && wall3 == null && wall4 == null)
        {
            Destroy(gameObject);
        }
	}
}
