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
}
