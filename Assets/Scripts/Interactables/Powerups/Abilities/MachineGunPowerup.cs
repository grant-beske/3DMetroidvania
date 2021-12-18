using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents the machine gun weapon powerup that you get in the Undersea Dock.
public class MachineGunPowerup : AbilityPowerup {
    public override bool IsPreviouslyObtained() {
        return GetPlayerState().coreStateValues.hasMachineGun;
    }

    public override void GivePlayerPowerup() {
        GetPlayerState().coreStateValues.hasMachineGun = true;
        Destroy(gameObject);
    }
}
