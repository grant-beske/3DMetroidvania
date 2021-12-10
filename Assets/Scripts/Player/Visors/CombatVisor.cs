using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatVisor : MonoBehaviour {

    // Player state variables to get data to display.
    public GameObject playerState;
    private PlayerState playerStateVars;

    public MetricElements metricElements;

    [System.Serializable]
    public class MetricElements {
        [SerializeField] public Text healthMetric;
        [HideInInspector] public float prevHealth;
        [SerializeField] public Text energyMetric;
        [HideInInspector] public float prevEnergy;
    }

    void Start() {
        playerStateVars = playerState.GetComponent<PlayerState>();
        InitHealth();
        InitEnergy();
    }

    void Update() {
        UpdateHealth();
        UpdateEnergy();
    }

    private void InitHealth() {
        // metricElements.prevHealth = playerStateVars.coreStateValues.health;
    }

    private void UpdateHealth() {
        // if (metricElements.prevHealth != playerStateVars.coreStateValues.health) {
            metricElements.prevHealth = playerStateVars.coreStateValues.health;
            metricElements.healthMetric.text = ((int) metricElements.prevHealth).ToString();
        // }
    }

    private void InitEnergy() {
        // metricElements.prevEnergy = playerStateVars.coreStateValues.energy;
    }

    private void UpdateEnergy() {
        // if (metricElements.prevEnergy != playerStateVars.coreStateValues.energy) {
            metricElements.prevEnergy = playerStateVars.coreStateValues.energy;
            metricElements.energyMetric.text = ((int) metricElements.prevEnergy).ToString();
        // }
    }
}
