using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that can be directly serialized to represent a "save file".
public class PlayerState : MonoBehaviour {

    // General purpose string-bool dict. Used for lookup of fields that depend on player
    // state and should persist between rooms, like scan states.
    public Dictionary<string, bool> generalStateDict;

    public CoreStateValues coreStateValues;

    [System.Serializable]
    public class CoreStateValues {
        public float health = 50;
        public float healthCapacity = 100;
        public float energy = 0; 
        public float energyCapacity = 100; 
    }

    void Start() {
        generalStateDict = new Dictionary<string, bool>();
    }
}
