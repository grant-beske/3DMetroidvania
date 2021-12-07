using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour {

    // Visor state variables.
    enum Visor {SELECT, COMBAT, SCAN};
    private Visor activeVisor = Visor.COMBAT;

    // Visor gameobjects.
    public GameObject visorSelect;
    public GameObject combatVisor;
    public GameObject scanVisor;
    private GameObject _activeVisorObj;

    // Mouse cursor texture. Set on UI initialization.
    public Texture2D mouseCursor;

    // Variables to enable / disable mouse look when in menu.
    public GameObject mouseXObj;
    private MouseLook mouseXController;
    public GameObject mouseYObj;
    private MouseLook mouseYController;

    // Audio variables for menu and UI sounds.
    public AudioClip visorSelectSound;

    void Start() {
        // Initialize visor behavior.
        visorSelect.SetActive(false);
        scanVisor.SetActive(false);
        combatVisor.SetActive(true);
        _activeVisorObj = combatVisor;

        // Initialize cursor and mouse behavior.
        Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.Auto);
        mouseXController = mouseXObj.GetComponent<MouseLook>();
        mouseYController = mouseYObj.GetComponent<MouseLook>();
        DisableCursor();
    }

    void Update() {
        if (activeVisor != Visor.SELECT && Input.GetMouseButtonDown(2)) {
            SetVisorSelect();
        } else if (activeVisor == Visor.SELECT && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(2))) {
            SetCombatVisor();
        }
    }

    public void SetVisorSelect() {
        PlaySound(visorSelectSound);
        EnableCursor();
        activeVisor = Visor.SELECT;
        _activeVisorObj.SetActive(false);
        _activeVisorObj = visorSelect;
        visorSelect.SetActive(true);
    }

    public void SetCombatVisor() {
        DisableCursor();
        activeVisor = Visor.COMBAT;
        _activeVisorObj.SetActive(false);
        _activeVisorObj = combatVisor;
        combatVisor.SetActive(true);
    }

    public void SetScanVisor() {
        PlaySound(visorSelectSound);
        DisableCursor();
        activeVisor = Visor.COMBAT;
        _activeVisorObj.SetActive(false);
        _activeVisorObj = scanVisor;
        scanVisor.SetActive(true);
    }

    public void EnableCursor() {
        // TODO - reset cursor position to center of screen.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        // Disable mouse look while cursor is enabled.
        mouseXController.isEnabled = false;
        mouseYController.isEnabled = false;
    }

    public void DisableCursor() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // Enable mouse look while cursor is disabled.
        mouseXController.isEnabled = true;
        mouseYController.isEnabled = true;
    }

    private void PlaySound(AudioClip clip) {
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }
}
