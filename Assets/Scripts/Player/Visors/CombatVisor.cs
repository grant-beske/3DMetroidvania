using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatVisor : MonoBehaviour {
    
    public UserInterface userInterface;

    void Update() {
        if (!userInterface.enableGameControls) return;
        if (Input.GetMouseButtonDown(2)) {
            userInterface.SetVisorSelect();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            userInterface.PauseGame();
        }
    }
}
