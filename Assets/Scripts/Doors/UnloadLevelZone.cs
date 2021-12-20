using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnloadLevelZone : MonoBehaviour {
    public ProximityDoor proximityDoor;

    void OnTriggerExit(Collider col) {
        if (col.gameObject.tag == "Player") {
            proximityDoor.CloseDoorAndUnloadLevel();
        }
    }
}
