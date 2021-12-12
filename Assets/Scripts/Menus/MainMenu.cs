using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public Texture2D mouseCursor;

    void Start() {
        Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.Auto);
    }

    public void HandlePlayClick() {
        SceneManager.LoadScene("Loader");
    }

    public void HandleQuitToDesktopClick() {
        Application.Quit();
    }
}
