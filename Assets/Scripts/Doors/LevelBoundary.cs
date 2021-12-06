using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBoundary : MonoBehaviour {

    private bool playerInLevel = false;

    void OnTriggerEnter (Collider col) {
        if (col.gameObject.tag == "Player") {
            playerInLevel = true;
        }
    }

    void OnTriggerExit (Collider col) {
        if (col.gameObject.tag == "Player") {
            playerInLevel = false;
        }
    }

    public bool IsPlayerInLevel() {
        return playerInLevel;
    }
}
