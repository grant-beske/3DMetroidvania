using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMode : MonoBehaviour {

    public GameObject playerStateObj;
    private PlayerState playerState;
    [SerializeField] public Text debugOutput;
    private string textToAdd;

    void Start() {
        playerState = playerStateObj.GetComponent<PlayerState>();
        debugOutput.text = "";
    }

    void Update() {
        debugOutput.text = playerState.PrintState();
    }
}
