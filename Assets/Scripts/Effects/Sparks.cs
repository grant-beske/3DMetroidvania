using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparks : MonoBehaviour {

    // Lighting variables
    public GameObject lightObj;

    // Particle system variables
    public GameObject[] particleSystems;

    // Timing variables
    public float minIntervalSec = 0.5f;
    public float maxIntervalSec = 10.0f;
    private float nextInterval;
    private float timeSinceLastSpark = 0.0f;

    void Start() {
        nextInterval = GetNewInterval();
    }

    void Update() {
        timeSinceLastSpark += Time.deltaTime;
        if (timeSinceLastSpark > nextInterval) {
            timeSinceLastSpark = 0.0f;
            nextInterval = GetNewInterval();
            Spark();
        }
    }

    private void Spark() {
        GetComponent<AudioSource>().Play();
        StartCoroutine(FlashLight());
        foreach (GameObject particleSystem in particleSystems) {
            particleSystem.GetComponent<ParticleSystem>().Play();
        }
    }

    private IEnumerator FlashLight() {
        lightObj.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        lightObj.SetActive(false);
    }

    private float GetNewInterval() {
        return Random.Range(minIntervalSec, maxIntervalSec);
    }
}
