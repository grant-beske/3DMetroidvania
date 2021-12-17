using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents the machine gun weapon powerup that you get in the Undersea Dock.
public class MachineGunPowerup : AbilityPowerup {
    public override void GivePlayerPowerup() {
        PlayerState playerState = GetPlayerState();
        playerState.coreStateValues.hasMachineGun = true;
        Destroy(gameObject);
    }
}
