using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Image fadePlane;
    [SerializeField] private GameObject gameOverUI;

    private void Start()
    {
        FindObjectOfType<PlayerInput>().OnDeath += OnGameOver;
    }

    void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, Color.black, 1));
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
}
