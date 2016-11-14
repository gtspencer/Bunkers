using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using Valve.VR;

public class EnemyBehaviour : MonoBehaviour {
    private GameObject target;
    private Vector3 targetPos;
    private WallBehaviour wallBehaviour;
    private GameObject collisionObject;
    private bool attackMode;


    public float speed = 0.1f;
    public int hp = 100;
    public float attackCooldown = 1.0f;
    public int attackPoints = 1;
    public float killThreshhold = 1.5f;
    public Color defaultColor;

    //Use this for initialization
    void Start() {
        target = GameObject.FindGameObjectWithTag("MainCamera");
        targetPos = target.transform.position;
        attackMode = false;
    }

    void FixedUpdate() {
        targetPos = target.transform.position;
        transform.LookAt(target.transform);
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        if (attackMode)
        {
            StartCoroutine("Attack");
        }
        else
        {
            GetComponent<Renderer>().material.color = defaultColor;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        collisionObject = collision.gameObject;
        if (collision.gameObject.tag == "Bunker") {
            wallBehaviour = collision.gameObject.GetComponent<WallBehaviour>();
            attackMode = true;
        }
        if (collision.gameObject.tag == "Defender")
        {

        }
    }


    public IEnumerator Attack()
    {
        attackMode = false;
        GetComponent<Renderer>().material.color = Color.red;
        wallBehaviour.ProcessDamage(attackPoints);
        yield return new WaitForSeconds(attackCooldown);
        attackMode = true;
    }

    void ProcessDamage(int attackPoints)
    {
        hp -= attackPoints;
        if (hp <= 0)
        {
            Die();
        }
    }

    public int getHitPoints()
    {
        return hp;
    }

    public void Damage(DamageInfo info)
    {
        hp -= info.damage;
        if (hp <= 0) {
            Die();
        }
    }

    public void Damage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}