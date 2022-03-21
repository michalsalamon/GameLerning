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

    private MapGenerator map;
    private LivingEntity playerEntity;
    private Transform playerTransform;

    private float timeBeatweenCampChecks = 2;
    private float campThresholdDistance = 1.5f;
    private float nextCampCheckTime;
    private Vector3 campPositionOld;
    private bool isCamping = false;

    private bool isDisabled = false;

    [System.Serializable]
    public class Wave
    {
        public int enemtyCount;
        public float timeBetweenSpawns;
    }

    public event System.Action<int> OnNewWave;

    private void Start()
    {
        playerEntity = FindObjectOfType<PlayerInput>();
        playerTransform = playerEntity.transform;

        nextCampCheckTime = timeBeatweenCampChecks + Time.time;
        campPositionOld = playerTransform.position;
        playerEntity.OnDeath += OnPLayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    private void Update()
    {
        if (!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBeatweenCampChecks;
                isCamping = (Vector3.Distance(playerTransform.position, campPositionOld)) < campThresholdDistance;
                campPositionOld = playerTransform.position;
            }

            if (enemiesRemainigToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemiesRemainigToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine(SpawnEnemy());
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float f_spawnDelay = 1;
        float f_tileFlashSpeed = 4;

        Transform f_spawnTile = map.GetRandomOpenTile();
        if (isCamping)
        {
            f_spawnTile = map.GetTileFromPosition(playerTransform.position);
        }

        Material f_tileMaterial = f_spawnTile.GetComponent<Renderer>().material;
        Color f_initialColour = f_tileMaterial.color;
        Color f_flashColour = Color.red;
        float f_spawnTimer = 0;

        while (f_spawnTimer < f_spawnDelay)
        {
            f_tileMaterial.color = Color.Lerp(f_initialColour, f_flashColour, Mathf.PingPong(f_spawnTimer * f_tileFlashSpeed, 1));

            f_spawnTimer += Time.deltaTime;
            yield return null;
        }
        f_tileMaterial.color = f_initialColour;

        Enemy spawnedEnemy = Instantiate(enemy, f_spawnTile.position + Vector3.up, Quaternion.identity);
        spawnedEnemy.OnDeath += OnEnemyDeath;
    }

    void OnPLayerDeath()
    {
        isDisabled = true;
    }

    void OnEnemyDeath()
    {
        enemiesRemainigAlive--;

        if (enemiesRemainigAlive == 0)
        {
            NextWave();
        }
    }

    private void ResetPlayerPosition()
    {
        playerTransform.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }


    void NextWave()
    {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainigToSpawn = currentWave.enemtyCount;
            enemiesRemainigAlive = enemiesRemainigToSpawn;

            if (OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
        }
        ResetPlayerPosition();
    }

}
