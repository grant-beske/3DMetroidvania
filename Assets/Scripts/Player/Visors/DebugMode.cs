using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMode : MonoBehaviour {

    [SerializeField] public Text debugOutput;

    void Start() {
        debugOutput.text = PlayerStateToLoad.state.Print(true);
    }
}
