using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseGun : MonoBehaviour {

    public PlayerState playerState;

    public BaseProjectile projectile;
    public GameObject emitterPoint;

    public GameObject muzzleFlashObj;
    private ParticleSystem muzzleFlash;

    public AudioClip[] shotSounds;
    public AudioClip failToFireSound;
    public AudioCoordinator audioCoordinator;

    public enum FiringMode {SINGLE, AUTOMATIC};
    public FiringMode firingMode;
    public float energyCost;
    public float firingIntervalTime;
    public float projectileVelocity;
    public float projectileTimeToLive;
    private float timeSinceLastShot;

    private bool enableControl = true;
    
    /////////////////////////////////////////////////////////////////////////////////////
    // MonoBehavior
    /////////////////////////////////////////////////////////////////////////////////////

    void Start() {
        muzzleFlash = muzzleFlashObj.GetComponent<ParticleSystem>();
    }

    void Update() {
        if (enableControl) {
            if (firingMode == FiringMode.SINGLE) {
                UpdateSingleShot();
            } else if (firingMode == FiringMode.AUTOMATIC) {
               UpdateAutomaticShot();
            }
        }
    }

    private void UpdateSingleShot() {
        timeSinceLastShot += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && timeSinceLastShot >= firingIntervalTime) {
            timeSinceLastShot = 0;

            if (CanFire()) Fire();
            else FailToFire();
        }
    }

    private void UpdateAutomaticShot() {
        timeSinceLastShot += Time.deltaTime;
        if (Input.GetMouseButton(0) && timeSinceLastShot >= firingIntervalTime) {
            timeSinceLastShot = 0;

            if (CanFire()) Fire();
            else FailToFire();
        }
    }

    private bool CanFire() {
        return (energyCost <= playerState.coreStateValues.energy);
    }

    private void Fire() {
        SubtractEnergyCost();
        EmitProjectile();
        PlayMuzzleFlash();
        PlayShotSound();
    }

    private void SubtractEnergyCost() {
        playerState.SubtractEnergy(energyCost);
    }

    private void EmitProjectile() {
        BaseProjectile tempProjectile =
            Instantiate(projectile, emitterPoint.transform.position, emitterPoint.transform.rotation);
        tempProjectile.velocity = projectileVelocity;
        tempProjectile.timeToLive = projectileTimeToLive;
        tempProjectile.audioCoordinator = audioCoordinator;
    }

    private void PlayMuzzleFlash() {
        muzzleFlash.Play();
    }

    private void PlayShotSound() {
        audioCoordinator.PlaySound2D(shotSounds[Random.Range(0, shotSounds.Length)]);
    }

    private void FailToFire() {
        audioCoordinator.PlaySound2D(failToFireSound);
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Public API
    /////////////////////////////////////////////////////////////////////////////////////

    public void EnableControl() {
        enableControl = true;
    }

    public void DisableControl() {
        enableControl = false;
    }
}
