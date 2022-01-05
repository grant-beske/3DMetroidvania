using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProximityDoor : MonoBehaviour {

    // Variables to control the loaded scenes. 
    public string sceneName1;
    public string sceneName2;
    private bool isScene1Loaded;
    private bool isScene2Loaded;
    private bool isDoorOpen = false;

    public LevelBoundary level1Boundary;
    public bool isStartingInScene1 = true;

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

    void Update() {
        // Self destruct if neither of the two scenes are loaded.
        if (!SceneManager.GetSceneByName(sceneName1).isLoaded 
                && !SceneManager.GetSceneByName(sceneName2).isLoaded) {
            Destroy(gameObject);
        }
    }

    // If player enters the door proximity zone, then open the door.
    void OnTriggerEnter (Collider col) {
        if (col.gameObject.tag == "Player" && !isDoorOpen) {
            if (CanLoadScene(isScene1Loaded, sceneName1)) {
                isScene1Loaded = true;
                StartCoroutine(LoadNewScene(sceneName1));
            }
            if (CanLoadScene(isScene2Loaded, sceneName2)) {
                isScene2Loaded = true;
                StartCoroutine(LoadNewScene(sceneName2));
            }
        }
    }

    private bool CanLoadScene(bool isSceneLoaded, string sceneName) {
        if (!isSceneLoaded && SceneManager.GetSceneByName(sceneName).IsValid()) {
            // TODO - investigate this case. 99% sure it happens when the next level is
            // loaded and the door opening collision function on the duplicate door is
            // triggered before PreserveBetweenScenes runs and destroys the duplicate
            // door.
            Debug.Log("Glitch case");
        }
        return !isSceneLoaded && !SceneManager.GetSceneByName(sceneName).IsValid();
    }

    private IEnumerator LoadNewScene(string sceneName) {
        AsyncOperation asyncLoad =
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return asyncLoad;

        audioSource.Play();
        animator.SetTrigger(trDoorOpen);
        isDoorOpen = true;

        // Wait 1 frame before rendering for current visor to give ScanGroups
        // time to initialize.
        yield return null;
        RenderNewScenesForCurrentVisor();
    }

    public void CloseDoorAndUnloadLevel() {
        if (isDoorOpen) {
            isDoorOpen = false;
            audioSource.Play();
            animator.SetTrigger(trDoorClose);
            if (level1Boundary.IsPlayerInLevel()) {
                StartCoroutine(UnloadScene2AfterDoorClose());
            } else {
                StartCoroutine(UnloadScene1AfterDoorClose());
            }
        }
    }

    // TODO - the following 2 functions seem buggy.
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

    private void RenderNewScenesForCurrentVisor() {
        GameObject.Find("UIController").GetComponent<UserInterface>().ReInitRenderMode();
    }
}
