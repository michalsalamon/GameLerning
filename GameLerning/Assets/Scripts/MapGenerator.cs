using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Transform obstaclePrefab;
    [SerializeField] private Vector2 mapSize;

    [SerializeField] [Range(0, 1)] private float outlinePercent;
    [SerializeField] [Range(0, 1)] private float obstacleProcent;

    [SerializeField] private float tileSize;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;

    [SerializeField] private int seed = 10;
    private Coord mapCenter;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));
        mapCenter = new Coord((int)mapSize.x / 2, (int)mapSize.y / 2);

                string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.transform.parent = transform;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3 f_tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, f_tilePosition, Quaternion.Euler(Vector3.right * 90));
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.transform.parent = mapHolder;
            }
        }


        bool[,] f_obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];
        int f_obstacleCount = (int)(mapSize.x * mapSize.y * obstacleProcent);
        int f_currentObstacleCount = 0;

        for (int i = 0; i < f_obstacleCount; i++)
        {
            Coord f_randomCoord = GetRandomCoord();
            f_obstacleMap[f_randomCoord.x, f_randomCoord.y] = true;
            f_currentObstacleCount ++;

            if (f_randomCoord != mapCenter && MapIsFullyAccessible(f_obstacleMap, f_currentObstacleCount))
            {
                Vector3 f_obstaclePosition = CoordToPosition(f_randomCoord.x, f_randomCoord.y);


                Transform f_newObstacle = Instantiate(obstaclePrefab, f_obstaclePosition + Vector3.up * 0.5f * tileSize, Quaternion.identity);
                f_newObstacle.localScale = Vector3.one * tileSize;
                f_newObstacle.transform.parent = mapHolder;
            }
            else
            {
                f_obstacleMap[f_randomCoord.x, f_randomCoord.y] = false;
                f_currentObstacleCount--;
            }
        }

    }

    private bool MapIsFullyAccessible(bool[,] f_obstacleMap, int f_currentObstacleCount)
    {
        bool[,] f_mapFlags = new bool[f_obstacleMap.GetLength(0), f_obstacleMap.GetLength(1)];
        Queue<Coord> f_queue = new Queue<Coord>();
        f_queue.Enqueue(mapCenter);
        f_mapFlags[mapCenter.x, mapCenter.y] = true;

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
        int f_targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - f_currentObstacleCount);
        return f_targetAccessibleTileCount == f_accsessibleTileCount;
    }

    private Vector3 CoordToPosition(int f_x, int f_y)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + f_x, 0, -mapSize.y / 2 + 0.5f + f_y) * tileSize;
    }

    private Coord GetRandomCoord()
    {
        Coord f_randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(f_randomCoord);
        return f_randomCoord;

    }

    private struct Coord
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
}
