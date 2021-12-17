using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCoordinator : MonoBehaviour {
    
    public AudioSource genericSfxPrefab;

    /////////////////////////////////////////////////////////////////////////////////////
    // Public API
    /////////////////////////////////////////////////////////////////////////////////////

    public void PlaySound2D(AudioClip audioClip) {
        AudioSource audioSource =
            Instantiate(genericSfxPrefab, transform.position, transform.rotation);
        // Set as a child of the AudioCoordinator.
        audioSource.gameObject.transform.parent = gameObject.transform;
        audioSource.spatialBlend = 0;
        audioSource.clip = audioClip;
        audioSource.Play();
        Destroy(audioSource.gameObject, audioClip.length);
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
}
