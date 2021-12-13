using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Util class for loading screen. Loads a level and then positions player inside.
public class LevelLoader : MonoBehaviour {

    public GameObject player;
    public GameObject playerState;
    public GameObject uiControllerObj;
    private UserInterface uiController;
    private string targetSceneName;
    private bool sceneLoaded = false;

    void Start() {
        playerState.GetComponent<PlayerState>().Deserialize(PlayerStateToLoad.state);
        targetSceneName = playerState.GetComponent<PlayerState>().saveFileSceneName;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        if (player == null) {
            player = GetPlayer();
        }
        if (uiControllerObj == null) {
            uiControllerObj = GetUiControllerObj();
        }
        uiController = uiControllerObj.GetComponent<UserInterface>();
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    void Update() {
        if (!sceneLoaded) {
            sceneLoaded = true;
            SceneManager.LoadScene(targetSceneName, LoadSceneMode.Additive);
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode) {
        SceneManager.UnloadSceneAsync("Loader");
        GameObject dockingPoint = GetDockingPoint();
        player.transform.position = dockingPoint.transform.position;
        player.transform.rotation = dockingPoint.transform.rotation;
        uiController.FinishLoading();

        // Deserialize the save file again to make sure PlayerState Start() does not
        // override the coreStateValues and generalStateDict.
        playerState.GetComponent<PlayerState>().Deserialize(PlayerStateToLoad.state);
        Destroy(gameObject);
    }

    private GameObject GetDockingPoint() {
        return GameObject.Find("PlayerDockingPoint");
    }

    private GameObject GetPlayer() {
        return GameObject.Find("Player");
    }

    private GameObject GetUiControllerObj() {
        return GameObject.Find("UIController");
    }
}
