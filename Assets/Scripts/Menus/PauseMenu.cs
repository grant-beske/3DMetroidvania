using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public GameObject uiControllerObj;
    private UserInterface uiController;

    void Start() {
        uiController = uiControllerObj.GetComponent<UserInterface>();
    }

    public void HandleResumeClick() {
        uiController.UnpauseGame();
    }

    public void HandleQuitToMenuClick() {
        DestroyPersistentObjects();
        SceneManager.LoadScene("MainMenu");
    }

    public void HandleQuitToDesktopClick() {
        Application.Quit();
    }

    private void DestroyPersistentObjects() {
        Destroy(GameObject.Find("Player"));
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("DoorSystem")) {
            Destroy(obj);
        }
    }
}
