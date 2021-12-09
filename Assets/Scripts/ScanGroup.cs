using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanGroup : MonoBehaviour {

    // Scan description to display when objects in the group are scanned.
    public string description;

    public enum State {NORMAL, CRITICAL, SCANNED};
    public State initialState = State.NORMAL;
    private State activeState;

    void Start() {
        activeState = initialState;
    }

    public State GetActiveState() {
        return activeState;
    }

    public string ViewScan() {
        // Update the state to SCANNED.
        if (activeState == State.NORMAL || activeState == State.CRITICAL) {
            activeState = State.SCANNED;
        }
        return description;
    }
}
