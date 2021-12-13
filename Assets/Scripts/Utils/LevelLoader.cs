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
        SceneManager.sceneUnloaded += HandleSceneUnloaded;
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
        SceneManager.sceneUnloaded -= HandleSceneUnloaded;
    }

    void Update() {
        if (!sceneLoaded) {
            sceneLoaded = true;
            SceneManager.LoadScene(targetSceneName, LoadSceneMode.Additive);
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode) {
        SceneManager.UnloadSceneAsync("Loader");
    }

    private void HandleSceneUnloaded(Scene scene) {
        GameObject dockingPoint = GetDockingPoint();
        player.transform.position = dockingPoint.transform.position;
        player.transform.rotation = dockingPoint.transform.rotation;
        uiController.FinishLoading();
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
