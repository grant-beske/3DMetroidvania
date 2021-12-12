using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Util class for loading screen. Loads a level and then positions player inside.
public class LevelLoader : MonoBehaviour {

    public GameObject player;
    public string targetSceneName;
    private bool sceneLoaded = false;
    private bool shouldUnload = false;

    void Start() {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    void Update() {
        if (!sceneLoaded) {
            sceneLoaded = true;
            SceneManager.LoadScene(targetSceneName, LoadSceneMode.Additive);
        }
        if (shouldUnload) {
            SceneManager.UnloadSceneAsync("Loader");
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode) {
        player.transform.position = GetDockingPoint().transform.position;
        shouldUnload = true;
    }

    private GameObject GetDockingPoint() {
        return GameObject.Find("PlayerDockingPoint");
    }
}
