using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthWavecircle : Wavecircle {
    public override float GetMetricPercent() {
        return playerState.coreStateValues.health / playerState.coreStateValues.healthCapacity;
    }

    public override string GetMetricText() {
        return Mathf.RoundToInt(playerState.coreStateValues.health).ToString();
    }
}
