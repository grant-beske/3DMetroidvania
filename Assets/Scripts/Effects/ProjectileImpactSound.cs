using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileImpactSound : MonoBehaviour {
    
    public AudioClip clip;

    void Start() {
        GetComponent<AudioSource>().Play();
        Destroy(gameObject, clip.length);
    }
}
