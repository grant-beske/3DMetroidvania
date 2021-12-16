using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that performs the core functions of updating the player state.
public class CoreGameLogic : MonoBehaviour {

    // Using a shorter name here to keep line lengths clean.
    public PlayerState pS;

    void Update() {
        if (pS.coreStateValues.energy < pS.coreStateValues.energyCapacity) {
            pS.coreStateValues.energy +=
                (pS.coreStateValues.energyRechargeRate * Time.deltaTime);
            pS.coreStateValues.energy =
                Mathf.Min(pS.coreStateValues.energy, pS.coreStateValues.energyCapacity);
        }
    }
}
