using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyWavecircle : Wavecircle {
    public override float GetMetricPercent() {
        return playerState.coreStateValues.energy / playerState.coreStateValues.energyCapacity;
    }

    public override string GetMetricText() {
        return Mathf.RoundToInt(playerState.coreStateValues.energy).ToString();
    }
}
