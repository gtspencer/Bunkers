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
    public float attackCooldown;
    public int attackPoints = 1;
    public float killThreshhold = 1.5f;
    private bool attackMode;
    public Color defaultColor;

    //Use this for initialization
    void Start() {
        target = GameObject.FindGameObjectWithTag("MainCamera");
        targetPos = target.transform.position;
    }

    void Update() {
        target = GameObject.FindGameObjectWithTag("MainCamera");
        targetPos = target.transform.position;
        transform.LookAt(target.transform);
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
        if (attackMode)
        {
            attemptAttack();
        } else
        {
            GetComponent<Renderer>().material.color = defaultColor;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        collisionObject = collision.gameObject;
        if (collisionObject.name == "SmallWall" || collisionObject.name == "LargeWall") {
            wallBehaviour = collisionObject.GetComponent<WallBehaviour>();
            attackMode = true;
        }
        StartCoroutine("Update");
    }

    void Move()
    {

    }

    void Attack()
    {
        GetComponent<Renderer>().material.color = Color.red;
        wallBehaviour.ProcessDamage(attackPoints);
    }

    public IEnumerator attemptAttack()
    {
        attackMode = false;
        StartCoroutine("Attack");
        yield return new WaitForSeconds(attackCooldown);
        attackMode = true;
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
        hp -= info.damage;
        if (hp <= 0) { Destroy(gameObject); } else { StartCoroutine("FixedUpdate"); }
    }
}