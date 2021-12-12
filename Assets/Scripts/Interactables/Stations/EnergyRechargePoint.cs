using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyRechargePoint : MonoBehaviour {

    public float rechargeRate = 0.1f;
    private float timeSinceLastRecharge = 0.0f;

    void Update() {
        if (timeSinceLastRecharge <= rechargeRate) {
            timeSinceLastRecharge += Time.deltaTime;
        }
    }

    void OnTriggerStay(Collider col) {
        if (col.gameObject.tag == "Player" && timeSinceLastRecharge > rechargeRate) {
            timeSinceLastRecharge = 0;
            PlayerState playerState =
                col.gameObject.GetComponent<PlayerControllerReferences>().playerState;
            if (playerState.coreStateValues.energyCapacity > playerState.coreStateValues.energy) {
                playerState.coreStateValues.energy += 1;
            }
        }
    }
}
