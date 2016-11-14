using UnityEngine;
using System.Collections;

public class ItsATrap : MonoBehaviour
{
    public int hp;
    public int attackPoints;
    private EnemyBehaviour enemyBehaviour;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionTrigger(Collision collision)
    {
        GameObject collisionObject = collision.gameObject;
        if (collisionObject.name == "Enemy")
        {
            enemyBehaviour = collisionObject.GetComponent<EnemyBehaviour>();
            //enemyBehaviour.processDamage(attackPoints);
        }
    }
}
