using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour {

    public CoreStateValues coreStateValues;

    [System.Serializable]
    public class CoreStateValues {
        public float health = 50;
        public float healthCapacity = 100;
        public float energy = 0; 
        public float energyCapacity = 100; 
    }
}
