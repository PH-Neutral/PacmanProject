using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IOTools {

    public static string GetWorkingDirectory() {
        string parentDirectoryName = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        //Debug.Log("Parent Directory: " + parentDirectoryName);
        return parentDirectoryName;
    }

    public static string[] LoadFile(string folderPath, string fileName) {
        string projectFolder = GetWorkingDirectory();
        string filePathFull = projectFolder + folderPath + '/' + fileName;
        if (!File.Exists(filePathFull)) {
            Debug.LogWarning("File was not found at specified path! " + filePathFull);
            return null;
        }
        string[] lines = File.ReadAllLines(filePathFull);
        if (lines != null) {
            //Debug.LogWarning("File successfully read!");
        } else {
            Debug.LogWarning("File could not be read!");
        }
        return lines;
    }

    public static bool WriteFile(string folderPath, string fileName, string[] linesToWrite) {
        string projectFolder = GetWorkingDirectory();
        string filePathFull = projectFolder + folderPath + '/' + fileName;
        if(!File.Exists(filePathFull)) {
            Debug.LogWarning("File was not found at specified path! " + filePathFull);
            return false;
        }
        File.AppendAllLines(filePathFull, linesToWrite);
        return true;
    }

    public static bool CreateFile(string folderPath, string fileName) {
        string projectFolder = GetWorkingDirectory();
        string folderPathFull = projectFolder + folderPath;
        if (!Directory.Exists(folderPathFull)) {
            Directory.CreateDirectory(folderPathFull);
        }
        string filePath = folderPathFull + '/' + fileName;
        File.Create(filePath);
        //Debug.Log("File created with success at: " + filePath);
        return true;

        //using(StreamWriter w = File.AppendText("log.txt"))
    }
}