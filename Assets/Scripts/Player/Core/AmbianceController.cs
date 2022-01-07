using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceController : MonoBehaviour {

    private string ambianceName;
    private AudioClip ambianceClipCurrent;
    private AudioClip ambianceClipPrev;

    public AudioSource audioSourceA;
    public AudioSource audioSourceB;
    private bool isSourceAActive = true;

    public float crossfadeTime = 0.3f;

    /////////////////////////////////////////////////////////////////////////////////////
    // Public API
    /////////////////////////////////////////////////////////////////////////////////////

    public void SetAmbiance(AudioClip clip, string name) {
        if (ambianceName != name) {
            ambianceName = name;
            ambianceClipPrev = ambianceClipCurrent;
            ambianceClipCurrent = clip;
            if (isSourceAActive) {
                StartCoroutine(CrossfadeAmbiance(audioSourceA, audioSourceB));
            } else {
                StartCoroutine(CrossfadeAmbiance(audioSourceB, audioSourceA));
            }
            isSourceAActive = !isSourceAActive;
        }
    }

    private IEnumerator CrossfadeAmbiance(AudioSource fadeOut, AudioSource fadeIn) {
        fadeIn.clip = ambianceClipCurrent;
        fadeIn.volume = 0;
        fadeIn.Play();

        while (fadeIn.volume < 1.0f) {
            float increment = Time.deltaTime / crossfadeTime;
            fadeOut.volume -= increment;
            fadeIn.volume += increment;
            yield return null;
        }

        fadeIn.volume = 1.0f;
        fadeOut.volume = 0.0f;
        fadeOut.Stop();
    }
}
