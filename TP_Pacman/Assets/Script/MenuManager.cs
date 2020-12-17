using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance = null;

    public Canvas canvas;
    public GameObject panelExplanations, panelEndGame, textsVictory, textsDefeat;
    public Text ScoreText, HighscoreText, BallsText, TimeText;
    public Button startButton;
    public Toggle englishToggle, frenchToggle;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        SelectButton(startButton);
        if (SceneManager.GetActiveScene().buildIndex == 0) {
            ActivateLanguageToggle();
            ChooseLanguage();
        } else {
            UpdateMenuTexts();
        }
    }

    public void ChooseLanguage() {
        Language chosenLang = LanguageManager.Instance.defaultLanguage;
        if (englishToggle.isOn) {
            chosenLang = Language.English;
        } else if (frenchToggle.isOn) {
            chosenLang = Language.French;
        }
        LanguageManager.LoadLanguage(chosenLang);
        UpdateMenuTexts();
    }

    /// <summary>
    /// Update all the text elements with component "TextLang" with their associated text in the selected language
    /// </summary>
    public void UpdateMenuTexts() {
        LanguageManager.UpdateTexts(canvas);
    }

    public void UpdateOverlay(float time, int score, int highscore, int remainingBalls) {
        TimeText.text = TimeSpan.FromSeconds(time).ToString("hh':'mm':'ss");
        ScoreText.text = score.ToString();
        HighscoreText.text = highscore.ToString();
        BallsText.text = remainingBalls.ToString();
        //Debug.Log("Time: " + TimeSpan.FromSeconds(time).ToString("hh':'mm':'ss"));
    }

    public void ShowVictory() {
        panelEndGame.SetActive(true);
        textsVictory.SetActive(true);
        textsDefeat.SetActive(false);
    }
    public void ShowGameover() {
        panelEndGame.SetActive(true);
        textsVictory.SetActive(false);
        textsDefeat.SetActive(true);
    }

    public void ResetHighscore() {
        PlayerPrefs.SetInt(GameManager.keyHighscore, 0);
        PlayerPrefs.Save();
    }

    public void ActivateLanguageToggle() {
        Language currentLang = LanguageManager.GetLanguage();
        if (currentLang == Language.French) {
            frenchToggle.isOn = true;
        } else if(currentLang == Language.English) {
            englishToggle.isOn = true;
        }
    }

    public void SelectButton(Button btn) {
        if (btn == null) { return; }
        EventSystem.current.SetSelectedGameObject(btn.gameObject);
    }

    public void LoadGameScene() {
        SceneManager.LoadScene(1);
    }
    public void StartGame() {
        panelExplanations.SetActive(false);
        GameManager.Instance.StartGame();
    }
    public void RestartGame()
    {
        panelEndGame.SetActive(false);
        GameManager.Instance.RestartGame();
    }
    public void LoadMenuScene()
    {
        SceneManager.LoadScene(0);
    }
    public void ExitApplication()
    {
        Application.Quit();
    }
}
