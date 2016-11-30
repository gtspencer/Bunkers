using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using MovementEffects;
using Valve.VR;

public class FriendlyBehaviour : MonoBehaviour
{
    private GameObject[] targets;
    private GameObject currentTarget;
    private Vector3 targetPos;
    private EnemyBehaviour enemyBehaviour;
    private GameObject collisionObject;
    private float distanceFromTarget;

    public bool attackMode;
    public bool smallMode;
    public bool scaleMode;

    private Animation animation;
    public float speed = 0.1f;
    public int hp = 100;
    public float attackCooldown = 1.0f;
    public int attackPoints = 1;
    public float killThreshhold = 1.5f;
    public Color defaultColor;
    public float scaleUp = .1f;
    public float maxScale = 1.3f;
    public float minScale = .1f;


    //Use this for initialization
    void Start()
    {
        targets = GameObject.FindGameObjectsWithTag("Enemy");
        currentTarget = targets[0];
        distanceFromTarget = Vector3.Distance(transform.position, targets[0].transform.position);
        animation = GetComponent<Animation>();
        animation.CrossFade("Devil_Dog_Idle");
        attackMode = false;
        smallMode = true;
        scaleMode = false;
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

        if (attackMode) //collided with valid enemy
        {
            Timing.RunCoroutine(Attack());
        }
        else if (smallMode) //in play pen
        {
            animation.CrossFade("Devil_Dog_Walk01");
        }
        else if (scaleMode) //grow up here
        {
            scaleMode = false;
            if (gameObject.transform.localScale.x < maxScale)
            {
                Vector3 scale = gameObject.transform.localScale;
                scale.x += scaleUp;
                scale.y += scaleUp;
                scale.z += scaleUp;
                gameObject.transform.localScale = scale;
                scaleMode = true;
            }
        }
        else if (hp <= 0) //dead mode
        {
            Timing.RunCoroutine(Die());
            targetPos = transform.position;
        }
        else //normal mode
        {
            //GetComponent<Renderer>().material.color = defaultColor;
            animation.CrossFade("Devil_Dog_Run");
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        collisionObject = collision.gameObject;
        if (collision.gameObject == currentTarget)
        {
            enemyBehaviour = collision.gameObject.GetComponent<EnemyBehaviour>();
            attackMode = true;
        } else if (collision.gameObject.tag == "Ground" && smallMode == true)
        {
            scaleMode = true;
        }
    }


    public IEnumerator<float> Attack()
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

    public IEnumerator<float> Die()
    {
        attackMode = false;
        animation.CrossFade("Devil_Dog_Death", .3f);
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}