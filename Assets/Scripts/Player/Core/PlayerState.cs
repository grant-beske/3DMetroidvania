using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that can be directly serialized to represent a "save file". The default values
// used in the inner classes represent the starting state of the game.
[System.Serializable]
public class PlayerState : MonoBehaviour {

    // Save file related variables.
    public string saveFileName = "Default";
    public string saveFileArea = "Unknown";
    public string saveFileSceneName = "UD-PassengerTerminal";

    // General purpose string-string dict. Used for lookup of fields that depend on
    // player state and should persist between rooms, like scan states.
    public Dictionary<string, string> generalStateDict;

    public CoreStateValues coreStateValues;

    // Core location values represent the player's current in game location.
    // They do not need to be saved.
    public CoreLocationValues coreLocationValues;

    // Variables related to weapons.
    public string selectedGunName = "Laser Pistol";

    [System.Serializable]
    public class CoreStateValues {
        // Numeric items representing player stats.
        public float health = 50;
        public float healthCapacity = 100;
        public float energy = 0; 
        public float energyCapacity = 100;
        public float energyRechargeRate = 10f;

        // Booleans representing abilities the player has gotten.
        // Visors
        public bool hasCombatVisor = true;
        public bool hasScanVisor = false;
        // Weapons
        public bool hasPistol = true;
        public bool hasMachineGun = false;
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
        JSONObject coreStateJson = new JSONObject();
        coreStateJson.AddField("health", coreStateValues.health);
        coreStateJson.AddField("healthCapacity", coreStateValues.healthCapacity);
        coreStateJson.AddField("energy", coreStateValues.energy);
        coreStateJson.AddField("energyCapacity", coreStateValues.energyCapacity);
        coreStateJson.AddField("energyRechargeRate", coreStateValues.energyRechargeRate);

        // Abilities
        coreStateJson.AddField("hasCombatVisor", coreStateValues.hasCombatVisor);
        coreStateJson.AddField("hasScanVisor", coreStateValues.hasScanVisor);
        coreStateJson.AddField("hasPistol", coreStateValues.hasPistol);
        coreStateJson.AddField("hasMachineGun", coreStateValues.hasMachineGun);

        JSONObject generalStateDictJson = new JSONObject(generalStateDict);

        JSONObject saveDataJson = new JSONObject();
        saveDataJson.AddField("saveFileName", saveFileName);
        saveDataJson.AddField("saveFileArea", saveFileArea);
        saveDataJson.AddField("saveFileSceneName", saveFileSceneName);
        saveDataJson.AddField("coreStateValues", coreStateJson);
        saveDataJson.AddField("generalStateDict", generalStateDictJson);

        return saveDataJson;
    }

    public void Deserialize(JSONObject saveDataJson) {
        saveDataJson.GetField(out saveFileName, "saveFileName", "Default");
        saveDataJson.GetField(out saveFileArea, "saveFileArea", "Unknown");
        saveDataJson.GetField(out saveFileSceneName, "saveFileSceneName", "DronePoolA");

        JSONObject coreStateJson = saveDataJson.GetField("coreStateValues");
        coreStateJson.GetField(out coreStateValues.health, "health", coreStateValues.health);
        coreStateJson.GetField(
            out coreStateValues.healthCapacity, "healthCapacity", coreStateValues.healthCapacity);
        coreStateJson.GetField(out coreStateValues.energy, "energy", coreStateValues.energy);
        coreStateJson.GetField(
            out coreStateValues.energyCapacity, "energyCapacity", coreStateValues.energyCapacity);
        coreStateJson.GetField(
            out coreStateValues.energyRechargeRate,
            "energyRechargeRate",
            coreStateValues.energyRechargeRate);

        // Abilities
        coreStateJson.GetField(
            out coreStateValues.hasCombatVisor, "hasCombatVisor", coreStateValues.hasCombatVisor);
        coreStateJson.GetField(
            out coreStateValues.hasScanVisor, "hasScanVisor", coreStateValues.hasScanVisor);
        coreStateJson.GetField(
            out coreStateValues.hasPistol, "hasPistol", coreStateValues.hasPistol);
        coreStateJson.GetField(
            out coreStateValues.hasMachineGun, "hasMachineGun", coreStateValues.hasMachineGun);


        // When deserializing, also set the coreLocationValues from the loaded data.
        coreLocationValues.currentArea = saveFileArea;
        coreLocationValues.currentRoom = saveFileSceneName;

        generalStateDict = saveDataJson.GetField("generalStateDict").ToDictionary();
    }

    public string PrintState() {
        return Serialize().Print(true);
    }
}
