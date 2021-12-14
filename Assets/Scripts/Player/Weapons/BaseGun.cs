using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseGun : MonoBehaviour {

    public BaseProjectile projectile;
    public GameObject emitterPoint;

    public AudioClip[] shotSounds;
    private AudioSource audioSource;

    public enum FiringMode {SINGLE, AUTOMATIC};
    public FiringMode firingMode;
    public float fireRatePerSec;
    public float projectileVelocity;
    public float projectileTimeToLive;
    private float timeSinceLastShot;

    private bool enableControl = true;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        if (enableControl) {
            if (firingMode == FiringMode.SINGLE) {
                UpdateSingleShot();
            } else if (firingMode == FiringMode.AUTOMATIC) {
                // TODO - implement automatic weapons
            }
        }
    }

    private void UpdateSingleShot() {
        timeSinceLastShot += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && timeSinceLastShot >= fireRatePerSec) {
            timeSinceLastShot = 0;
            Fire();
        }
    }

    public void Fire() {
        EmitProjectile();
        PlayShotSound();
    }

    private void EmitProjectile() {
        BaseProjectile tempProjectile =
            Instantiate(projectile, emitterPoint.transform.position, emitterPoint.transform.rotation);
        // TODO - figure out a less hacky way to get the correct projectile direction.
        tempProjectile.transform.Rotate(0, 90, 0);
        tempProjectile.velocity = projectileVelocity;
        tempProjectile.timeToLive = projectileTimeToLive;
    }

    private void PlayShotSound() {
        audioSource.clip = shotSounds[Random.Range(0, shotSounds.Length)];
        audioSource.Play();
    }

    public void EnableControl() {
        enableControl = true;
    }

    public void DisableControl() {
        enableControl = false;
    }
}
