using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProximityDoor : MonoBehaviour {

    // Variables to control the loaded scenes. 
    public string sceneName1;
    public GameObject level1Boundary;
    private LevelBoundary levelBoundaryState;
    public bool isStartingInScene1 = true;
    private bool isScene1Loaded;
    public string sceneName2;
    private bool isScene2Loaded;

    // Door animation variables.
    private int trDoorOpen = Animator.StringToHash("DoorOpen");
    private int trDoorClose = Animator.StringToHash("DoorClose");
    private Animator animator;
    private AudioSource audioSource;

	void Start() {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        isScene1Loaded = isStartingInScene1;
        isScene2Loaded = !isStartingInScene1;
	}

    void Awake() {
        levelBoundaryState = level1Boundary.GetComponent<LevelBoundary>();
    }

    // If player enters the door proximity zone, then open the door.
    void OnTriggerEnter (Collider col) {
        if (col.gameObject.tag == "Player") {
            audioSource.Play();
            animator.SetTrigger(trDoorOpen);
            if (!isScene1Loaded) {
                isScene1Loaded = true;
                SceneManager.LoadScene(sceneName1, LoadSceneMode.Additive);
                StartCoroutine(RenderNewScenesForCurrentVisor());
            }
            if (!isScene2Loaded) {
                isScene2Loaded = true;
                SceneManager.LoadScene(sceneName2, LoadSceneMode.Additive);
                StartCoroutine(RenderNewScenesForCurrentVisor());
            }
        }
    }

    // If player leaves the zone, then close the door.
    void OnTriggerExit (Collider col) {
        if (col.gameObject.tag == "Player") {
            audioSource.Play();
            animator.SetTrigger(trDoorClose);
            if (levelBoundaryState.IsPlayerInLevel()) {
                StartCoroutine(UnloadScene2AfterDoorClose());
            } else {
                StartCoroutine(UnloadScene1AfterDoorClose());
            }
        }
    }

    private IEnumerator UnloadScene1AfterDoorClose() {
        yield return new WaitForSeconds(0.75f);
        isScene1Loaded = false;
        SceneManager.UnloadSceneAsync(sceneName1);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName2));
    }

    private IEnumerator UnloadScene2AfterDoorClose() {
        yield return new WaitForSeconds(0.75f);
        isScene2Loaded = false;
        SceneManager.UnloadSceneAsync(sceneName2);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName1));
    }

    private IEnumerator RenderNewScenesForCurrentVisor() {
        // Wait for a tenth of a sec to give the scene time to load.
        yield return new WaitForSeconds(0.1f);
        GameObject.Find("UIController").GetComponent<UserInterface>().ReInitRenderMode();
    }
}
