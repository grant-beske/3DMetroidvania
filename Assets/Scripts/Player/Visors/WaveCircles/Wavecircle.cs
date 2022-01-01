using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Wavecircle : MonoBehaviour {
    public PlayerState playerState;

    public Transform wave;
    public Transform start, end;
    private Vector3 fullMagnitude;

    public Text metricText;

    void Start() {
        fullMagnitude = end.position - start.position;
    }

    void Update() {
        UpdatePercent();
    }

    public void UpdatePercent() {
        wave.position = start.position + (fullMagnitude * GetMetricPercent());
        metricText.text = GetMetricText();
    }

    public abstract float GetMetricPercent();

    public abstract string GetMetricText();
}
