using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour {

    // Visor state variables.
    public enum Visor {LOADING, PAUSED, DIALOG, SELECT, COMBAT, SCAN};
    private Visor activeVisor = Visor.LOADING;
    // Previously active visor, not counting technical visor states like SELECT.
    private Visor previouslyActiveVisor = Visor.COMBAT;

    // Enable / disable game control. Useful for displaying menus, etc.
    public bool enableGameControls = true;

    // Standard controller variables.
    public PlayerState playerState;
    public WeaponsCoordinator gunControl;
    public AudioCoordinator audioCoordinator;
    public MusicController musicController;

    // State classes for UserInterface.
    public VisorControlVars visorControlVars;
    public MouseControlVars mouseControlVars;
    public VisorSelectControlVars visorSelectControlVars;

    [System.Serializable]
    public class VisorControlVars {
        // Options.
        public bool debugModeEnabled = false;
        // Visor gameobjects.
        public GameObject loadingState;
        public GameObject pauseMenu;
        public GameObject overlayMessages;
        public GameObject dialogMessages;
        public GameObject debugMode;
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

    [System.Serializable]
    public class VisorSelectControlVars {
        public GameObject combatVisorButton;
        public GameObject scanVisorButton;
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // MonoBehavior Functions
    /////////////////////////////////////////////////////////////////////////////////////

    void Start() {
        // Initialize visor behavior. Default to LOADING. The LevelLoader script will
        // handle changing state from LOADING to COMBAT.
        visorControlVars.loadingState.SetActive(true);
        visorControlVars.pauseMenu.SetActive(false);
        visorControlVars.overlayMessages.SetActive(false);
        visorControlVars.dialogMessages.SetActive(false);
        visorControlVars.visorSelect.SetActive(false);
        visorControlVars.scanVisor.SetActive(false);
        visorControlVars.combatVisor.SetActive(false);
        visorControlVars.activeVisorObj = visorControlVars.loadingState;
        visorControlVars.debugMode.SetActive(visorControlVars.debugModeEnabled);

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
        } else if (activeVisor == Visor.PAUSED) {
            // Currently a no-op, behavior is handled in PauseMenu.
        } else if (activeVisor == Visor.DIALOG) {
            // Currently a no-op, behavior is handled in DialogMessages.
        } else if (activeVisor == Visor.SELECT) {
            // Currently a no-op, behavior is handled in VisorSelect.
        } else if (activeVisor == Visor.COMBAT) {
            // Currently a no-op, behavior is handled in CombatVisor.
        } else if (activeVisor == Visor.SCAN) {
            UpdateScanVisor();
        }
    }

    private void UpdateScanVisor() {
        if (!enableGameControls) return;
        if (Input.GetMouseButtonDown(2)) {
            SetVisorSelect();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            PauseGame();
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Game logic functions
    /////////////////////////////////////////////////////////////////////////////////////

    public void FinishLoading() {
        SetCombatVisor();
        gunControl.ActivateWeapon();
        Time.timeScale = 1f;

        // Trigger the "entered area" jingle on game load.
        visorControlVars.overlayMessages.SetActive(true);
        visorControlVars.overlayMessages
            .GetComponent<OverlayMessages>()
            .TriggerInitialEnterLocationMessage();

        StartInitialMusic();
        StartCoroutine(DisableCharacterForTeleport());
    }

    private void StartInitialMusic() {
        // Start out with 10s of silence to let enter level jingle play.
        musicController.EnqueueSilence(7f);
    }

    // Disable character motor for 0.1 sec when loading is finished. This is a hacky way
    // to correctly position the character at the PlayerDockingPoint when loading a
    // save file.
    private IEnumerator DisableCharacterForTeleport() {
        mouseControlVars.mouseXObj.GetComponent<CharacterMotor>().enabled = false;
        yield return new WaitForSeconds(0.1f);
        mouseControlVars.mouseXObj.GetComponent<CharacterMotor>().enabled = true;
    }

    public void PauseGame() {
        visorControlVars.pauseMenu.SetActive(true);
        PauseSimState(Visor.PAUSED);
    }

    public void UnpauseGame() {
        visorControlVars.pauseMenu.SetActive(false);
        ResumeSimState();
    }

    public void EnterDialogMessages(string[] messages) {
        visorControlVars.dialogMessages.SetActive(true);
        PauseSimState(Visor.DIALOG);
        visorControlVars.dialogMessages
            .GetComponent<DialogMessages>().InitializeMessages(messages);
    }

    public void ExitDialogMessages() {
        visorControlVars.dialogMessages.SetActive(false);
        ResumeSimState();
    }

    public void PauseSimState(Visor visor) {
        EnableCursor();
        DisableGameControls();
        if (activeVisor == Visor.COMBAT)
            gunControl.DeactivateWeaponControl();
        activeVisor = visor;
        Time.timeScale = 0f;
        musicController.PauseSong();
        audioCoordinator.PauseSounds();
    }

    public void ResumeSimState() {
        DisableCursor();
        EnableGameControls();
        if (previouslyActiveVisor == Visor.COMBAT)
            gunControl.ActivateWeaponControl();
        activeVisor = previouslyActiveVisor;
        Time.timeScale = 1f;
        musicController.ResumeSong();
        audioCoordinator.ResumeSounds();
    }

    public void SetVisorSelect() {
        PlaySound(visorControlVars.visorSelectSound);
        EnableCursor();
        activeVisor = Visor.SELECT;
        visorControlVars.activeVisorObj.SetActive(false);
        visorControlVars.activeVisorObj = visorControlVars.visorSelect;

        // Show / hide visor buttons based on what the player has gotten.
        visorSelectControlVars.combatVisorButton.SetActive(
            playerState.coreStateValues.hasCombatVisor);
        visorSelectControlVars.scanVisorButton.SetActive(
            playerState.coreStateValues.hasScanVisor);
        visorControlVars.visorSelect.SetActive(true);

        // Disable the control of any open weapons.
        gunControl.DeactivateWeaponControl();
    }

    public void SetCombatVisor() {
        DisableCursor();
        activeVisor = Visor.COMBAT;
        previouslyActiveVisor = Visor.COMBAT;
        visorControlVars.activeVisorObj.SetActive(false);
        visorControlVars.activeVisorObj = visorControlVars.combatVisor;
        visorControlVars.combatVisor.SetActive(true);

        // Enable weapons.
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

    public void PlayPowerupDialogMessage(string[] messages, float delay) {
        StartCoroutine(PlayPowerupDialogMessageWithDelay(messages, delay));
    }

    public IEnumerator PlayPowerupDialogMessageWithDelay(string[] messages, float delay) {
        yield return new WaitForSeconds(delay);
        EnterDialogMessages(messages);
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
        // All UI sounds are played in 2D.
        audioCoordinator.PlaySound2D(clip);
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
