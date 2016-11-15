using UnityEngine;
using System.Collections;

public class Delete : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Groud")
        {
            yield return new WaitForSeconds(7);
            if (!gameObject.active)
            {
                Destroy(gameObject);
            }
        }
    }
}
