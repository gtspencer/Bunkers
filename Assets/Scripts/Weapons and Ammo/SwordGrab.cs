using UnityEngine;
using System.Collections;

public class SwordGrab : MonoBehaviour
{
    public GameObject sword;

    // Use this for initialization
    void Start()
    {
        sword = gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter()
    {
        //sword.transform.SetParent();
    }
}
