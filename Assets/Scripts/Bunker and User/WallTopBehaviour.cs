using UnityEngine;
using System.Collections;

public class WallTopBehaviour : MonoBehaviour {

    public GameObject wallBottom;
	
	// Update is called once per frame
	void FixedUpdate () {
	    if (wallBottom == null)
        {
            Destroy(gameObject);
        }
	}
}
