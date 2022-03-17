using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Wave[] waves;
    [SerializeField] private Enemy enemy;

    private Wave currentWave;
    int currentWaveNumber = 0;

    private int enemiesRemainigToSpawn;
    private int enemiesRemainigAlive;
    float nextSpawnTime;

    [System.Serializable]
    public class Wave
    {
        public int enemtyCount;
        public float timeBetweenSpawns;
    }

    private void Start()
    {
        NextWave();
    }

    private void Update()
    {
        if (enemiesRemainigToSpawn > 0 && Time.time > nextSpawnTime)
        {
            enemiesRemainigToSpawn --;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity);
            spawnedEnemy.OnDeath += OnEnemyDeath;
        }
    }

    void OnEnemyDeath()
    {
        enemiesRemainigAlive--;

        if (enemiesRemainigAlive == 0)
        {
            NextWave();
        }
    }

    void NextWave()
    {
        currentWaveNumber++;
        Debug.Log("Wave " + currentWaveNumber);
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainigToSpawn = currentWave.enemtyCount;
            enemiesRemainigAlive = enemiesRemainigToSpawn;
        }
    }

}
