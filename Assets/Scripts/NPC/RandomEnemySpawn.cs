using UnityEngine;
using System.Collections.Generic;
using MovementEffects;

public class RandomEnemySpawn : MonoBehaviour
{
    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;
    public float spawnMinRange = -10;
    public float spawnMaxRange = 10;

    private Vector3 spawnPoint;
    private GameObject enemyToSpawn;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Activate()
    {
        spawnEnemy();
    }

    void spawnEnemy()
    {
        var chooseEnemy = Random.Range(0, 2);
        switch (chooseEnemy)
        {
            case 1:
                enemyToSpawn = enemy1;
                break;
            case 2:
                enemyToSpawn = enemy2;
                break;
            case 3:
                enemyToSpawn = enemy3;
                break;
            default:
                enemyToSpawn = enemy1;
                break;
        }
        spawnPoint.x = Random.Range(spawnMinRange, spawnMaxRange);
        spawnPoint.y = 0.7f;
        spawnPoint.z = Random.Range(spawnMinRange, spawnMaxRange);

        Instantiate(enemyToSpawn, spawnPoint, Quaternion.identity);
    }
}
