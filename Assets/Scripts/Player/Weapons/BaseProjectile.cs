using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour {

    public float velocity;
    public float timeToLive;
    private float timeAlive;

    void Start() {
        timeAlive = 0;
    }

    // Update is called once per frame
    void Update() {
        timeAlive += Time.deltaTime;
        if (timeAlive >= timeToLive) {
            Destroy(gameObject);
        }
        transform.position += (transform.forward * velocity);
    }
}
