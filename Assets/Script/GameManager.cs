using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyobjs;
    public Transform[] spawnPoints;

    public float maxSpawnDelay;
    public float curSpawnDelay;

    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if(curSpawnDelay > maxSpawnDelay)
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 3f);
            curSpawnDelay = 0;
        }
    }

    void SpawnEnemy()
    {
        int randomEnemy = Random.Range(0, 3);
        int randomPoint = Random.Range(0, 5);
        Instantiate(enemyobjs[randomEnemy], spawnPoints[randomPoint].position, spawnPoints[randomPoint].rotation);
    }
}
