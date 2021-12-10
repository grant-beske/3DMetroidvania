using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScanGroup : MonoBehaviour {

    // Scan description to display when objects in the group are scanned.
    public string description;

    // Callback event to trigger when the scan is finished.
    public UnityEvent triggerEvent;

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
        // If trigger event exists, then execute it.
        if (triggerEvent != null) {
            triggerEvent.Invoke();
        }
        return description;
    }
}
