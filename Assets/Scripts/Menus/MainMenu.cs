using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public Texture2D mouseCursor;

    public GameObject mainMenuObj;
    public GameObject selectSaveObj;
    [SerializeField] public Text slot1Text;
    [SerializeField] public Text slot2Text;
    [SerializeField] public Text slot3Text;

    // Save data objects
    private bool save1Loaded = false;
    private bool save2Loaded = false;
    private bool save3Loaded = false;
    private JSONObject saveData1;
    private JSONObject saveData2;
    private JSONObject saveData3;

    void Start() {
        Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.Auto);
        mainMenuObj.SetActive(true);
        selectSaveObj.SetActive(false);
        InitializeSaveData();

        // Initially application loading priority will be set to High to load the game
        // as fast as possible. Once in game this will be set to Low to make level
        // loading seamless, which happens in LevelLoader.
        Application.backgroundLoadingPriority = ThreadPriority.High;
    }

    public void HandlePlayClick() {
        mainMenuObj.SetActive(false);
        selectSaveObj.SetActive(true);
    }

    public void HandleQuitToDesktopClick() {
        Application.Quit();
    }

    public void HandleBackClick() {
        mainMenuObj.SetActive(true);
        selectSaveObj.SetActive(false);
    }

    public void HandleDeleteSaveDataClick() {
        SaveFileUtil.DeleteAllFiles();
        InitializeSaveData();
    }

    public void HandleLoadSlot1() {
        if (save1Loaded) {
            LoadSaveData(saveData1);
        } else {
            StartNewGame("SaveFile1");
        }
    }
    
    public void HandleLoadSlot2() {
        if (save2Loaded) {
            LoadSaveData(saveData2);
        } else {
            StartNewGame("SaveFile2");
        }
    }

    public void HandleLoadSlot3() {
        if (save3Loaded) {
            LoadSaveData(saveData3);
        } else {
            StartNewGame("SaveFile3");
        }
    }

    // Initialize file data for save select screen.
    private void InitializeSaveData() {
        save1Loaded = SaveFileUtil.ReadFromFile("SaveFile1", out saveData1);
        if (save1Loaded) {
            string saveFileArea;
            saveData1.GetField(out saveFileArea, "saveFileArea", "Unknown");
            slot1Text.text = " S L O T  1  (" + saveFileArea + ")";
        } else {
            slot1Text.text = " E M P T Y";
        }

        save2Loaded = SaveFileUtil.ReadFromFile("SaveFile2", out saveData2);
        if (save2Loaded) {
            string saveFileArea;
            saveData2.GetField(out saveFileArea, "saveFileArea", "Unknown");
            slot2Text.text = " S L O T  2  (" + saveFileArea + ")";
        } else {
            slot2Text.text = " E M P T Y";
        }

        save3Loaded = SaveFileUtil.ReadFromFile("SaveFile3", out saveData3);
        if (save3Loaded) {
            string saveFileArea;
            saveData3.GetField(out saveFileArea, "saveFileArea", "Unknown");
            slot3Text.text = " S L O T  3  (" + saveFileArea + ")";
        } else {
            slot3Text.text = " E M P T Y";
        }
    }

    // Take PlayerState from loader and update the values to the saved values.
    private void LoadSaveData(JSONObject targetSave) {
        PlayerStateToLoad.state = targetSave;
        SceneManager.LoadScene("Loader");
    }

    // Open loader scene and set saveFileName to slot name.
    private void StartNewGame(string slotName) {
        PlayerState targetPlayerState = GetComponent<PlayerState>();
        targetPlayerState.saveFileName = slotName;
        // Player starts out in Undersea Dock - Passenger Terminal.
        targetPlayerState.saveFileArea = "Undersea Dock";
        targetPlayerState.saveFileSceneName = "UD-PassengerTerminal";

        PlayerStateToLoad.state = targetPlayerState.Serialize();
        SceneManager.LoadScene("Loader");
    }
}
