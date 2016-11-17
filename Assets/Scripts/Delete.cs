using UnityEngine;
using System.Collections;

public class Delete : MonoBehaviour {

    public int deleteAfterSeconds;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        StartCoroutine(EraseForeverAndEver());
	}

    /**
    IEnumerator OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Groud")
        {
            yield return new WaitForSeconds(deleteAfterSeconds);
            if (!gameObject.active)
            {
                Destroy(gameObject);
            }
        }
    }
    */

    IEnumerator EraseForeverAndEver()
    {
        yield return new WaitForSeconds(deleteAfterSeconds);
        Destroy(gameObject);
    }
}
