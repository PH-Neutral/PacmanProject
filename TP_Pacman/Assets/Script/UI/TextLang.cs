using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLang : MonoBehaviour {
    public LangKeyWord keyWord;
    public string Text {
        set {
            GetComponent<Text>().text = value;
        }
    }

    Text textElement = null;
    private void Awake() {
        textElement = GetComponent<Text>();
        if (textElement == null) {
            Debug.LogError(name + " doesn't have a UI.Text component!");
        }
    }
}
