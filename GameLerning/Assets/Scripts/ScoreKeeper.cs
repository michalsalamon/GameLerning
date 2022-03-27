using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static int score { get; private set; }
    private float lastEnemyKillTime;
    private int streakCount = 0;
    float strakExpairyTime = 1;

    private void Start()
    {
        Enemy.OnDeathStatic += OnEnemyKilled;
        if (FindObjectOfType<Player>() != null)
        {
            FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
        }
    }

    private void OnEnemyKilled()
    {
        if (Time.time < lastEnemyKillTime + strakExpairyTime)
        {
            streakCount++;
        }
        else
        {
            streakCount = 0;
        }
        lastEnemyKillTime = Time.time;

        score += 5 + (int)Mathf.Pow(2, streakCount);
    }

    private void OnPlayerDeath()
    {
        Enemy.OnDeathStatic -= OnEnemyKilled;
    }
}
