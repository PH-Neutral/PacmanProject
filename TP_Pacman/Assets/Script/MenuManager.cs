using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance = null;

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

    public void OnClick_Menu()
    {
        SceneManager.LoadScene(0);
    }
    public void OnClick_Exit()
    {
        Application.Quit();
    }
}
