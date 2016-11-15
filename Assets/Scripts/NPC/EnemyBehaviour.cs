using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using Valve.VR;

public class EnemyBehaviour : MonoBehaviour {
    private GameObject target;
    private Vector3 targetPos;
    private WallBehaviour wallBehaviour;
    public GameObject collisionObject;
    public bool attackMode;
    private Animation animation;


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
        animation = GetComponent<Animation>();
        animation.CrossFade("Devil_Dog_Run");
    }

    void FixedUpdate() {
        if (transform.position.y <= -1)
        {
            Destroy(gameObject);
        }
        targetPos = target.transform.position;
        transform.LookAt(target.transform);

        if (attackMode)
        {
            StartCoroutine("Attack");
        } else if (hp <= 0)
        {
            StartCoroutine("Die");
        } else
        {
            //GetComponent<Renderer>().material.color = defaultColor;
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
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
        animation.CrossFade("Devil_Dog_Attack01", .2f);
        attackMode = false;
        //GetComponent<Renderer>().material.color = Color.red;
        wallBehaviour.ProcessDamage(attackPoints);
        yield return new WaitForSeconds(attackCooldown);
        attackMode = true;
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

    public void ProcessDamage(int attackPoints)
    {
        hp -= attackPoints;
        if (hp <= 0)
        {
            Die();
        }
    }

    public IEnumerator Die()
    {
        animation.CrossFade("Devil_Dog_Death", .3f);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}