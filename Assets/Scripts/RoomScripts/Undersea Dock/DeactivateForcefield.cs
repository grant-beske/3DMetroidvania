using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateForcefield : MonoBehaviour {

    public GameObject forcefieldObj;
    public AudioClip deactivateSound;
    
    private bool isActive = true;
    private bool deactivated = false;

    void Start() {
        GetComponent<AudioSource>().clip = deactivateSound;
    }

    // Update is called once per frame
    void Update() {
        if (!isActive && !deactivated && Time.timeScale != 0f) {
            deactivated = true;
            forcefieldObj.SetActive(false);
            GetComponent<AudioSource>().Play();
        }
    }

    public void Deactivate() {
        isActive = false;
    }
}
