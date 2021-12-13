using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateForcefield : MonoBehaviour {

    // State persistence variables.
    private string id = "RoomScripts-UnderseaDock-DeactivateForcefield";
    private PlayerState playerState;

    public GameObject forcefieldObj;
    public AudioClip deactivateSound;
    
    private bool isActive = true;
    private bool deactivated = false;

    void Start() {
        GetComponent<AudioSource>().clip = deactivateSound;
        playerState = GetPlayerState();
        if (IsPreviouslyTriggered()) {
            DeactivateForPreviousTrigger();
        }
    }

    // Update is called once per frame
    void Update() {
        // Make sure time is moving. This ensures sound play after the scan is exited.
        if (!isActive && !deactivated && Time.timeScale != 0f) {
            deactivated = true;
            forcefieldObj.SetActive(false);
            GetComponent<AudioSource>().Play();
        }
    }

    public void Deactivate() {
        isActive = false;
        SaveTriggerToPlayerState();
    }

    private bool IsPreviouslyTriggered() {
        return playerState.generalStateDict.ContainsKey(id);
    }

    private void DeactivateForPreviousTrigger() {
        isActive = false;
        deactivated = true;
        forcefieldObj.SetActive(false);
    }

    private void SaveTriggerToPlayerState() {
        playerState.generalStateDict.TryAdd(id, "");
    }

    private PlayerState GetPlayerState() {
        return GameObject.Find("PlayerState").GetComponent<PlayerState>();
    }
}
