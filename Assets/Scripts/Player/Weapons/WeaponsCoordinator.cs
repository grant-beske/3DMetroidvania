using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsCoordinator : MonoBehaviour {

    // Gun control state.
    // ACTIVE - gun is onscreen and ready to be fired.
    // TRANSITIONING - gun is being opened / closed and is moving.
    // INACTIVE - gun is offscreen and cannot be fired.
    enum State {ACTIVE, TRANSITIONING, INACTIVE};
    private State activeState = State.ACTIVE;

    // Transitioning state requires a target transform to move to.
    private Transform transitionEndPos;
    private Transform transitionStartPos;
    private Vector3 transitionDifference;
    private State transitionTargetState;
    public float transitionTimeSec = 0.25f;
    private float transitionTimeElapsed = 0.0f;

    // Gun objects.
    public GameObject gunObject;

    // Active and inactive gun positions.
    public GameObject gunActiveObj;
    private Transform gunActivePosition;
    public GameObject gunInactiveObj;
    private Transform gunInactivePosition;

    // TODO - generalize this interface to multiple guns.

    void Start() {
        gunInactivePosition = gunInactiveObj.transform;
        gunActivePosition = gunActiveObj.transform;
    }

    void Update() {
        if (activeState == State.TRANSITIONING) {
            UpdateTransitioningState();
        }
    }

    private void UpdateTransitioningState() {
        if (transitionTimeElapsed < transitionTimeSec) {
            transitionTimeElapsed += Time.deltaTime;
            float percentComplete = transitionTimeElapsed / transitionTimeSec;
            gunObject.transform.position =
                transitionStartPos.position + (percentComplete * transitionDifference);
        } else {
            // Transition has reached target state. Finalize transition.
            gunObject.transform.position = transitionEndPos.position;
            activeState = transitionTargetState;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Public API
    /////////////////////////////////////////////////////////////////////////////////////

    public void ActivateWeapon() {
        if (activeState != State.ACTIVE) {
            // Transition the weapon to the ACTIVE state.
            transitionTimeElapsed = 0.0f;
            activeState = State.TRANSITIONING;
            transitionTargetState = State.ACTIVE;
            transitionEndPos = gunActivePosition;
            transitionStartPos = gunInactivePosition;
            transitionDifference = gunActivePosition.position - gunInactivePosition.position;
        }
    }

    public void DeactivateWeapon() {
        if (activeState != State.INACTIVE) {
            // Transition the weapon to the INACTIVE state.
            transitionTimeElapsed = 0.0f;
            activeState = State.TRANSITIONING;
            transitionTargetState = State.INACTIVE;
            transitionEndPos = gunInactivePosition;
            transitionStartPos = gunActivePosition;
            transitionDifference = gunInactivePosition.position - gunActivePosition.position;
        }
    }
}
