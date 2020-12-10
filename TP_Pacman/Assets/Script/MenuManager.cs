using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance = null;

    public GameObject Background, Victory, Gameover;
    public Text ScoreText, BallsText, TimeText;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        // recover the settings from PlayerPref
        // recover the highscores from local storage (System.IO here we come !)
    }

    public void OnClick_Play()
    {
        SceneManager.LoadScene(1);
    }
    public void OnClick_LaunchGame()
    {
        GameManager.Instance.StartGame();
    }
    public void ShowVictory()
    {
        Background.SetActive(true);
        Victory.SetActive(true);
    }
    public void ShowGameover()
    {
        Background.SetActive(true);
        Gameover.SetActive(true);
    }
    public void OnClick_Retry()
    {
        GameManager.Instance.RestartGame();
    }
    public void OnClick_Menu()
    {
        SceneManager.LoadScene(0);
    }
    public void OnClick_Exit()
    {
        Application.Quit();
    }


    public void Update_Overlay(float time, int score, int remainingBalls)
    {
        //ScoreText.text = "Score : " + score;
        //BallsText.text = "Balles restantes : " + remainingBalls;
        //TimeText.text = "Temps : " + time.ToString("hh:mm:ss");
        //string.Format(str, "hh:mm:ss");
        Debug.Log("Time: " + TimeSpan.FromSeconds(time).ToString("hh':'mm':'ss"));
    }
}
