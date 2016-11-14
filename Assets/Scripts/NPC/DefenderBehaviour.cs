using UnityEngine;
using System.Collections;

public class DefenderBehaviour : MonoBehaviour
{
    //path around bunker to defend
    public Transform pointA;
    public Transform pointB;
    public Transform pointC;
    public Transform pointD;

    private GameObject target;
    private Vector3 targetPos;


    public GameObject collisionObject;
    public bool attackMode;


    public float speed = 0.1f;
    public int hp = 100;
    public float attackCooldown = 1.0f;
    public int attackPoints = 1;
    public float killThreshhold = 1.5f;
    public Color defaultColor;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
    
    }
}
