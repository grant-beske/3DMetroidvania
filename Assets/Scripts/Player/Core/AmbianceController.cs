using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceController : MonoBehaviour {

    private string ambianceName;
    private AudioClip ambianceClip;
    private AudioSource audioSource;

    /////////////////////////////////////////////////////////////////////////////////////
    // MonoBehavior
    /////////////////////////////////////////////////////////////////////////////////////

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Public API
    /////////////////////////////////////////////////////////////////////////////////////

    public void SetAmbiance(AudioClip clip, string name) {
        if (ambianceName != name) {
            ambianceName = name;
            audioSource.Stop();
            ambianceClip = clip;
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
