using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using Valve.VR;

public class EnemyHUDBehaviour : MonoBehaviour
{
    private TextMesh textMesh;
    public GameObject enemy;
    private EnemyBehaviour enemyBehaviour;

    //Use this for initialization
    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
    }

    void FixedUpdate()
    {
        textMesh.transform.LookAt(Camera.main.transform.position);
        int hp = enemyBehaviour.getHitPoints();
        if (hp <= 0)
        {
            textMesh.text = "HP: 0";
        }
        else
        {
            textMesh.text = "HP: " + hp;
        }
    }
}