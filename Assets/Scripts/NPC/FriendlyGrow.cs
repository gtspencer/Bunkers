using UnityEngine;
using System.Collections;

public class FriendlyGrow : MonoBehaviour
{
    //Legacy code -- integrated into FriendlyBehavoiur now
    public float scaleUp = .1f;
    public float maxScale = 1.3f;
    public float minScale = .1f;
    private bool beginScale;

    // Use this for initialization
    void Start()
    {
        beginScale = false;
        Vector3 scale = new Vector3(minScale, minScale);
        //scale.x = .1;
        //scale.y = .1;
        scale.z = minScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (beginScale)
        {
            Vector3 scale = gameObject.transform.localScale;
            scale.x += scaleUp;
            scale.y += scaleUp;
            scale.z += scaleUp;
            gameObject.transform.localScale = scale;
        }
    }

    void OnCollisionTrigger(Collider collider)
    {
        if (collider.gameObject.tag == "Ground")
        {
            beginScale = true;
        }
    } 
}
