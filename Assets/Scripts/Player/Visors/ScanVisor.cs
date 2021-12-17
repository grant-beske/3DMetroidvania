using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScanVisor : MonoBehaviour {

    enum State {NORMAL, SCANNING, VIEWSCAN}
    private State activeState = State.NORMAL;

    // UI controller
    public GameObject userInterface;
    private UserInterface uiController;

    // AudioCoordinator to use in playing scan state UI effects.
    public AudioCoordinator audioCoordinator;

    // Scanning state object containers
    public NormalStateVars normalStateVars;
    public ScanningStateVars scanningStateVars;
    public ViewScanStateVars viewScanStateVars;

    [System.Serializable]
    public class NormalStateVars {
        // Main state gameobject
        public GameObject stateObj;

        // Scan crosshair gameobjects
        public GameObject scanTargetBlank;
        public GameObject scanTargetHighlighted;
        [HideInInspector] public bool targetIsHighlighted = false;

        // SFX
        public AudioClip highlightSound;
    }

    [System.Serializable]
    public class ScanningStateVars {
        // Main state gameobject
        public GameObject stateObj;

        // UI gameobjects
        [SerializeField] Text scanningText;
        public GameObject progressBar;
        public GameObject scanningSound;
        // For now, the scanning has its own special audio source. This is to ensure that
        // we can resume the scanning sound at the same place we left off in the case of
        // a repeat scan.
        [HideInInspector] public AudioSource scanningAudioSource;

        // Scanning state objects
        public float timeRequiredToScan = 1.0f;
        [HideInInspector] public GameObject scanGroup;
        [HideInInspector] public float timeElapsed = 0.0f;
    }

    [System.Serializable]
    public class ViewScanStateVars {
        // Main state gameobject
        public GameObject stateObj;

        // Internal state - could be typing out scan, or done typing.
        public enum InternalState {ANIMATING, DONE};
        [HideInInspector] public InternalState internalState = InternalState.DONE;
        [HideInInspector] public IEnumerator animatorCoroutine;

        // UI gameobjects
        public GameObject scanDescriptionDialog;
        [SerializeField] public Text scanDescriptionText;
        [HideInInspector] public string scanDescription;

        // SFX
        public AudioClip showSound;
        public AudioClip showDialogSound;
        public AudioClip typingSfx;
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // MonoBehavior
    /////////////////////////////////////////////////////////////////////////////////////

    void Start() {
        InitializeNormalState();
        InitializeScanningState();
        InitializeViewScanState();
        uiController = userInterface.GetComponent<UserInterface>();
    }

    private void InitializeNormalState() {
        normalStateVars.stateObj.SetActive(true);
        normalStateVars.scanTargetBlank.SetActive(true);
        normalStateVars.scanTargetHighlighted.SetActive(false);
    }

    private void InitializeScanningState() {
        scanningStateVars.stateObj.SetActive(false);
        scanningStateVars.scanningAudioSource =
            scanningStateVars.scanningSound.GetComponent<AudioSource>();
    }

    private void InitializeViewScanState() {
        viewScanStateVars.stateObj.SetActive(false);
    }

    void Update() {
        if (activeState == State.NORMAL) {
            UpdateNormalState();
        } else if (activeState == State.SCANNING) {
            UpdateScanningState();
        } else if (activeState == State.VIEWSCAN) {
            UpdateViewScanState();
        }
    }

    private void UpdateNormalState() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            if (Physics.Raycast(
                    Camera.main.transform.position,
                    Camera.main.transform.forward,
                    out hit,
                    Mathf.Infinity,
                    // Layer 7: Terrain, Layer 10-12: Scannable
                    (1 << 7) | (1 << 10) | (1 << 11) | (1 << 12),
                    QueryTriggerInteraction.Collide)) {
                if (hit.collider.gameObject.layer >= 10 && hit.collider.gameObject.layer <= 12) {
                    SwitchToScanningState(hit);
                }
            }
        } else {
            RaycastHit hit;
            if (Physics.Raycast(
                    Camera.main.transform.position,
                    Camera.main.transform.forward,
                    out hit,
                    Mathf.Infinity,
                    // Layer 7: Terrain, Layer 10-12: Scannable
                    (1 << 7) | (1 << 10) | (1 << 11) | (1 << 12),
                    QueryTriggerInteraction.Collide)) {
                if (hit.collider.gameObject.layer >= 10 && hit.collider.gameObject.layer <= 12) {
                    ToggleScanHighlight(true);
                } else {
                    ToggleScanHighlight(false);
                }
            } else {
                ToggleScanHighlight(false);
            }
        }
    }

    private void UpdateScanningState() {
        if (Input.GetMouseButton(0)) {
            RaycastHit hit;
            bool raycastHasHit =
                Physics.Raycast(
                    Camera.main.transform.position,
                    Camera.main.transform.forward,
                    out hit,
                    Mathf.Infinity,
                    // Layer 7: Terrain, Layer 10-12: Scannable
                    (1 << 7) | (1 << 10) | (1 << 11) | (1 << 12),
                    QueryTriggerInteraction.Collide);
            if (raycastHasHit) {
                // Check for layer 10 (Normal), 11 (Critical), or 12 (Scanned)
                if (hit.collider.gameObject.layer >= 10 && hit.collider.gameObject.layer <= 12) {
                    GameObject hitScanGroup =
                        hit.collider.gameObject.GetComponent<Scannable>().parentScanGroupObj;
                    if (hitScanGroup == scanningStateVars.scanGroup) {
                        scanningStateVars.timeElapsed += Time.deltaTime;
                        UpdateProgressBar();
                        // TODO - scanning sound
                        if (scanningStateVars.timeElapsed >= scanningStateVars.timeRequiredToScan) {
                            SwitchToViewScanState(hit);
                        }
                    } else {
                        SwitchToNormalState();
                    }
                } else {
                    SwitchToNormalState();
                }
            } else {
                SwitchToNormalState();
            }
        } else {
            SwitchToNormalState();
        }
    }

    private void UpdateViewScanState() {
        if (viewScanStateVars.internalState == ViewScanStateVars.InternalState.ANIMATING) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                TerminateViewScanAnimation();
                SwitchToNormalState();
            }
            if (Input.GetMouseButtonDown(0)) {
                TerminateViewScanAnimation();
            }
        } else if (viewScanStateVars.internalState == ViewScanStateVars.InternalState.DONE) {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0)) {
                SwitchToNormalState();
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Private helpers
    /////////////////////////////////////////////////////////////////////////////////////

    private void SwitchToNormalState() {
        viewScanStateVars.stateObj.SetActive(false);
        scanningStateVars.stateObj.SetActive(false);
        normalStateVars.stateObj.SetActive(true);
        activeState = State.NORMAL;
        uiController.DisableCursor();
        uiController.EnableGameControls();

        // Pause the scanning sound for now, may resume later.
        scanningStateVars.scanningAudioSource.Pause();

        // Time flows regularly in normal scan mode.
        Time.timeScale = 1f;
    }

    private void SwitchToScanningState(RaycastHit hit) {
        viewScanStateVars.stateObj.SetActive(false);
        scanningStateVars.stateObj.SetActive(true);
        normalStateVars.stateObj.SetActive(false);
        activeState = State.SCANNING;
        uiController.DisableCursor();
        uiController.EnableGameControls();

        // Time flows regularly in scanning mode.
        Time.timeScale = 1f;

        // Store information about what is being scanned. Allow for resumption of prev scan.
        GameObject hitScanGroup =
            hit.collider.gameObject.GetComponent<Scannable>().parentScanGroupObj;
        if (scanningStateVars.scanGroup != hitScanGroup) {
            scanningStateVars.scanGroup = hitScanGroup;
            scanningStateVars.timeElapsed = 0.0f;
            ResetProgressBar();
            // Reset the scanning sound on new scan target.
            scanningStateVars.scanningAudioSource.Stop();
        }

        // If the hit scan group was already scanned, just directly go to VIEWSCAN state.
        if (hitScanGroup.GetComponent<ScanGroup>().GetActiveState() == ScanGroup.State.SCANNED) {
            SwitchToViewScanState(hit);
        } else {
            // Only play the audio source for incomplete scans.
            scanningStateVars.scanningAudioSource.Play();
        }
    }

    private void SwitchToViewScanState(RaycastHit hit) {
        normalStateVars.stateObj.SetActive(false);
        scanningStateVars.stateObj.SetActive(false);
        viewScanStateVars.stateObj.SetActive(true);
        activeState = State.VIEWSCAN;
        uiController.EnableCursor();
        uiController.DisableGameControls();

        // Reset the scanning sound.
        scanningStateVars.scanningAudioSource.Stop();

        // Play view scan sound effect
        PlaySound(viewScanStateVars.showSound);

        // Pause time while viewing scan description.
        Time.timeScale = 0f;

        // Set scan interface based on item scanned
        Scannable scan = hit.collider.gameObject.GetComponent<Scannable>();
        ScanGroup.State scanInitialState = scan.GetState();
        viewScanStateVars.scanDescription = scan.GetScan();
        viewScanStateVars.scanDescriptionText.text = viewScanStateVars.scanDescription;

        // If the item scanned was not previously scanned, animate the display.
        if (scanInitialState != ScanGroup.State.SCANNED) {
            viewScanStateVars.internalState = ViewScanStateVars.InternalState.ANIMATING;
            viewScanStateVars.animatorCoroutine = AnimateViewScanDisplay();
            StartCoroutine(viewScanStateVars.animatorCoroutine);
        } else {
            viewScanStateVars.internalState = ViewScanStateVars.InternalState.DONE;
        }

        // Re-render all scannable objects to update highlight colors.
        ReRenderScannableObjects();
    }

    // Plays the full animation on a view scan. This only happens when scanning
    // an object for the first time.
    private IEnumerator AnimateViewScanDisplay() {
        viewScanStateVars.scanDescriptionText.text = "";

        // Flash in the scan description container element
        float dialogEntryTime = 1.0f;
        viewScanStateVars.scanDescriptionDialog.transform.localScale =
            new Vector3(1.0f, 0, 1.0f);
        PlaySound(viewScanStateVars.showDialogSound, 0.8f, 0.4f);
        // Wait for a tiny bit to give the animation more oomph
        yield return new WaitForSecondsRealtime(0.1f);
        while (viewScanStateVars.scanDescriptionDialog.transform.localScale.y < 1.0f) {
            // Use fixed delta time here since the game is paused.
            viewScanStateVars.scanDescriptionDialog.transform.localScale +=
                new Vector3(0, Time.fixedDeltaTime / dialogEntryTime, 0);
            yield return null;
        }
        viewScanStateVars.scanDescriptionDialog.transform.localScale =
            new Vector3(1.0f, 1.0f, 1.0f);

        // Wait for a tiny bit to give the animation more oomph
        yield return new WaitForSecondsRealtime(0.1f);

        // Type out the scan description
        for (int i = 0; i < viewScanStateVars.scanDescription.Length; i++) {
            viewScanStateVars.scanDescriptionText.text +=
                viewScanStateVars.scanDescription[i];
            PlaySound(viewScanStateVars.typingSfx);
            // Must use WaitForSecondsRealtime since the game is paused.
            yield return new WaitForSecondsRealtime(0.04f);
        }
        viewScanStateVars.scanDescriptionText.text = viewScanStateVars.scanDescription;
        viewScanStateVars.internalState = ViewScanStateVars.InternalState.DONE;
    }

    private void TerminateViewScanAnimation() {
        StopCoroutine(viewScanStateVars.animatorCoroutine);
        viewScanStateVars.scanDescriptionText.text = viewScanStateVars.scanDescription;
        viewScanStateVars.internalState = ViewScanStateVars.InternalState.DONE;
    }

    private void ToggleScanHighlight(bool shouldHighlight) {
        if (normalStateVars.targetIsHighlighted) {
            if (!shouldHighlight) {
                normalStateVars.scanTargetHighlighted.SetActive(false);
                normalStateVars.scanTargetBlank.SetActive(true);
                normalStateVars.targetIsHighlighted = false;
            }
        } else {
            if (shouldHighlight) {
                normalStateVars.scanTargetBlank.SetActive(false);
                normalStateVars.scanTargetHighlighted.SetActive(true);
                normalStateVars.targetIsHighlighted = true;
                PlaySound(normalStateVars.highlightSound);
            }
        }
    }

    private void ResetProgressBar() {
        scanningStateVars.progressBar.transform.localScale = new Vector3(0, 1, 1);
    }

    private void UpdateProgressBar() {
        float percentComplete =
            scanningStateVars.timeElapsed / scanningStateVars.timeRequiredToScan;
        scanningStateVars.progressBar.transform.localScale = new Vector3(percentComplete, 1, 1);
    }

    private void PlaySound(AudioClip clip, float pitch=1.0f, float volume=1.0f) {
        audioCoordinator.PlaySound2D(clip, pitch, volume);
    }

    private void ReRenderScannableObjects() {
        GameObject.Find("UIController").GetComponent<UserInterface>().ReInitRenderMode();
    }
}
