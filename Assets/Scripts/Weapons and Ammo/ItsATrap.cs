using UnityEngine;
using System.Collections;

public class ItsATrap : MonoBehaviour
{
    public int hp;
    public int attackPoints;
    public float attackCooldown;
    private EnemyBehaviour enemyBehaviour;
    private bool trapping;

    // Use this for initialization
    void Start()
    {
        trapping = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (trapping)
        {
            StartCoroutine("DealDamage");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject collisionObject = collision.gameObject;
        if (collisionObject.tag == "Enemy")
        {
            enemyBehaviour = collisionObject.GetComponent<EnemyBehaviour>();
            trapping = true;
        }
    }

    public IEnumerator DealDamage()
    {
        trapping = false;
        enemyBehaviour.ProcessDamage(attackPoints);
        yield return new WaitForSeconds(trapCooldown);
        trapping = true;
    }
}
