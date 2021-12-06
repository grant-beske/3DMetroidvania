using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour {

    enum Visor {COMBAT, SCAN};
    private Visor activeVisor = Visor.COMBAT;

    public GameObject crosshair;
    public GameObject scanTarget;

    void Start() {
        scanTarget.SetActive(false);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            if (activeVisor == Visor.COMBAT) {
                activeVisor = Visor.SCAN;
                crosshair.SetActive(false);
                scanTarget.SetActive(true);
            } else if (activeVisor == Visor.SCAN) {
                activeVisor = Visor.COMBAT;
                crosshair.SetActive(true);
                scanTarget.SetActive(false);
            }
        }
    }
}
