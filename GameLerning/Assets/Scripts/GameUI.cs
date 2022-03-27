using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Image fadePlane;
    [SerializeField] private GameObject gameOverUI;

    [SerializeField] private RectTransform newWaveBanner;
    [SerializeField] private Text newWaveTitle;
    [SerializeField] private Text enemyInWave;
    [SerializeField] private Text scoreDisplay;
    [SerializeField] private Text gameOverScoreDisplay;
    [SerializeField] private RectTransform healthBar;

    private Spawner spawner;
    private Player player;

    private void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;

    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
    }

    private void Update()
    {
        scoreDisplay.text = ScoreKeeper.score.ToString("D6");

        float healthPercent = 0;
        if (player != null)
        {
            healthPercent = player.health / player.StartingHealth;
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
    }

    private void OnNewWave(int waveNumber)
    {
        newWaveTitle.text = "- Wave " + waveNumber + " -";
        string enemyCount;
        enemyCount = spawner.CurrentWave.infinite ? "Infinite" : spawner.CurrentWave.enemtyCount + "";
        enemyInWave.text = "Enemies: " + enemyCount;

        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float delayTime = 1f;
        float speed = 1.5f;
        float animationPercent = 0;
        int dir = 1;
        float startPos = newWaveBanner.anchoredPosition.y;
        float endPos = startPos + 400;

        float endDelayTime = Time.time + 1 / speed + delayTime;

        while (animationPercent >= 0)
        {
            animationPercent += Time.deltaTime * speed * dir;
            if (animationPercent >= 1)
            {
                animationPercent = 1;
                if (Time.time > endDelayTime)
                {
                    dir *= -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(startPos, endPos, animationPercent);

            yield return null;
        }
    }

    void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, 0.95f), 1));
        gameOverScoreDisplay.text = scoreDisplay.text;
        scoreDisplay.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color f_from, Color f_to, float f_time)
    {
        float f_speed = 1 / f_time;
        float f_percent = 0;

        while (f_percent < 1)
        {
            f_percent += Time.deltaTime * f_speed;
            fadePlane.color = Color.Lerp(f_from, f_to, f_percent);
            yield return null;
        }
    }

    //UI Input
    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
