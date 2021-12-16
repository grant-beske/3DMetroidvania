using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsCoordinator : MonoBehaviour {

    // Gun control state.
    // ACTIVE - gun is onscreen and ready to be fired. Can switch weapons.
    // INACTIVE - gun is offscreen and can't be fired. Happens in other visors like scan.
    enum State {ACTIVE, INACTIVE};
    private State activeState = State.ACTIVE;

    // Gun objects.
    public GameObject gunObject;

    // Gun transitioning variables.
    public Transform gunActivePosition;
    public Transform gunInactivePosition;
    public float transitionTimeSec = 0.25f;

    void Update() {
        if (activeState == State.ACTIVE) {
            UpdateActiveState();
        }
    }

    private void UpdateActiveState() {
        // TODO - generalize this interface to multiple guns.
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Public API
    /////////////////////////////////////////////////////////////////////////////////////

    // These 2 functions are for when we want to basically equip / unequip the gun,
    // both functionally and visually.

    public void ActivateWeapon() {
        if (activeState != State.ACTIVE) {
            StartCoroutine(TransitionToActiveState());
        } else {
            // If weapon was already active, just re-enable control.
            gunObject.GetComponent<BaseGun>().EnableControl();
        }
    }

    private IEnumerator TransitionToActiveState() {
        float timeElapsed = 0;
        gunObject.transform.position = gunInactivePosition.position;
        Vector3 transitionDifference =
            gunActivePosition.position - gunInactivePosition.position;
        gunObject.SetActive(true);
        while (timeElapsed < transitionTimeSec) {
            timeElapsed += Time.deltaTime;
            float percentComplete = timeElapsed / transitionTimeSec;
            gunObject.transform.position =
                gunInactivePosition.position + (transitionDifference * percentComplete);
            yield return null;
        }
        gunObject.transform.position = gunActivePosition.position;
        gunObject.GetComponent<BaseGun>().EnableControl();
        activeState = State.ACTIVE;
    }

    public void DeactivateWeapon() {
        if (activeState != State.INACTIVE) {
            StartCoroutine(TransitionToInactiveState());
        }
    }

    private IEnumerator TransitionToInactiveState() {
        float timeElapsed = 0;
        gunObject.transform.position = gunActivePosition.position;
        Vector3 transitionDifference =
            gunInactivePosition.position - gunActivePosition.position;
        gunObject.GetComponent<BaseGun>().DisableControl();
        while (timeElapsed < transitionTimeSec) {
            timeElapsed += Time.deltaTime;
            float percentComplete = timeElapsed / transitionTimeSec;
            gunObject.transform.position =
                gunActivePosition.position + (transitionDifference * percentComplete);
            yield return null;
        }
        gunObject.transform.position = gunInactivePosition.position;
        gunObject.SetActive(false);
        activeState = State.INACTIVE;
    }

    // These functions are just for when we want to disable the firing of the gun.
    // Useful for some states such as visor select and pause.

    public void DeactivateWeaponControl() {
        gunObject.GetComponent<BaseGun>().DisableControl();
    }

    public void ActivateWeaponControl() {
        gunObject.GetComponent<BaseGun>().EnableControl();
    }
}
