using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScanGroup : MonoBehaviour {

    // ID of the scan group. MUST BE UNIQUE. Used to preserve scan data between scenes.
    public string scanId;
    private PlayerState playerState;

    // Scan description to display when objects in the group are scanned.
    public string description;

    // Callback event to trigger when the scan is finished.
    public UnityEvent triggerEvent;

    public enum State {NORMAL, CRITICAL, SCANNED};
    public State initialState = State.NORMAL;
    private State activeState;

    void Start() {
        playerState = GetPlayerState();
        activeState = IsPreviouslyScanned() ? State.SCANNED : initialState;
    }

    public State GetActiveState() {
        return activeState;
    }

    public string ViewScan() {
        // Update the state to SCANNED.
        if (activeState == State.NORMAL || activeState == State.CRITICAL) {
            activeState = State.SCANNED;
            SaveScanToPlayerState();
        }
        // If trigger event exists, then execute it.
        if (triggerEvent != null) {
            triggerEvent.Invoke();
        }
        return description;
    }

    private bool IsPreviouslyScanned() {
        return playerState.generalStateDict.ContainsKey(scanId);
    }

    private void SaveScanToPlayerState() {
        playerState.generalStateDict.TryAdd(scanId, "");
    }

    private PlayerState GetPlayerState() {
        return GameObject.Find("PlayerState").GetComponent<PlayerState>();
    }
}
