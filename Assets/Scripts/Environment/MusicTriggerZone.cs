using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTriggerZone : MonoBehaviour {
    public string musicName;
    public AudioClip musicClip;

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Player") {
            MusicController musicController =
                col.gameObject.GetComponent<PlayerControllerReferences>().musicController;
            musicController.EnqueueSong(musicClip, musicClip.length, true, musicName);
        }
    }
}
