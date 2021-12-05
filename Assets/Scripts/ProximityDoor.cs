using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityDoor : MonoBehaviour {

    // Doorway gameobject which will be translated on proximity trigger.
    public GameObject doorObject;

    // Audio clips to play when door opens or closes.
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;

    // If player enters the door proximity zone, then open the door.
    void OnTriggerEnter (Collider col) {
        if (col.gameObject.tag == "Player") {
            doorObject.transform.position += Vector3.down * 3.59f;
            GetComponent<AudioSource>().clip = doorOpenSound;
            GetComponent<AudioSource>().Play();
        }
    }

    // If player leaves the zone, then close the door.
    void OnTriggerExit (Collider col) {
        if (col.gameObject.tag == "Player") {
            doorObject.transform.position += Vector3.up * 3.59f;
            GetComponent<AudioSource>().clip = doorCloseSound;
            GetComponent<AudioSource>().Play();
        }
    }
}
