using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using Valve.VR;

public class FriendlyBehaviour : MonoBehaviour
{
    private GameObject[] targets;
    private GameObject currentTarget;
    private Vector3 targetPos;
    private EnemyBehaviour enemyBehaviour;
    public GameObject collisionObject;
    public bool attackMode;
    private Animation animation;
    private float distanceFromTarget;


    public float speed = 0.1f;
    public int hp = 100;
    public float attackCooldown = 1.0f;
    public int attackPoints = 1;
    public float killThreshhold = 1.5f;
    public Color defaultColor;

    //Use this for initialization
    void Start()
    {
        targets = GameObject.FindGameObjectsWithTag("Enemy");
        currentTarget = targets[0];
        distanceFromTarget = Vector3.Distance(transform.position, targets[0].transform.position);
        animation = GetComponent<Animation>();
        animation.CrossFade("Devil_Dog_Run");
        attackMode = false;
    }

    void FixedUpdate()
    {
        foreach (GameObject target in targets)
        {
            float tempDistance = Vector3.Distance(transform.position, target.transform.position);
            if (tempDistance < distanceFromTarget)
            {
                distanceFromTarget = Vector3.Distance(transform.position, target.transform.position);
                currentTarget = target;
            }
        }


        if (transform.position.y <= -1)
        {
            Destroy(gameObject);
        }
        targetPos = currentTarget.transform.position;
        transform.LookAt(currentTarget.transform);

        if (attackMode)
        {
            StartCoroutine(Attack());
        }
        else if (hp <= 0)
        {
            StartCoroutine(Die());
            targetPos = transform.position;
        }
        else
        {
            //GetComponent<Renderer>().material.color = defaultColor;
        }
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
    }

    void OnCollisionEnter(Collision collision)
    {
        collisionObject = collision.gameObject;
        if (collision.gameObject == currentTarget)
        {
            enemyBehaviour = collision.gameObject.GetComponent<EnemyBehaviour>();
            attackMode = true;
        }
    }


    public IEnumerator Attack()
    {
        animation.CrossFade("Devil_Dog_Attack01", .2f);
        attackMode = false;
        //GetComponent<Renderer>().material.color = Color.red;
        enemyBehaviour.ProcessDamage(attackPoints);
        yield return new WaitForSeconds(attackCooldown);
        attackMode = true;
    }

    public int getHitPoints()
    {
        return hp;
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
        animation.CrossFade("Devil_Dog_Death", .3f);
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}