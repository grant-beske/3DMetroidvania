using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class SaveFileUtil {

    private static readonly bool shouldLogMissingFile = false;

    public static bool WriteToFile(JSONObject saveDataJson) {
        string saveFileName;
        saveDataJson.GetField(out saveFileName, "saveFileName", "Default");

        var fullPath = Path.Combine(Application.persistentDataPath, saveFileName);

        try {
            File.WriteAllText(fullPath, saveDataJson.Print(true));
            return true;
        } catch (Exception e) {
            if (shouldLogMissingFile) {
                Debug.Log($"Failed to write to {fullPath} with exception {e}");
            }
            return false;
        }
    }

    public static bool ReadFromFile(string saveFileName, out JSONObject saveDataJson) {
        var fullPath = Path.Combine(Application.persistentDataPath, saveFileName);

        try {
            string saveData = File.ReadAllText(fullPath);
            saveDataJson = new JSONObject(File.ReadAllText(fullPath));
            return true;
        } catch (Exception e) {
            if (shouldLogMissingFile) {
                Debug.Log($"Failed to read from {fullPath} with exception {e}");
            }
            saveDataJson = new JSONObject("{}");
            return false;
        }
    }

    public static void DeleteAllFiles() {
        var savePath1 = Path.Combine(Application.persistentDataPath, "SaveFile1");
        var savePath2 = Path.Combine(Application.persistentDataPath, "SaveFile2");
        var savePath3 = Path.Combine(Application.persistentDataPath, "SaveFile3");
        try {
            File.Delete(savePath1);
        } catch (Exception e) {
            if (shouldLogMissingFile) {
                Debug.Log($"Failed to delete from {savePath1} with exception {e}");
            }
        }
        try {
            File.Delete(savePath2);
        } catch (Exception e) {
            if (shouldLogMissingFile) {
                Debug.Log($"Failed to delete from {savePath2} with exception {e}");
            }
        }
        try {
            File.Delete(savePath2);
        } catch (Exception e) {
            if (shouldLogMissingFile) {
                Debug.Log($"Failed to delete from {savePath3} with exception {e}");
            }
        }
    }
}
