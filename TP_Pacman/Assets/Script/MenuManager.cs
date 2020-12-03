using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void OnClick_Play()
    {
        SceneManager.LoadScene(1);
    }
    public void OnClick_Exit()
    {
        Application.Quit();
    }
}
