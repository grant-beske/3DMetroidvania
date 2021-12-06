using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanVisor : MonoBehaviour {

    // Scan crosshair gameobjects
    public GameObject scanTargetBlank;
    public GameObject scanTargetHighlighted;
    public GameObject scanTargetSelected;

    void Start() {
        scanTargetBlank.SetActive(true);
        scanTargetHighlighted.SetActive(false);
        scanTargetSelected.SetActive(false);
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            if (Physics.Raycast(
                    Camera.main.transform.position,
                    Camera.main.transform.forward,
                    out hit,
                    Mathf.Infinity,
                    1 << 6, // Only look at layer 6: Scannable
                    QueryTriggerInteraction.Ignore)) {
                Debug.Log(hit.collider.name);
            }
        } else {
            RaycastHit hit;
            if (Physics.Raycast(
                    Camera.main.transform.position,
                    Camera.main.transform.forward,
                    out hit,
                    Mathf.Infinity,
                    1 << 6, // Only look at layer 6: Scannable
                    QueryTriggerInteraction.Ignore)) {
                scanTargetBlank.SetActive(false);
                scanTargetHighlighted.SetActive(true);
            } else {
                scanTargetHighlighted.SetActive(false);
                scanTargetBlank.SetActive(true);
            }
        }
    }
}
