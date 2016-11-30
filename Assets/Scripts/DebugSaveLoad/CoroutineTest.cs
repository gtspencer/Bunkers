using UnityEngine;
using System.Collections;

public class CoroutineTest : MonoBehaviour
{
    private bool test;
    private Vector3 spawnPoint;
    public GameObject makeThis;
    public int firstWait;
    public int lastWait;
    public int spawnMinRange = -5;
    public int spawnMaxRange = 5;

    // Use this for initialization
    void Start()
    {
        test = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (test)
        {
            StartCoroutine(MakeSomething());
        }
    }

    IEnumerator MakeSomething()
    {
        test = false;
        spawnPoint.x = Random.Range(spawnMinRange, spawnMaxRange);
        spawnPoint.y = 0.7f;
        spawnPoint.z = Random.Range(spawnMinRange, spawnMaxRange);

        Instantiate(makeThis, spawnPoint, Quaternion.identity);
        yield return new WaitForSeconds(firstWait);
        test = true;
        yield return new WaitForSeconds(lastWait);
    }
}
