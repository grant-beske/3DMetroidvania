using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerReferences : MonoBehaviour {
    public GameObject playerStateObj;
    [HideInInspector] public PlayerState playerState;

    void Start() {
        playerState = playerStateObj.GetComponent<PlayerState>();
    }
}
