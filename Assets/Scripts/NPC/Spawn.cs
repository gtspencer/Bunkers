using UnityEngine;
using System.Collections.Generic;
using MovementEffects;

public class Spawn : MonoBehaviour {

    public GameObject enemyToSpawn;
    public string tag;
    private int enemiesInScene;
    public int enemiesAllowedInScene = 5;
    public int totalEnemies = 10;
    public int spawnCoolDown = 2;
    public int spawnMinRange = -10;
    public int spawnMaxRange = 10;

    private bool spawning;
    private GameObject[] enemies;
    private int enemiesSoFar;
    private Vector3 spawnPoint;
	
	// Update is called once per frame
	void FixedUpdate () {
        enemies = GameObject.FindGameObjectsWithTag(tag);
        enemiesInScene = enemies.Length;

        if (enemiesInScene < enemiesAllowedInScene && !spawning && enemiesSoFar < totalEnemies)
        {
            Timing.RunCoroutine(spawnEnemy());
        }

        if (enemiesSoFar >= totalEnemies)
        {
            Timing.RunCoroutine(Win());
        }
    }

    IEnumerator<float> spawnEnemy()
    {
        spawning = true;
        while (enemiesInScene < enemiesAllowedInScene)
        {
            enemies = GameObject.FindGameObjectsWithTag(tag);
            enemiesInScene = enemies.Length;

            spawnPoint.x = Random.Range(spawnMinRange, spawnMaxRange);
            spawnPoint.y = 0.7f;
            spawnPoint.z = Random.Range(spawnMinRange, spawnMaxRange);

            Instantiate(enemyToSpawn, spawnPoint, Quaternion.identity);
            enemiesSoFar++;
            yield return new WaitForSeconds(spawnCoolDown);
        }
        spawning = false;
    }

    IEnumerator<float> Win()
    {
        yield return new WaitForSeconds(1);
    }
}
