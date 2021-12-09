using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scannable : MonoBehaviour {
    // GameObject with a ScanGroup script attached.
    public GameObject parentScanGroupObj;
    private ScanGroup parentScanGroup;

    void Start() {
        parentScanGroup = parentScanGroupObj.GetComponent<ScanGroup>();
    }

    public string GetScan() {
        return parentScanGroup.description;
    }
}
