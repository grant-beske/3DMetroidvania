using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveStation : MonoBehaviour {

    public string enclosingArea;
    public string sceneName;

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Player") {
            SaveGame();
        }
    }

    private void SaveGame() {
        PlayerState playerState = GetPlayerState();
        playerState.saveFileArea = enclosingArea;
        playerState.saveFileSceneName = sceneName;
        JSONObject playerStateJson = playerState.Serialize();
        if (SaveFileUtil.WriteToFile(playerStateJson)) {
            GetOverlayMessages().TriggerSavedMessage();
        } else {
            // TODO - figure out what to do on save failure.
            Debug.Log("Save failed");
        }
    }

    private PlayerState GetPlayerState() {
        return GameObject.Find("PlayerState").GetComponent<PlayerState>();
    }

    private OverlayMessages GetOverlayMessages() {
        return GameObject.Find("OverlayMessages").GetComponent<OverlayMessages>();
    }
}
