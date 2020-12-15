using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager {
    const string folderPath = "/Lang", fileExtension = ".txt";
    static string[] langFiles = new string[] { "FR-fr", "EN-us" };

    static LanguageManager Lang = null;

    public static void UpdateTexts(TextLang[] textElements) {
        for(int i = 0; i < textElements.Length; i++) {
            textElements[i].Text = Lang.GetValue(textElements[i].keyWord);
        }
    }

    public static bool LoadLanguage(Language language) {
        if ((int)language < 0 || (int)language >= langFiles.Length) { return false; }
        Lang = new LanguageManager(language);
        return true;
    }

    public static Language GetLanguage() {
        return Lang.currentLanguage;
    }

    public static string GetText(LangKeyWord keyWord) {
        return Lang.GetValue(keyWord);
    }

    static void InitializeDefaultValues() {
        languageValues = new string[][] { null, englishValues };
    }

    // - - - - - Instance - - - - - //

    public Language currentLanguage;
    public Dictionary<string, string> textAssociations;

    public LanguageManager(Language language) {
        InitializeDefaultValues();
        currentLanguage = language;
        textAssociations = new Dictionary<string, string>();
        WriteLanguageFileDefault();
        if (!LoadLanguageFile(language)) {
            LoadLanguageFileDefault();
            currentLanguage = defaultLanguage;
        }
    }

    bool LoadLanguageFileDefault() {
        for(int i=0; i< keyWords.Length; i++) {
            string txtValue = missingValue;
            //Debug.Log("DefLang: " + (int)defaultLanguage + "; langValue: " + languageValues[(int)defaultLanguage]);
            if (i < languageValues[(int)defaultLanguage].Length) {
                txtValue = languageValues[(int)defaultLanguage][i];
            }
            textAssociations[keyWords[i].ToString()] = txtValue;
            //Debug.Log("line: " + lines[i] + "; key: " + decodedLine[0] + "; value: " + decodedLine[1]);
        }
        return true;
    }

    bool LoadLanguageFile(Language language) {
        //if((int)language < 0 || (int)language >= langFiles.Length) { return; }
        string fileName = langFiles[(int)language] + fileExtension;
        string[] lines = IOTools.LoadFile(folderPath, fileName);
        if(lines == null) { return false; }
        if (lines.Length < keyWords.Length) { return false; }
        for(int i = 0; i < lines.Length; i++) {
            //Debug.Log(line);
            string[] decodedLine = DecodeLine(lines[i]);
            if (decodedLine != null) {
                textAssociations[decodedLine[0]] = decodedLine[1];
            }
            //Debug.Log("line: " + lines[i] + "; key: " + decodedLine[0] + "; value: " + decodedLine[1]);
        }
        return true;
    }

    void WriteLanguageFileDefault() {
        string fileName = "defaultLanguageFile" + fileExtension;
        string[] lines = new string[keyWords.Length];
        string[] values = languageValues[(int)defaultLanguage];
        for(int i = 0; i < lines.Length; i++) {
            string value = i < values.Length ? values[i] : missingValue;
            if(value == "") {
                value = missingValue;
            }
            lines[i] = EncodeLine(keyWords[i].ToString(), value);
        }
        IOTools.WriteFile(folderPath, fileName, lines);
    }

    void WriteLanguageFile(Language language) {
        string fileName = langFiles[(int)language] + fileExtension;
        string[] lines = new string[keyWords.Length];
        string[] values;
        if ((values = languageValues[(int)language]) == null) {
            values = languageValues[(int)defaultLanguage];
        }
        for (int i=0; i<keyWords.Length; i++) {
            string value = i < values.Length ? values[i] : missingValue;
            if (value == "") {
                value = missingValue;
            }
            lines[i] = EncodeLine(keyWords[i].ToString(), value);
        }
        IOTools.WriteFile(folderPath, fileName, lines);
    }

    string[] DecodeLine(string line) {
        string key = "";
        string workingString = "";
        line.TrimStart();
        if (line.Length > 0 && line[0] == '#') {
            return null;
        }
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
                    endIndex = startIndex;
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

    public string GetValue(LangKeyWord keyWord) => GetValue(keyWord.ToString());
    public string GetValue(string keyWord) {
        if (!textAssociations.ContainsKey(keyWord)) {
            return missingValue;
        }
        return textAssociations[keyWord];
    }

    // - - - - - DEFAULT VALUES - - - - - //

    static Language defaultLanguage = Language.English;
    static LangKeyWord[] keyWords = new LangKeyWord[] {
        LangKeyWord.language,
        LangKeyWord.mainMenu_title, LangKeyWord.mainMenu_subtitle, LangKeyWord.button_start, LangKeyWord.button_settings, LangKeyWord.button_credits,
        LangKeyWord.button_exitApp, LangKeyWord.button_retry, LangKeyWord.button_menu,
        LangKeyWord.gameOverlay_score, LangKeyWord.gameOverlay_highscore, LangKeyWord.gameOverlay_elapsedTime, LangKeyWord.gameOverlay_remainingBalls, LangKeyWord.gameOverlay_victoryTitle,
        LangKeyWord.gameOverlay_victoryDescription, LangKeyWord.gameOverlay_defeatTitle, LangKeyWord.gameOverlay_defeatDescription,
        LangKeyWord.inProgress,
        LangKeyWord.creditMenu_title, LangKeyWord.creditMenu_developpedBy,
        LangKeyWord.settingsMenu_title
    };
    static string missingValue = "[Missing]";
    static string[][] languageValues;
    static string[] englishValues = new string[] {
        "English",
        "Pacman", "Unity practical work", "Start", "Settings", "Credits", "Exit", "Retry", "Menu", "SCORE:", "HIGHSCORE:", "TIME:", "REMAINING BALLS:", 
        "VICTORY!", "You managed to get all the pac-gums while avoiding those pesky ghosts. Congratulation.", 
        "DEFEAT...", "It seems the ghosts got to you and killed you. Better luck next time.",
        "In progress...",
        "Credits", "Developped by:",
        "Settings"
    };
}

public enum Language {
    French, English
}

public enum LangKeyWord {
    mainMenu_title, mainMenu_subtitle, button_start, button_settings, button_credits, button_exitApp, button_retry, button_menu,
    gameOverlay_score, gameOverlay_highscore, gameOverlay_elapsedTime, gameOverlay_remainingBalls, gameOverlay_victoryTitle, gameOverlay_defeatTitle, gameOverlay_victoryDescription,
    gameOverlay_defeatDescription, language, inProgress, creditMenu_developpedBy, creditMenu_title, settingsMenu_title
}