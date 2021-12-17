using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCoordinator : MonoBehaviour {
    
    // SFX Prefab for 3D SFX, will be positioned in scene.
    public AudioSource genericSfxPrefab;

    // UI AudioSource with ignoreListenerPause set to true.
    public AudioSource uiAudioSource;

    /////////////////////////////////////////////////////////////////////////////////////
    // MonoBehavior
    /////////////////////////////////////////////////////////////////////////////////////

    void Start() {
        uiAudioSource = GetComponent<AudioSource>();
        uiAudioSource.ignoreListenerPause = true;
    }
    
    /////////////////////////////////////////////////////////////////////////////////////
    // Public API
    /////////////////////////////////////////////////////////////////////////////////////

    public void PlaySound2D(AudioClip audioClip) {
        uiAudioSource.PlayOneShot(audioClip);
    }

    public void PlaySound3D(AudioClip audioClip, Vector3 pos) {
        AudioSource audioSource =
            Instantiate(genericSfxPrefab, pos, transform.rotation);
        // TODO - set 3D audio position level-relative, to not move with character.
        audioSource.gameObject.transform.parent = gameObject.transform;
        audioSource.clip = audioClip;
        audioSource.Play();
        Destroy(audioSource.gameObject, audioClip.length);
    }

    public void PauseSounds() {
        AudioListener.pause = true;
    }

    public void ResumeSounds() {
        AudioListener.pause = false;
    }
}
