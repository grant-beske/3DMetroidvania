using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProximityDoor : MonoBehaviour {

    // Doorway gameobject which will be translated on proximity trigger.
    public GameObject doorObject;

    // Audio clips to play when door opens or closes.
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;

    // Variables to control the loaded scenes. 
    public string sceneName1;
    public GameObject level1Boundary;
    private LevelBoundary levelBoundaryState;
    private bool isScene1Loaded = true;
    public string sceneName2;
    private bool isScene2Loaded = false;

    void Awake() {
        levelBoundaryState = level1Boundary.GetComponent<LevelBoundary>();
    }

    // If player enters the door proximity zone, then open the door.
    void OnTriggerEnter (Collider col) {
        if (col.gameObject.tag == "Player") {
            if (!isScene1Loaded) {
                isScene1Loaded = true;
                SceneManager.LoadScene(sceneName1, LoadSceneMode.Additive);
            }
            if (!isScene2Loaded) {
                isScene2Loaded = true;
                SceneManager.LoadScene(sceneName2, LoadSceneMode.Additive);
            }
            doorObject.transform.position += Vector3.down * 3.59f;
            GetComponent<AudioSource>().clip = doorOpenSound;
            GetComponent<AudioSource>().Play();
        }
    }

    // If player leaves the zone, then close the door.
    void OnTriggerExit (Collider col) {
        if (col.gameObject.tag == "Player") {
            if (levelBoundaryState.IsPlayerInLevel()) {
                isScene2Loaded = false;
                SceneManager.UnloadSceneAsync(sceneName2);
            } else {
                isScene1Loaded = false;
                SceneManager.UnloadSceneAsync(sceneName1);
            }
            doorObject.transform.position += Vector3.up * 3.59f;
            GetComponent<AudioSource>().clip = doorCloseSound;
            GetComponent<AudioSource>().Play();
        }
    }
}
