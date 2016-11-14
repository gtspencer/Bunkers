using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour {

    public GameObject enemyToSpawn;
    public string tag;
    public int numberOfEnemies = 5;
    public float spawnTimeLowRange = 3;
    public float spawnTimeHighRange = 5;

    public int spawnMinRange = -10;
    public int spawnMaxRange = 10;

    public GameObject[] enemies;

    private int amount;

    private Vector3 spawnPoint;
	
	// Update is called once per frame
	void FixedUpdate () {
        enemies = GameObject.FindGameObjectsWithTag(tag);
        amount = enemies.Length;

        if (amount != numberOfEnemies)
        {
            InvokeRepeating("spawnEnemy", spawnTimeLowRange, spawnTimeHighRange);
        }
	}

    void spawnEnemy()
    {
        spawnPoint.x = Random.Range(spawnMinRange, spawnMaxRange);
        spawnPoint.y = 0.7f;
        spawnPoint.z = Random.Range(spawnMinRange, spawnMaxRange);

        Instantiate(enemyToSpawn, spawnPoint, Quaternion.identity);
        CancelInvoke();
    }
}
