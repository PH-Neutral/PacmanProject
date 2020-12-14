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
        // +++ recover the settings from PlayerPref +++ //
        // recover language file
        //LanguageManager.LoadLanguage(Language.English);
        //UpdateMenuTexts();
        // recover the highscores from local storage (System.IO here we come !)
    }

    void UpdateMenuTexts() {
        // get all references to the mainMenu UI.Text elements and then write them using the LanguageManager class
    }

    public void Update_Overlay(float time, int score, int remainingBalls) {
        ScoreText.text = LanguageManager.GetText(LangKeyWord.gameOverlay_score) + ": " + score;
        TimeText.text = LanguageManager.GetText(LangKeyWord.gameOverlay_elapsedTime) + ": " + TimeSpan.FromSeconds(time).ToString("hh':'mm':'ss");
        BallsText.text = LanguageManager.GetText(LangKeyWord.gameOverlay_remainingBalls) + ": " + remainingBalls;
        //Debug.Log("Time: " + TimeSpan.FromSeconds(time).ToString("hh':'mm':'ss"));
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

    public void OnClick_Play() {
        SceneManager.LoadScene(1);
    }
    public void OnClick_LaunchGame() {
        GameManager.Instance.StartGame();
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
}
