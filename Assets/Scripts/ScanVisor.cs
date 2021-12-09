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

    // SFX for scanning
    public AudioClip scanHighlightSound;
    public AudioClip scanViewScanSound;

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
    }

    [System.Serializable]
    public class ScanningStateVars {
        // Main state gameobject
        public GameObject stateObj;

        // UI gameobjects
        [SerializeField] Text scanningText;
        public GameObject progressBar;
        public GameObject scanningSound;
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

        // UI gameobjects
        [SerializeField] public Text scanDescription;
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Class Functions
    /////////////////////////////////////////////////////////////////////////////////////

    void Start() {
        InitializeNormalState();
        InitializeScanningState();
        InitializeViewScanState();
        uiController = userInterface.GetComponent<UserInterface>();
    }

    void Update() {
        if (activeState == State.NORMAL) {
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
        } else if (activeState == State.SCANNING) {
            if (Input.GetMouseButton(0)) {
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
        } else if (activeState == State.VIEWSCAN) {
            if (Input.GetMouseButtonDown(0)) {
                SwitchToNormalState();
            }
        }
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

        // Set scan interface based on item scanned
        Scannable scan = hit.collider.gameObject.GetComponent<Scannable>();
        viewScanStateVars.scanDescription.text = scan.GetScan();

        // Play view scan sound effect
        PlaySound(scanViewScanSound);

        // Pause time while viewing scan description.
        Time.timeScale = 0f;

        // Re-render all scannable objects to update highlight colors.
        ReRenderScannableObjects();
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
                PlaySound(scanHighlightSound);
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

    private void PlaySound(AudioClip clip) {
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }

    private void ReRenderScannableObjects() {
        GameObject.Find("UIController").GetComponent<UserInterface>().ReInitRenderMode();
    }
}
