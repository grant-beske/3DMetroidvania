using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour {

    // Visor state variables.
    enum Visor {SELECT, COMBAT, SCAN};
    private Visor activeVisor = Visor.COMBAT;
    // Previously active visor, not counting technical visor states like SELECT.
    private Visor previouslyActiveVisor = Visor.COMBAT;
    // Enable / disable game control. Useful for displaying menus, etc.
    private bool enableGameControls = true;

    // Visor gameobjects.
    public GameObject visorSelect;
    public GameObject combatVisor;
    public GameObject scanVisor;
    private GameObject _activeVisorObj;

    // Gun gameobjects.
    public GameObject gunControlObj;
    private GenericGunController gunControl;

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
        // Initialize weapon behavior.
        gunControl = gunControlObj.GetComponent<GenericGunController>();

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
        if (enableGameControls) {
            if (activeVisor != Visor.SELECT && Input.GetMouseButtonDown(2)) {
                SetVisorSelect();
            } else if (activeVisor == Visor.SELECT && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(2))) {
                SetCombatVisor();
            }
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
        previouslyActiveVisor = Visor.COMBAT;
        _activeVisorObj.SetActive(false);
        _activeVisorObj = combatVisor;
        combatVisor.SetActive(true);

        // Disable any open weapons.
        gunControl.ActivateWeapon();

        SetCombatRenderMode();
    }

    public void SetScanVisor() {
        PlaySound(visorSelectSound);
        DisableCursor();
        activeVisor = Visor.SCAN;
        previouslyActiveVisor = Visor.SCAN;
        _activeVisorObj.SetActive(false);
        _activeVisorObj = scanVisor;
        scanVisor.SetActive(true);

        // Disable any open weapons.
        gunControl.DeactivateWeapon();

        SetScanRenderMode();
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

    public void EnableGameControls() {
        enableGameControls = true;
    }

    public void DisableGameControls() {
        enableGameControls = false;
    }

    public void ReInitRenderMode() {
        switch (previouslyActiveVisor) {
            case Visor.COMBAT:
                SetCombatRenderMode();
                break;
            case Visor.SCAN:
                SetScanRenderMode();
                break;
            default:
                SetCombatRenderMode();
                break;
        }
    }

    private void PlaySound(AudioClip clip) {
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }

    // Set the scan render mode by moving all objects tagged scannable into the
    // ScannableXXXXXX layer.
    private void SetScanRenderMode() {
        var scannables = GameObject.FindGameObjectsWithTag("Scannable");
        foreach (GameObject obj in scannables) {
            ScanGroup.State scanGroupState = obj.GetComponent<Scannable>().GetState();
            switch (scanGroupState) {
                case ScanGroup.State.NORMAL:
                    obj.layer = 10; // Layer 10: ScannableNormal
                    break;
                case ScanGroup.State.CRITICAL:
                    obj.layer = 11; // Layer 11: ScannableCritical
                    break;
                case ScanGroup.State.SCANNED:
                    obj.layer = 12; // Layer 12: ScannableScanned
                    break;
            }
        }
    }

    // Set the combat render mode by moving all scannable objects back to terrain layer.
    private void SetCombatRenderMode() {
        var scannables = GameObject.FindGameObjectsWithTag("Scannable");
        foreach (GameObject obj in scannables) {
            obj.layer = 6; // Layer 6: Scannable
        }
    }
}
