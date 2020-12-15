using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IOTools {

    public static string GetFullPath(string folderPath, string fileName = "") {
        string parentDirectoryName = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        //Debug.Log("Parent Directory: " + parentDirectoryName);
        return parentDirectoryName + folderPath + '/' + fileName;
    }

    public static string[] LoadFile(string folderPath, string fileName) {
        try {
            string filePathFull = GetFullPath(folderPath, fileName);
            if(!File.Exists(filePathFull)) {
                Debug.LogWarning("File was not found at specified path! " + filePathFull);
                return null;
            }
            return File.ReadAllLines(filePathFull);
        } catch(Exception e) {
            Debug.LogWarning("File could not be read.\n" + e);
            return null;
        }
    }

    public static bool WriteFile(string folderPath, string fileName, string[] linesToWrite) {
        try {
            string filePathFull = GetFullPath(folderPath, fileName);
            if(!File.Exists(filePathFull)) {
                if(!CreateFile(folderPath, fileName)) {
                    return false;
                }
            }
            File.WriteAllLines(filePathFull, linesToWrite);
            //Debug.Log("File was created and written at specified path: " + filePathFull);
            return true;
        } catch (Exception e) {
            Debug.LogWarning("File could not be written to.\n" + e);
            return false;
        }
    }

    public static bool CreateFile(string folderPath, string fileName) {
        try {
            string folderPathFull = GetFullPath(folderPath);
            if(!Directory.Exists(folderPathFull)) {
                Directory.CreateDirectory(folderPathFull);
            }
            string filePathFull = GetFullPath(folderPath, fileName);
            File.Create(filePathFull);
            return true;
        } catch (Exception e) {
            Debug.LogWarning("File could not be created.\n" + e);
            return false;
        }
    }
}