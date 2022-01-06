using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceTriggerZone : MonoBehaviour {
    public string ambianceName;
    public AudioClip ambianceClip;

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Player") {
            AmbianceController ambianceController =
                col.gameObject.GetComponent<PlayerControllerReferences>().ambianceController;
            ambianceController.SetAmbiance(ambianceClip, ambianceName);
        }
    }
}
