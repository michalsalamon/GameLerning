using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Map[] maps;
    [SerializeField] int mapIndex;

    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Transform obstaclePrefab;
    [SerializeField] private Transform navmeshMaskPrefab;
    [SerializeField] private Transform mapFloor;
    [SerializeField] private Transform navmeshFloor;
    [SerializeField] private Vector2 maxMapSize;

    [SerializeField] [Range(0, 1)] private float outlinePercent;

    [SerializeField] private float tileSize;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;
    Queue<Coord> shuffledOpenTileCoords;
    private Transform[,] tileMap;

    private Map currentMap;
    public Map CurrentMap
    { get { return currentMap; } }

    private void Start()
    {
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        mapIndex = waveNumber - 1;
        if (mapIndex >= maps.Length)
        {
            mapIndex = maps.Length - 1;
        }
        GenerateMap();
    }

    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        System.Random f_prng = new System.Random(currentMap.seed);

        //Generaiting coords
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));

        //create map holder
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.transform.parent = transform;

        //Spawning tiles
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Vector3 f_tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, f_tilePosition, Quaternion.Euler(Vector3.right * 90));
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.transform.parent = mapHolder;
                tileMap[x, y] = newTile;
            }
        }


        //spawning obstacles
        bool[,] f_obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];
        int f_obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int f_currentObstacleCount = 0;
        List<Coord> allOpenCoords = new List<Coord>(allTileCoords);

        for (int i = 0; i < f_obstacleCount; i++)
        {
            Coord f_randomCoord = GetRandomCoord();
            f_obstacleMap[f_randomCoord.x, f_randomCoord.y] = true;
            f_currentObstacleCount ++;

            if (f_randomCoord != currentMap.mapCenter && MapIsFullyAccessible(f_obstacleMap, f_currentObstacleCount))
            {
                float f_obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)f_prng.NextDouble());
                Vector3 f_obstaclePosition = CoordToPosition(f_randomCoord.x, f_randomCoord.y);

                Transform f_newObstacle = Instantiate(obstaclePrefab, f_obstaclePosition + Vector3.up * f_obstacleHeight * 0.5f * tileSize, Quaternion.identity);
                f_newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, f_obstacleHeight, (1 - outlinePercent) * tileSize);
                f_newObstacle.transform.parent = mapHolder;

                Renderer f_obstacleRenderer = f_newObstacle.GetComponent<Renderer>();
                Material f_obstacleMaterial = new Material(f_obstacleRenderer.sharedMaterial);
                float f_colourPercent = f_randomCoord.y / (float)currentMap.mapSize.y;
                f_obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, f_colourPercent);
                f_obstacleRenderer.sharedMaterial = f_obstacleMaterial;

                allOpenCoords.Remove(f_randomCoord);
            }
            else
            {
                f_obstacleMap[f_randomCoord.x, f_randomCoord.y] = false;
                f_currentObstacleCount--;
            }
        }

        shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));

        //creating navmesh mask
        Transform f_maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity);
        f_maskLeft.parent = mapHolder;
        f_maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform f_maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity);
        f_maskRight.parent = mapHolder;
        f_maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform f_maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity);
        f_maskTop.parent = mapHolder;
        f_maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        Transform f_maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity);
        f_maskBottom.parent = mapHolder;
        f_maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;


        navmeshFloor.localScale = new Vector3( maxMapSize.x, maxMapSize.y, 1) * tileSize;
        mapFloor.localScale = new Vector3(currentMap.mapSize.x * tileSize, currentMap.mapSize.y * tileSize, 1);

    }

    private bool MapIsFullyAccessible(bool[,] f_obstacleMap, int f_currentObstacleCount)
    {
        bool[,] f_mapFlags = new bool[f_obstacleMap.GetLength(0), f_obstacleMap.GetLength(1)];
        Queue<Coord> f_queue = new Queue<Coord>();
        f_queue.Enqueue(currentMap.mapCenter);
        f_mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

        int f_accsessibleTileCount = 1;

        while (f_queue.Count > 0)
        {
            Coord f_tile = f_queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int f_neighbourX = f_tile.x + x;
                    int f_neighbourY = f_tile.y + y;
                    if (x == 0 || y == 0)
                    {
                        if (f_neighbourX >= 0 && f_neighbourX < f_obstacleMap.GetLength(0) && f_neighbourY >= 0 && f_neighbourY < f_obstacleMap.GetLength(1))
                        {
                            if (!f_mapFlags[f_neighbourX, f_neighbourY] && !f_obstacleMap[f_neighbourX, f_neighbourY])
                            {
                                f_mapFlags[f_neighbourX, f_neighbourY] = true;
                                f_queue.Enqueue(new Coord(f_neighbourX, f_neighbourY));
                                f_accsessibleTileCount ++;
                            }
                        }
                    }
                }
            }
        }
        int f_targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - f_currentObstacleCount);
        return f_targetAccessibleTileCount == f_accsessibleTileCount;
    }

    public Vector3 CoordToPosition(int f_x, int f_y)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + f_x, 0, -currentMap.mapSize.y / 2f + 0.5f + f_y) * tileSize;
    }

    public Transform GetTileFromPosition(Vector3 f_position)
    {
        int f_x = Mathf.RoundToInt(f_position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
        int f_y = Mathf.RoundToInt(f_position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);
        f_x = Mathf.Clamp(f_x, 0 ,tileMap.GetLength(0) - 1);
        f_y = Mathf.Clamp(f_y, 0, tileMap.GetLength(1) - 1);
        return tileMap[f_x, f_y];
    }

    private Coord GetRandomCoord()
    {
        Coord f_randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(f_randomCoord);
        return f_randomCoord;
    }

    public Transform GetRandomOpenTile()
    {
        Coord f_randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(f_randomCoord);
        return tileMap[f_randomCoord.x, f_randomCoord.y];
    }

    [System.Serializable]
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int f_x, int f_y)
        {
            x = f_x;
            y = f_y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }

    [System.Serializable]
    public class Map
    {
        public Coord mapSize;
        [Range (0,1)][SerializeField] public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;

        public Coord mapCenter
        {
            get
            {
                return new Coord(Mathf.RoundToInt(mapSize.x / 2), Mathf.RoundToInt(mapSize.y / 2));
            }
        }
    }
}
