using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisorSelect : MonoBehaviour {
    
    public UserInterface userInterface;
    
    void Update() {
        if (!userInterface.enableGameControls) return;
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(2)) {
            userInterface.SetCombatVisor();
        }
    }
}
