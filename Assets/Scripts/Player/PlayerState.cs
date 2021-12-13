using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that can be directly serialized to represent a "save file".
[System.Serializable]
public class PlayerState : MonoBehaviour {

    // Save file related variables.
    public string saveFileName = "Default";
    public string saveFileArea = "Unknown";
    public string saveFileSceneName = "DronePoolA";

    // General purpose string-string dict. Used for lookup of fields that depend on
    // player state and should persist between rooms, like scan states.
    public Dictionary<string, string> generalStateDict;

    public CoreStateValues coreStateValues;

    [System.Serializable]
    public class CoreStateValues {
        public float health = 50;
        public float healthCapacity = 100;
        public float energy = 0; 
        public float energyCapacity = 100; 
    }

    void Start() {
        generalStateDict = new Dictionary<string, string>();
        coreStateValues = new CoreStateValues();
    }

    public JSONObject Serialize() {
        JSONObject coreStateValuesJson = new JSONObject();
        coreStateValuesJson.AddField("health", coreStateValues.health);
        coreStateValuesJson.AddField("healthCapacity", coreStateValues.healthCapacity);
        coreStateValuesJson.AddField("energy", coreStateValues.energy);
        coreStateValuesJson.AddField("energyCapacity", coreStateValues.energyCapacity);

        JSONObject generalStateDictJson = new JSONObject(generalStateDict);

        JSONObject saveDataJson = new JSONObject();
        saveDataJson.AddField("saveFileName", saveFileName);
        saveDataJson.AddField("saveFileArea", saveFileArea);
        saveDataJson.AddField("saveFileSceneName", saveFileSceneName);
        saveDataJson.AddField("coreStateValues", coreStateValuesJson);
        saveDataJson.AddField("generalStateDict", generalStateDictJson);

        return saveDataJson;
    }

    public void Deserialize(JSONObject saveDataJson) {
        saveDataJson.GetField(out saveFileName, "saveFileName", "Default");
        saveDataJson.GetField(out saveFileArea, "saveFileArea", "Unknown");
        saveDataJson.GetField(out saveFileSceneName, "saveFileSceneName", "DronePoolA");

        JSONObject coreStateValuesJson = saveDataJson.GetField("coreStateValues");
        coreStateValuesJson.GetField(out coreStateValues.health, "health", coreStateValues.health);
        coreStateValuesJson.GetField(
            out coreStateValues.healthCapacity, "healthCapacity", coreStateValues.healthCapacity);
        coreStateValuesJson.GetField(out coreStateValues.energy, "energy", coreStateValues.energy);
        coreStateValuesJson.GetField(
            out coreStateValues.energyCapacity, "energyCapacity", coreStateValues.energyCapacity);

        generalStateDict = saveDataJson.GetField("generalStateDict").ToDictionary();
    }
}
