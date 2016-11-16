using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using Valve.VR;

public class FriendlyHUDBehaviour : MonoBehaviour
{
    private TextMesh textMesh;
    public GameObject friendly;
    private FriendlyBehaviour friendlyBehaviour;

    //Use this for initialization
    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        friendlyBehaviour = friendly.GetComponent<FriendlyBehaviour>();
    }

    void FixedUpdate()
    {
        //textMesh.transform.LookAt(Camera.main.transform.position);
        int hp = friendlyBehaviour.getHitPoints();
        if (hp <= 0)
        {
            textMesh.text = "HP: 0";
        }
        else
        {
            textMesh.text = "HP: " + hp;
        }
    }
}