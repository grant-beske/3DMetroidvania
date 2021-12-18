using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityPowerup : MonoBehaviour {

    public float rotationDegPerSec = 200f;

    private PlayerState playerState;

    public string[] powerupDialogMessages = new string[]{"New ability acquired."};

    public GameObject coneTopObj;
    public GameObject coneBottomObj;
    public GameObject powerupObj;
    public GameObject getPowerupVfxPrefab;
    public Vector3 powerupRotationDirection = Vector3.forward;

    public AudioClip[] getPowerupSounds;

    void Start() {
        if (IsPreviouslyObtained()) Destroy(gameObject);
    }

    public abstract bool IsPreviouslyObtained();

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
        PlayObtainPowerupVfx();
        PlayObtainPowerupSfx();
        GivePlayerPowerup();
        GetUserInterface().PlayPowerupDialogMessage(powerupDialogMessages, 0.5f);
        Destroy(gameObject);
    }

    private void PlayObtainPowerupVfx() {
        GameObject vfx =
            Instantiate(getPowerupVfxPrefab, transform.position, transform.rotation);
        Destroy(vfx, 1.0f);
    }

    private void PlayObtainPowerupSfx() {
        AudioCoordinator audioCoordinator = GetAudioCoordinator();
        foreach (AudioClip clip in getPowerupSounds) {
            audioCoordinator.PlaySound2D(clip);
        }
    }

    public abstract void GivePlayerPowerup();

    public PlayerState GetPlayerState() {
        if (playerState != null) return playerState;
        playerState = GameObject.Find("PlayerState").GetComponent<PlayerState>();
        return playerState;
    }

    public UserInterface GetUserInterface() {
        return GameObject.Find("UIController").GetComponent<UserInterface>();
    }

    public AudioCoordinator GetAudioCoordinator() {
        return GameObject.Find("AudioCoordinator").GetComponent<AudioCoordinator>();
    }
}
