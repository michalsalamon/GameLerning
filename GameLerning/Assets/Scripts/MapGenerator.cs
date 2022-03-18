using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Transform obstaclePrefab;
    [SerializeField] private Vector2 mapSize;

    [SerializeField] [Range(0, 1)] private float outlinePercent;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;

    [SerializeField] private int seed = 10;

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
                newTile.localScale *= (1 - outlinePercent);
                newTile.transform.parent = mapHolder;
            }
        }

        int f_obstacleCount = 10;
        for (int i = 0; i < f_obstacleCount; i++)
        {
            Coord f_randomCoord = GetRandomCoord();
            Vector3 f_obstaclePosition = CoordToPosition(f_randomCoord.x, f_randomCoord.y);
            Transform f_newObstacle = Instantiate(obstaclePrefab, f_obstaclePosition + Vector3.up * 0.5f, Quaternion.identity);
            f_newObstacle.transform.parent = mapHolder;
        }

    }

    private Vector3 CoordToPosition(int f_x, int f_y)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + f_x, 0, -mapSize.y / 2 + 0.5f + f_y);
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
    }
}
