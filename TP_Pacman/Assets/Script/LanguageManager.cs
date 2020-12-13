using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager {
    const string folderPath = "/Lang", fileExtension = ".txt";
    static string[] langFiles = new string[] { "FR-fr", "EN-us" };

    static LanguageManager Lang = null;

    public static bool LoadLanguage(Language language) {
        if ((int)language < 0 || (int)language >= langFiles.Length) { return false; }
        Lang = new LanguageManager(language);
        return true;
    }

    public static string GetText(LangKeyWord keyWord) {
        string key = keyWord.ToString();
        if(!Lang._texts.ContainsKey(key)) { return ""; }
        return Lang._texts[key];
    }

    // - - - - - Instance - - - - - //

    Dictionary<string, string> _texts;

    public LanguageManager(Language language) {
        _texts = new Dictionary<string, string>();
        if (!LoadLanguageFile(language)) {
            WriteLanguageFile(language);
            LoadLanguageFile(language);
        }
    }

    bool LoadLanguageFile(Language language) {
        //if((int)language < 0 || (int)language >= langFiles.Length) { return; }
        string fileName = langFiles[(int)language] + fileExtension;
        string[] lines = IOTools.LoadFile(folderPath, fileName);
        if (lines == null) { return false; }
        for(int i=0; i<lines.Length; i++) {
            //Debug.Log(line);
            string[] decodedLine = DecodeLine(lines[i]);
            _texts[decodedLine[0]] = decodedLine[1];
            //Debug.Log("line: " + lines[i] + "; key: " + decodedLine[0] + "; value: " + decodedLine[1]);
        }
        return true;
    }

    void WriteLanguageFile(Language language) {
        string fileName = langFiles[(int)language] + fileExtension;
        string[] lines = new string[keyWords.Length];
        string[] values;
        if ((values = languageValues[(int)language]) == null) {
            values = languageValues[(int)defaultLanguage];
        }
        for (int i=0; i<keyWords.Length; i++) {
            string value = i < values.Length ? values[i] : defaultValue;
            if (value == "") {
                value = defaultValue;
            }
            lines[i] = EncodeLine(keyWords[i].ToString(), value);
        }
        IOTools.WriteFile(folderPath, fileName, lines);
    }

    string[] DecodeLine(string line) {
        string key = "";
        string workingString = "";
        for (int i=0; i<line.Length; i++) {
            if (line[i] == '=') {
                workingString = line.Substring(i + 1, line.Length - (i + 1));
                break;
            }
            if (line[i] != ' ') {
                key += line[i];
            }
        }
        bool quoteOpen = false;
        int startIndex = 0, endIndex = 0;
        for (int i=0; i< workingString.Length; i++) {
            if (workingString[i] == '\"') {
                if (!quoteOpen) {
                    startIndex = i + 1;
                    quoteOpen = true;
                } else {
                    endIndex = i;
                }
            }
        }
        string value = workingString.Substring(startIndex, endIndex - startIndex);
        return new string[] { key, value };
    }

    string EncodeLine(string key, string value) {
        return key + '=' + '\"' + value + '\"';
    }

    // - - - - - DEFAULT VALUES - - - - - //

    static Language defaultLanguage = Language.English;
    static LangKeyWord[] keyWords = new LangKeyWord[] {
        LangKeyWord.mainMenu_title, LangKeyWord.mainMenu_subtitle, LangKeyWord.mainMenu_button_start, LangKeyWord.mainMenu_button_settings, 
        LangKeyWord.mainMenu_button_credits, LangKeyWord.mainMenu_button_exitApp, LangKeyWord.gameOverlay_score, LangKeyWord.gameOverlay_elapsedTime, 
        LangKeyWord.gameOverlay_remainingBalls
    };
    static string defaultValue = "[Missing]";
    static string[][] languageValues = new string[][] { null, englishValues };
    static string[] englishValues = new string[] {
        "Pacman", "Unity practical work", "Start", "Settings", "Credits", "Exit", "Score", "Time", "Remaing Balls"
    };
}

public enum Language {
    French, English
}

public enum LangKeyWord {
    mainMenu_title, mainMenu_subtitle, mainMenu_button_start, mainMenu_button_settings, mainMenu_button_credits, mainMenu_button_exitApp, 
    gameOverlay_score, gameOverlay_elapsedTime, gameOverlay_remainingBalls
}