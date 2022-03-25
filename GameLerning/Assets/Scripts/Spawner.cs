using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private bool developerMode;

    [SerializeField] private Wave[] waves;
    [SerializeField] private Enemy enemy;

    private Wave currentWave;
    public Wave CurrentWave
    { get { return currentWave; } }
    int currentWaveNumber = 0;

    private int enemiesRemainigToSpawn;
    private int enemiesRemainigAlive;
    float nextSpawnTime;

    private MapGenerator map;
    private LivingEntity playerEntity;
    private Transform playerTransform;
    private Coroutine spawnEnemy;

    private float timeBeatweenCampChecks = 2;
    private float campThresholdDistance = 1.5f;
    private float nextCampCheckTime;
    private Vector3 campPositionOld;
    private bool isCamping = false;

    private Color initialTileColour;

    private bool isDisabled = false;

    [System.Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemtyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;
    }

    public event System.Action<int> OnNewWave;

    private void Start()
    {

        playerEntity = FindObjectOfType<Player>();
        playerTransform = playerEntity.transform;

        nextCampCheckTime = timeBeatweenCampChecks + Time.time;
        campPositionOld = playerTransform.position;
        playerEntity.OnDeath += OnPLayerDeath;

        map = FindObjectOfType<MapGenerator>();

        initialTileColour = Color.white;
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

            if ((enemiesRemainigToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
            {
                enemiesRemainigToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                spawnEnemy = StartCoroutine("SpawnEnemy");
            }
        }

        if (developerMode)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("SpawnEnemy");
                //StopAllCoroutines();
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    Destroy(enemy.gameObject);
                }
                NextWave();
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
        Color f_flashColour = Color.red;
        float f_spawnTimer = 0;

        while (f_spawnTimer < f_spawnDelay)
        {
            f_tileMaterial.color = Color.Lerp(initialTileColour, f_flashColour, Mathf.PingPong(f_spawnTimer * f_tileFlashSpeed, 1));

            f_spawnTimer += Time.deltaTime;
            yield return null;
        }
        f_tileMaterial.color = initialTileColour;

        Enemy spawnedEnemy = Instantiate(enemy, f_spawnTile.position + Vector3.up, Quaternion.identity);
        spawnedEnemy.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
        yield break;
    }

    void OnPLayerDeath()
    {
        isDisabled = true;
        Cursor.visible = true;
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
        playerTransform.position = map.CoordToPosition(map.CurrentMap.mapCenter.x, map.CurrentMap.mapCenter.y) + Vector3.up;
    }

    void NextWave()
    {
        if (currentWaveNumber > 0)
        {
            AudioManager.instance.PlaySound2D("Level Completed");
        }
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
