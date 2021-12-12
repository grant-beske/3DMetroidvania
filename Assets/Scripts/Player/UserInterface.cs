using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour {

    // Visor state variables.
    enum Visor {LOADING, SELECT, COMBAT, SCAN};
    private Visor activeVisor = Visor.LOADING;
    // Previously active visor, not counting technical visor states like SELECT.
    private Visor previouslyActiveVisor = Visor.COMBAT;
    // Enable / disable game control. Useful for displaying menus, etc.
    private bool enableGameControls = true;

    // Gun gameobjects.
    public GameObject gunControlObj;
    private GenericGunController gunControl;

    // Contains visor related variables.
    public VisorControlVars visorControlVars;

    // Contains mouse control variables.
    public MouseControlVars mouseControlVars;

    [System.Serializable]
    public class VisorControlVars {
        // Visor gameobjects.
        public GameObject loadingState;
        public GameObject visorSelect;
        public GameObject combatVisor;
        public GameObject scanVisor;
        [HideInInspector] public GameObject activeVisorObj;
        public AudioClip visorSelectSound;
    }
    
    [System.Serializable]
    public class MouseControlVars {
        // Variables to enable / disable mouse look when in menu.
        public GameObject mouseXObj;
        [HideInInspector] public MouseLook mouseXController;
        public GameObject mouseYObj;
        [HideInInspector] public MouseLook mouseYController;

        // Mouse cursor texture. Set on UI initialization.
        public Texture2D mouseCursor;
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // MonoBehavior Functions
    /////////////////////////////////////////////////////////////////////////////////////

    void Start() {
        // Initialize weapon behavior.
        gunControl = gunControlObj.GetComponent<GenericGunController>();

        // Initialize visor behavior. Default to LOADING. The LevelLoader script will
        // handle changing state from LOADING to COMBAT.
        visorControlVars.loadingState.SetActive(true);
        visorControlVars.visorSelect.SetActive(false);
        visorControlVars.scanVisor.SetActive(false);
        visorControlVars.combatVisor.SetActive(false);
        visorControlVars.activeVisorObj = visorControlVars.loadingState;

        // Initialize cursor and mouse behavior.
        Cursor.SetCursor(mouseControlVars.mouseCursor, Vector2.zero, CursorMode.Auto);
        mouseControlVars.mouseXController = mouseControlVars.mouseXObj.GetComponent<MouseLook>();
        mouseControlVars.mouseYController = mouseControlVars.mouseYObj.GetComponent<MouseLook>();
        DisableCursor();
    }

    void Update() {
        // Dispatch UI behavior based on currently active visor aka state.
        if (activeVisor == Visor.LOADING) {
            // Loading is currently a noop.
        } else if (activeVisor == Visor.SELECT) {
            UpdateVisorSelect();
        } else if (activeVisor == Visor.COMBAT) {
            UpdateCombatVisor();
        } else if (activeVisor == Visor.SCAN) {
            UpdateScanVisor();
        }
    }

    private void UpdateVisorSelect() {
        if (!enableGameControls) return;
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(2)) {
            SetCombatVisor();
        }
    }

    private void UpdateCombatVisor() {
        if (!enableGameControls) return;
        if (Input.GetMouseButtonDown(2)) {
            SetVisorSelect();
        }
    }

    private void UpdateScanVisor() {
        if (!enableGameControls) return;
        if (Input.GetMouseButtonDown(2)) {
            SetVisorSelect();
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Game logic functions
    /////////////////////////////////////////////////////////////////////////////////////

    public void FinishLoading() {
        SetCombatVisor();
    }

    public void SetVisorSelect() {
        PlaySound(visorControlVars.visorSelectSound);
        EnableCursor();
        activeVisor = Visor.SELECT;
        visorControlVars.activeVisorObj.SetActive(false);
        visorControlVars.activeVisorObj = visorControlVars.visorSelect;
        visorControlVars.visorSelect.SetActive(true);
    }

    public void SetCombatVisor() {
        DisableCursor();
        activeVisor = Visor.COMBAT;
        previouslyActiveVisor = Visor.COMBAT;
        visorControlVars.activeVisorObj.SetActive(false);
        visorControlVars.activeVisorObj = visorControlVars.combatVisor;
        visorControlVars.combatVisor.SetActive(true);

        // Disable any open weapons.
        gunControl.ActivateWeapon();

        SetCombatRenderMode();
    }

    public void SetScanVisor() {
        PlaySound(visorControlVars.visorSelectSound);
        DisableCursor();
        activeVisor = Visor.SCAN;
        previouslyActiveVisor = Visor.SCAN;
        visorControlVars.activeVisorObj.SetActive(false);
        visorControlVars.activeVisorObj = visorControlVars.scanVisor;
        visorControlVars.scanVisor.SetActive(true);

        // Disable any open weapons.
        gunControl.DeactivateWeapon();

        SetScanRenderMode();
    }

    public void EnableCursor() {
        // TODO - reset cursor position to center of screen.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        // Disable mouse look while cursor is enabled.
        mouseControlVars.mouseXController.isEnabled = false;
        mouseControlVars.mouseYController.isEnabled = false;
    }

    public void DisableCursor() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // Enable mouse look while cursor is disabled.
        mouseControlVars.mouseXController.isEnabled = true;
        mouseControlVars.mouseYController.isEnabled = true;
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
