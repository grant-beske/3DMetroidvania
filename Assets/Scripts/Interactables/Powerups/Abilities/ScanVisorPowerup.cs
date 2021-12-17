using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents the scan visor powerup that you get in the Undersea Dock.
// This is also the first powerup ability in the game.
public class ScanVisorPowerup : AbilityPowerup {
    public override void GivePlayerPowerup() {
        PlayerState playerState = GetPlayerState();
        playerState.coreStateValues.hasScanVisor = true;
        Destroy(gameObject);
    }
}
