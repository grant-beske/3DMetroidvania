using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

    // List of (song, songLength, shouldLoop)
    private List<(AudioClip, float, bool)> songQueue;
    private float timePlaying = 0;

    private AudioSource audioSource;

    /////////////////////////////////////////////////////////////////////////////////////
    // MonoBehavior
    /////////////////////////////////////////////////////////////////////////////////////

    void Start() {
        audioSource = GetComponent<AudioSource>();
        songQueue = new List<(AudioClip, float, bool)>();
    }

    void Update() {
        if (songQueue.Count == 0) return;
        if (timePlaying < songQueue[0].Item2) {
            timePlaying += Time.deltaTime;
        } else {
            PlayNextSongOrLoop();
        }
    }

    private void PlayNextSongOrLoop() {
        if (songQueue.Count == 0) return;
        
        timePlaying = 0;

        // Only 1 song in queue - only loop if shouldLoop is true.
        if (songQueue.Count == 1) {
            if (songQueue[0].Item3) PlaySong(songQueue[0].Item1);
            else StopSong();
        // Multiple songs in queue - dequeue first song and play next.
        } else {
            songQueue.RemoveAt(0);
            PlaySong(songQueue[0].Item1);
        }
    }

    private void PlaySong(AudioClip song) {
        audioSource.Stop();
        audioSource.clip = song;
        audioSource.Play();
    }

    private void StopSong() {
        audioSource.Stop();
        songQueue.RemoveAt(0);
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Public API
    /////////////////////////////////////////////////////////////////////////////////////

    public void EnqueueSong(AudioClip songClip, float songLength, bool shouldLoop) {
        songQueue.Add((songClip, songLength, shouldLoop));
        // If this is the first song being enqueued, play it.
        if (songQueue.Count == 1) PlaySong(songClip);
    }

    public void PauseSong() {
        audioSource.Pause();
    }

    public void ResumeSong() {
        audioSource.Play();
    }
}
