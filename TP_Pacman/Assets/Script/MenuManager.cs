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

    public GameObject panelExplanations, panelEndGame, textsVictory, textsDefeat;
    public Text ScoreText, HighscoreText, BallsText, TimeText;
    public Button startButton;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        // +++ recover the settings from PlayerPref +++ //
        // recover the highscores from local storage (System.IO here we come !)
        //Debug.Log("Scene loaded!");
        // select first button (only if on mainMenu)
        if (SceneManager.GetActiveScene().buildIndex == 0) {
            ChooseLanguage(Language.English);
            SelectButton(startButton);
        }
    }

    public void ChooseLanguage(Language lang) {
        LanguageManager.LoadLanguage(lang); // recover language file
        Debug.Log("The language is now" + LanguageManager.GetLanguage().ToString());
        UpdateMenuTexts(); // update all the text elements to the current language
    }

    void UpdateMenuTexts() {
        // get all references to the mainMenu UI.Text elements and then write them using the LanguageManager class
        TextLang[] texts = FindObjectsOfType<TextLang>();
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

    public void SelectButton(Button btn) {
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
