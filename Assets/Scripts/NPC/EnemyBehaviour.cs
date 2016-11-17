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
    private bool dead;
    private Animation animation;
    private FriendlyBehaviour friendlyBehaviour;
    private int whichAttack;


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
        dead = false;
    }

    void FixedUpdate() {
        if (transform.position.y <= -1)
        {
            Destroy(gameObject);
        }
        targetPos = target.transform.position;
        transform.LookAt(targetPos);

        if (collisionObject == null)
        {
            attackMode = false;
        }

        if (attackMode)
        {
            StartCoroutine(Attack());
        } else if (hp <= 0)
        {
            dead = true;
            StartCoroutine(Die());
        } else
        {
            if (!dead)
            {
                attackMode = false;
                //GetComponent<Renderer>().material.color = defaultColor;
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        collisionObject = collision.gameObject;
        if (collision.gameObject.tag == "Bunker") {
            wallBehaviour = collision.gameObject.GetComponent<WallBehaviour>();
            attackMode = true;
            whichAttack = 0;
        }
        if (collision.gameObject.tag == "GoodGuy")
        {
            friendlyBehaviour = collision.gameObject.GetComponent<FriendlyBehaviour>();
            attackMode = true;
            whichAttack = 1;
        }
    }


    public IEnumerator Attack()
    {
        animation.CrossFade("Devil_Dog_Attack01", .2f);
        attackMode = false;
        //GetComponent<Renderer>().material.color = Color.red;
        switch (whichAttack)
        {
            case 0:
                wallBehaviour.ProcessDamage(attackPoints);
                break;
            case 1:
                friendlyBehaviour.ProcessDamage(attackPoints);
                break;
            default:
                break;
        }
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
            attackMode = false;
        }
    }

    public void Damage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            attackMode = false;
        }
    }

    public void ProcessDamage(int attackPoints)
    {
        hp -= attackPoints;
        if (hp <= 0)
        {
            attackMode = false;
        }
    }

    public IEnumerator Die()
    {
        attackMode = false;
        StopCoroutine(Attack());
        animation.CrossFade("Devil_Dog_Death", .3f);
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}