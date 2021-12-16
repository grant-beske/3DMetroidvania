using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsCoordinator : MonoBehaviour {

    // Gun control state.
    // ACTIVE - gun is onscreen and ready to be fired. Can switch weapons.
    // INACTIVE - gun is offscreen and can't be fired. Happens in other visors like scan.
    enum State {ACTIVE, INACTIVE};
    private State activeState = State.INACTIVE;

    // Gun object variables. Assumes the guns are in the following order:
    // 1. Laser Pistol - 2. Laser Rifle
    public GameObject[] gunObjects;
    private GameObject selectedGunObject;
    private int selectedGunIndex = 0;

    // Gun transitioning variables.
    public Transform gunActivePosition;
    public Transform gunInactivePosition;
    public float transitionTimeSec = 0.25f;

    /////////////////////////////////////////////////////////////////////////////////////
    // MonoBehavior
    /////////////////////////////////////////////////////////////////////////////////////

    void Start () {
        foreach (GameObject gun in gunObjects) {
            gun.SetActive(false);
        }
        // TODO - save the user's weapon from their last save and default to that.
        selectedGunObject = gunObjects[0];
        selectedGunIndex = 0;
    }

    void Update() {
        if (activeState == State.ACTIVE) {
            UpdateActiveState();
        }
    }

    private void UpdateActiveState() {
        // Switch to laser pistol
        if (Input.GetKeyDown(KeyCode.Alpha1) && selectedGunIndex != 0) {
            StartCoroutine(SwitchWeapons(gunObjects[0], 0));
        }

        // Switch to laser rifle
        if (Input.GetKeyDown(KeyCode.Alpha2) && selectedGunIndex != 1) {
            StartCoroutine(SwitchWeapons(gunObjects[1], 1));
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Public API
    /////////////////////////////////////////////////////////////////////////////////////

    private IEnumerator SwitchWeapons(GameObject gunToSwitch, int gunIndex) {
        yield return StartCoroutine(TransitionToInactiveState());
        selectedGunObject = gunToSwitch;
        selectedGunIndex = gunIndex;
        yield return StartCoroutine(TransitionToActiveState());
    }

    // These 2 functions are for when we want to basically equip / unequip the gun,
    // both functionally and visually.

    public void ActivateWeapon() {
        if (activeState != State.ACTIVE) {
            StartCoroutine(TransitionToActiveState());
        } else {
            // If weapon was already active, just re-enable control.
            selectedGunObject.GetComponent<BaseGun>().EnableControl();
        }
    }

    public void DeactivateWeapon() {
        if (activeState != State.INACTIVE) {
            StartCoroutine(TransitionToInactiveState());
        }
    }

    private IEnumerator TransitionToActiveState() {
        float timeElapsed = 0;
        selectedGunObject.transform.position = gunInactivePosition.position;
        Vector3 transitionDifference =
            gunActivePosition.position - gunInactivePosition.position;
        selectedGunObject.SetActive(true);
        while (timeElapsed < transitionTimeSec) {
            timeElapsed += Time.deltaTime;
            float percentComplete = timeElapsed / transitionTimeSec;
            selectedGunObject.transform.position =
                gunInactivePosition.position + (transitionDifference * percentComplete);
            yield return null;
        }
        selectedGunObject.transform.position = gunActivePosition.position;
        selectedGunObject.GetComponent<BaseGun>().EnableControl();
        activeState = State.ACTIVE;
    }

    private IEnumerator TransitionToInactiveState() {
        float timeElapsed = 0;
        selectedGunObject.transform.position = gunActivePosition.position;
        Vector3 transitionDifference =
            gunInactivePosition.position - gunActivePosition.position;
        selectedGunObject.GetComponent<BaseGun>().DisableControl();
        while (timeElapsed < transitionTimeSec) {
            timeElapsed += Time.deltaTime;
            float percentComplete = timeElapsed / transitionTimeSec;
            selectedGunObject.transform.position =
                gunActivePosition.position + (transitionDifference * percentComplete);
            yield return null;
        }
        selectedGunObject.transform.position = gunInactivePosition.position;
        selectedGunObject.SetActive(false);
        activeState = State.INACTIVE;
    }

    // These 2 functions are just for when we want to disable the firing of the gun.
    // Useful for some states such as visor select and pause.

    public void DeactivateWeaponControl() {
        selectedGunObject.GetComponent<BaseGun>().DisableControl();
    }

    public void ActivateWeaponControl() {
        selectedGunObject.GetComponent<BaseGun>().EnableControl();
    }
}
