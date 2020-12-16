using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour {
    public static LanguageManager Instance = null;

    public static void UpdateTexts(TextLang[] textElements) {
        for(int i = 0; i < textElements.Length; i++) {
            textElements[i].Text = Instance.GetValue(textElements[i].keyWord);
        }
    }

    public static void LoadLanguage(Language language) {
        Instance.LoadLanguageHardCoded(language);
    }

    public static Language GetLanguage() {
        return Instance.currentLanguage;
    }

    static void InitializeDefaultValues() {
        languageValues = new string[][] { frenchValues, englishValues };
    }

    // - - - - - Instance - - - - - //
    public Language defaultLanguage = Language.English;
    public Language currentLanguage;

    Dictionary<string, string> _textAssociations;

    private void Start() {
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);

        InitializeDefaultValues();
        // get saved language from PLayerPrefs
        string savedLanguage = PlayerPrefs.HasKey(keyLanguage) ? PlayerPrefs.GetString(keyLanguage) : defaultLanguage.ToString();
        for (int i=0; i<languages.Length; i++) {
            if (savedLanguage == languages[i].ToString()) {
                currentLanguage = languages[i];
            }
        }
        _textAssociations = new Dictionary<string, string>();
        LoadLanguageHardCoded(currentLanguage);
        /*WriteLanguageFileDefault();
        if(!LoadLanguageFile(language)) {
            LoadLanguageFileDefault();
            currentLanguage = defaultLanguage;
        }*/
    }

    void LoadLanguageHardCoded(Language language) {
        for(int i = 0; i < keyWords.Length; i++) {
            string txtValue = missingValue;
            //Debug.Log("DefLang: " + (int)defaultLanguage + "; langValue: " + languageValues[(int)defaultLanguage]);
            if(i < languageValues[(int)language].Length) {
                txtValue = languageValues[(int)language][i];
            }
            _textAssociations[keyWords[i].ToString()] = txtValue;
            //Debug.Log("line: " + lines[i] + "; key: " + decodedLine[0] + "; value: " + decodedLine[1]);
        }
        PlayerPrefs.SetString(keyLanguage, language.ToString());
    }

    public string GetLanguageName(Language lang) {
        return languageNames[(int)lang];
    }

    public string GetValue(LangKeyWord keyWord) => GetValue(keyWord.ToString());
    public string GetValue(string keyWord) {
        if(!_textAssociations.ContainsKey(keyWord)) {
            return missingValue;
        }
        return _textAssociations[keyWord];
    }

    // +++ Unused and incomplete code meant for managing langauges with txt files +++ //

    /*const string folderPath = "/Lang", fileExtension = ".txt";
    static string[] langFiles = new string[] { "FR-fr", "EN-us" };

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
    }*/

    // - - - - - DEFAULT VALUES - - - - - //

    static string keyLanguage = "language";
    static Language[] languages = new Language[] { Language.French, Language.English };
    static string[] languageNames = new string[] { "Français", "English" };
    static LangKeyWord[] keyWords = new LangKeyWord[] {
        LangKeyWord.language,
        LangKeyWord.mainMenu_title, LangKeyWord.mainMenu_subtitle, LangKeyWord.button_start, LangKeyWord.button_settings, LangKeyWord.button_credits,
        LangKeyWord.button_exitApp, LangKeyWord.button_retry, LangKeyWord.button_menu,
        LangKeyWord.gameOverlay_score, LangKeyWord.gameOverlay_highscore, LangKeyWord.gameOverlay_elapsedTime, LangKeyWord.gameOverlay_remainingBalls, LangKeyWord.gameOverlay_victoryTitle,
        LangKeyWord.gameOverlay_victoryDescription, LangKeyWord.gameOverlay_defeatTitle, LangKeyWord.gameOverlay_defeatDescription,
        LangKeyWord.inProgress,
        LangKeyWord.creditMenu_title, LangKeyWord.creditMenu_developpedBy,
        LangKeyWord.settingsMenu_title, LangKeyWord.settingsMenu_language, LangKeyWord.button_resetHighscore
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
        "Settings", "Language:", "Reset Highscore"
    };
    static string[] frenchValues = new string[] {
        "Français",
        "Pacman", "TP Unity", "Jouer", "Options", "Crédits", "Quitter", "Rejouer", "Menu", "SCORE:", "HIGHSCORE:", "TEMPS:", "BALLES RESTANTES:",
        "VICTOIRE!", "Vous avez réussi à manger tous les Pac-Gums en évitant les fantômes. Bien joué.",
        "DEFAITE...", "Vous êtes mort, attrapé par les fantômes. Vous gagnerez peut-être la prochaine fois.",
        "En cours...",
        "Crédits", "Développé par:",
        "Options", "Langue:", "Réinitialiser Highscore"
    };
}

public enum Language {
    French, English
}

public enum LangKeyWord {
    mainMenu_title, mainMenu_subtitle, button_start, button_settings, button_credits, button_exitApp, button_retry, button_menu,
    gameOverlay_score, gameOverlay_highscore, gameOverlay_elapsedTime, gameOverlay_remainingBalls, gameOverlay_victoryTitle, gameOverlay_defeatTitle, gameOverlay_victoryDescription,
    gameOverlay_defeatDescription, language, inProgress, creditMenu_developpedBy, creditMenu_title, settingsMenu_title, settingsMenu_language, button_resetHighscore
}