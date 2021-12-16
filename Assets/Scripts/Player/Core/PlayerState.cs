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

    // Core location values represent the player's current in game location.
    // They do not need to be saved.
    public CoreLocationValues coreLocationValues;

    [System.Serializable]
    public class CoreStateValues {
        public float health = 50;
        public float healthCapacity = 100;
        public float energy = 0; 
        public float energyCapacity = 100;
        public float energyRechargeRate = 10f;
    }

    [System.Serializable]
    public class CoreLocationValues {
        public string currentArea = "Unknown";
        public string currentRoom = "Unknown";
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // MonoBehavior
    /////////////////////////////////////////////////////////////////////////////////////

    void Start() {
        generalStateDict = new Dictionary<string, string>();
        coreStateValues = new CoreStateValues();
        coreLocationValues = new CoreLocationValues();
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Public API
    /////////////////////////////////////////////////////////////////////////////////////

    public void SubtractEnergy(float quantity) {
        coreStateValues.energy = Mathf.Max(coreStateValues.energy - quantity, 0);
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Serialization / Deserialization API
    /////////////////////////////////////////////////////////////////////////////////////

    public JSONObject Serialize() {
        JSONObject coreStateValuesJson = new JSONObject();
        coreStateValuesJson.AddField("health", coreStateValues.health);
        coreStateValuesJson.AddField("healthCapacity", coreStateValues.healthCapacity);
        coreStateValuesJson.AddField("energy", coreStateValues.energy);
        coreStateValuesJson.AddField("energyCapacity", coreStateValues.energyCapacity);
        coreStateValuesJson.AddField("energyRechargeRate", coreStateValues.energyRechargeRate);

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
        coreStateValuesJson.GetField(
            out coreStateValues.energyRechargeRate,
            "energyRechargeRate",
            coreStateValues.energyRechargeRate);

        // When deserializing, also set the coreLocationValues from the loaded data.
        coreLocationValues.currentArea = saveFileArea;
        coreLocationValues.currentRoom = saveFileSceneName;

        generalStateDict = saveDataJson.GetField("generalStateDict").ToDictionary();
    }

    public string PrintState() {
        return Serialize().Print(true);
    }
}