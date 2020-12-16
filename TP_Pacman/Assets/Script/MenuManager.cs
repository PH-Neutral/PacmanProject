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
        //Debug.Log("The language is now " + LanguageManager.GetLanguage().ToString());
        UpdateMenuTexts(); // update all the text elements to the current language
    }

    public void UpdateMenuTexts() {
        // get all references to the mainMenu UI.Text elements and then write them using the LanguageManager class
        //TextLang[] texts = FindObjectsOfType<TextLang>();
        TextLang[] texts = canvas.GetComponentsInChildren<TextLang>(true);
        LanguageManager.UpdateTexts(texts);
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
