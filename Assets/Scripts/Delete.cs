using UnityEngine;
using System.Collections.Generic;
using MovementEffects;

public class Delete : MonoBehaviour {

    public float deleteAfterSeconds;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Timing.RunCoroutine(EraseForeverAndEver());
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

    IEnumerator<float> EraseForeverAndEver()
    {
        yield return Timing.WaitForSeconds(deleteAfterSeconds);
        Destroy(gameObject);
    }
}
