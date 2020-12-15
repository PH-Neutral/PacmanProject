using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ParametricButton : MonoBehaviour
{
    public bool selectFirst = false;
    public KeyCode keyCode;

    private void Start() {
        if (selectFirst) {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    private void Update() {
        /*if (Input.GetKeyDown(keyCode) && EventSystem.current.currentSelectedGameObject == gameObject) {
            EventSystem.current.
        }*/
    }
}
