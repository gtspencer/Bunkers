using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using Valve.VR;

public class EnemyBehaviour : MonoBehaviour {
    private GameObject target;
    private Vector3 targetPos;

    private WallBehaviour wallBehaviour;
    private GameObject collisionObject;

    public float speed = 0.1f;
    public int hp = 100;
    public float attackCooldownLow;
    public float attackCooldownHigh;
    public int attackPoints = 1;
    public float killThreshhold = 1.5f;
    private bool attackMode;

    //Use this for initialization
    void Start() {
        target = GameObject.FindGameObjectWithTag("MainCamera");
        targetPos = target.transform.position;
    }

    void FixedUpdate() {
        target = GameObject.FindGameObjectWithTag("MainCamera");
        targetPos = target.transform.position;
        transform.LookAt(target.transform);
        Move();
    }

    void OnCollisionEnter(Collision collision)
    {
        collisionObject = collision.gameObject;
        if (collisionObject.name == "SmallWall" || collisionObject.name == "LargeWall") {
            wallBehaviour = collisionObject.GetComponent<WallBehaviour>();
            attackMode = true;
        }
    }

    void Move()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
    }

    void Attack()
    {
        while (attackMode) 
        {
            GetComponent<Renderer>().material.color = Color.red;
            wallBehaviour.ProcessDamage(attackPoints);
            //add a timer here
        }
    }

    void ProcessDamage(int attackPoints)
    {
        hp -= attackPoints;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    public int getHitPoints()
    {
        return hp;
    }

    public void Damage(DamageInfo info)
    {
        if (hp <= 0) { return; }
        hp -= info.damage;
        if (hp <= 0) { Destroy(gameObject); } else { FixedUpdate(); }
    }
}