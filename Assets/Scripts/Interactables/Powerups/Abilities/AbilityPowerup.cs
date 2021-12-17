using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityPowerup : MonoBehaviour {

    public float rotationDegPerSec = 200f;

    public GameObject coneTopObj;
    public GameObject coneBottomObj;
    public GameObject powerupObj;
    public Vector3 powerupRotationDirection = Vector3.forward;

    public AudioClip[] getPowerupSounds;

    void Update() {
        coneTopObj.transform.Rotate(
            Vector3.up * rotationDegPerSec * Time.deltaTime);
        powerupObj.transform.Rotate(
            powerupRotationDirection * rotationDegPerSec * Time.deltaTime);
        coneBottomObj.transform.Rotate(
            Vector3.up * rotationDegPerSec * Time.deltaTime);
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Player") {
            ObtainPowerup();
        }
    }

    private void ObtainPowerup() {
        PlayObtainPowerupSfx();
        GivePlayerPowerup();
    }

    private void PlayObtainPowerupSfx() {
        AudioCoordinator audioCoordinator = GetAudioCoordinator();
        foreach (AudioClip clip in getPowerupSounds) {
            audioCoordinator.PlaySound2D(clip);
        }
    }

    public abstract void GivePlayerPowerup();

    public PlayerState GetPlayerState() {
        return GameObject.Find("PlayerState").GetComponent<PlayerState>();
    }

    public AudioCoordinator GetAudioCoordinator() {
        return GameObject.Find("AudioCoordinator").GetComponent<AudioCoordinator>();
    }
}
