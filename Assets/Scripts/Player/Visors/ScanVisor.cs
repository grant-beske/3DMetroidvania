using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScanVisor : MonoBehaviour {

    enum State {NORMAL, SCANNING, VIEWSCAN}
    private State activeState = State.NORMAL;

    // Common controller variables
    public UserInterface uiController;
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
        public enum InternalState {OPENING, OPENED, CLOSING};
        [HideInInspector] public InternalState internalState = InternalState.OPENED;

        // UI gameobjects
        public GameObject scanDescriptionDialog;
        [SerializeField] public Text scanCompleteText;
        [SerializeField] public Text scanDescriptionText;
        [HideInInspector] public string scanDescription;

        // SFX
        public AudioClip showSound;
        public AudioClip showDialogSound;
        public AudioClip iterateDialogSound;
        public AudioClip typingSfx;
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // MonoBehavior
    /////////////////////////////////////////////////////////////////////////////////////

    void Start() {
        InitializeNormalState();
        InitializeScanningState();
        InitializeViewScanState();
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
        if (viewScanStateVars.internalState == ViewScanStateVars.InternalState.OPENING) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                TerminateViewScanAnimation();
                SwitchToNormalState();
            }
            if (Input.GetMouseButtonDown(0)) {
                TerminateViewScanAnimation();
            }
        } else if (viewScanStateVars.internalState == ViewScanStateVars.InternalState.OPENED) {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0)) {
                PlaySound(viewScanStateVars.iterateDialogSound);
                StartCoroutine(CloseViewScanInterface());
            }
        } else if (viewScanStateVars.internalState == ViewScanStateVars.InternalState.CLOSING) {
            // Closing is currently a no-op, just wait for UI to close then switch to
            // normal state.
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
        uiController.ResumeSimState();

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
        uiController.PauseSimState(UserInterface.Visor.SCAN);

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
        viewScanStateVars.internalState = ViewScanStateVars.InternalState.OPENING;
        if (scanInitialState != ScanGroup.State.SCANNED) {
            StartCoroutine(OpenViewScanInterface(true));
        } else {
            StartCoroutine(OpenViewScanInterface(false));
        }

        // Re-render all scannable objects to update highlight colors.
        ReRenderScannableObjects();
    }

    // Plays the full animation on a view scan. This only happens when scanning
    // an object for the first time.
    private IEnumerator OpenViewScanInterface(bool isFirstTimeScanning = true) {
        viewScanStateVars.scanDescriptionDialog.transform.localScale =
            new Vector3(1.0f, 0, 1.0f);
        viewScanStateVars.scanDescriptionText.text = "";
        SetTextAlpha(viewScanStateVars.scanCompleteText, 0.0f);

        // Wait for a tiny bit to play sfx, this enhances the experience
        if (isFirstTimeScanning) {
            yield return new WaitForSecondsRealtime(0.1f);
            PlaySound(viewScanStateVars.showDialogSound, 0.8f, 0.4f);
        }

        // Wait for a tiny bit to give the animation more oomph
        yield return new WaitForSecondsRealtime(0.1f);
        Coroutine flashInDialog = StartCoroutine(FlashInScanDialog(1.0f));
        Coroutine fadeInScanComplete =
            StartCoroutine(FadeInText(viewScanStateVars.scanCompleteText, 1.0f, 0.5f));
        yield return flashInDialog;
        yield return fadeInScanComplete;

        // Wait for a tiny bit to give the animation more oomph
        if (isFirstTimeScanning) {
            yield return new WaitForSecondsRealtime(0.1f);
            yield return StartCoroutine(TypeOutScanDescription(0.04f));
        } else {
            SetTextAlpha(viewScanStateVars.scanDescriptionText, 0);
            viewScanStateVars.scanDescriptionText.text =
                viewScanStateVars.scanDescription;
            yield return StartCoroutine(
                FadeInText(viewScanStateVars.scanDescriptionText, 0.5f, 1.0f));
        }
        
        viewScanStateVars.internalState = ViewScanStateVars.InternalState.OPENED;
    }

    private IEnumerator CloseViewScanInterface() {
        viewScanStateVars.internalState = ViewScanStateVars.InternalState.CLOSING;
        yield return StartCoroutine(FadeOutText(viewScanStateVars.scanDescriptionText, 0.25f));

        Coroutine flashOutDialog = StartCoroutine(FlashOutScanDialog(1.0f));
        Coroutine fadeOutScanComplete =
            StartCoroutine(FadeOutText(viewScanStateVars.scanCompleteText, 1.0f));
        yield return flashOutDialog;
        yield return fadeOutScanComplete;

        SetTextAlpha(viewScanStateVars.scanDescriptionText, 1.0f);
        SwitchToNormalState();
    }

    private void TerminateViewScanAnimation() {
        StopAllCoroutines();
        ResetScanningInterface();
        viewScanStateVars.internalState = ViewScanStateVars.InternalState.OPENED;
    }

    private void ResetScanningInterface() {
        viewScanStateVars.scanDescriptionDialog.transform.localScale = Vector3.one;
        SetTextAlpha(viewScanStateVars.scanDescriptionText, 1.0f);
        SetTextAlpha(viewScanStateVars.scanCompleteText, 0.5f);
        viewScanStateVars.scanDescriptionText.text = viewScanStateVars.scanDescription;
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

    private IEnumerator FlashInScanDialog(float duration) {
        viewScanStateVars.scanDescriptionDialog.transform.localScale =
            new Vector3(1.0f, 0, 1.0f);
        while (viewScanStateVars.scanDescriptionDialog.transform.localScale.y < 1.0f) {
            // Use fixed delta time here since the game is paused.
            viewScanStateVars.scanDescriptionDialog.transform.localScale +=
                new Vector3(0, Time.fixedDeltaTime / duration, 0);
            yield return null;
        }
        viewScanStateVars.scanDescriptionDialog.transform.localScale = Vector3.one;
    }

    private IEnumerator FlashOutScanDialog(float duration) {
        viewScanStateVars.scanDescriptionDialog.transform.localScale = Vector3.one;
        while (viewScanStateVars.scanDescriptionDialog.transform.localScale.y > 0.0f) {
            // Use fixed delta time here since the game is paused.
            viewScanStateVars.scanDescriptionDialog.transform.localScale -=
                new Vector3(0, Time.fixedDeltaTime / duration, 0);
            yield return null;
        }
        viewScanStateVars.scanDescriptionDialog.transform.localScale =
            new Vector3(1.0f, 0, 1.0f);
    }

    private IEnumerator FadeInText(Text text, float duration, float alpha) {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        while (text.color.a < alpha) {
            // Use fixedDeltaTime since we are paused.
            text.color = new Color(
                text.color.r, text.color.g, text.color.b,
                text.color.a + ((Time.fixedDeltaTime * (alpha / 1.0f)) / duration));
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
    }

    private IEnumerator FadeOutText(Text text, float duration) {
        float initialAlpha = text.color.a;
        while (text.color.a > 0) {
            // Use fixedDeltaTime since we are paused.
            text.color = new Color(
                text.color.r, text.color.g, text.color.b,
                text.color.a - ((Time.fixedDeltaTime * (initialAlpha / 1.0f)) / duration));
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    }

    private IEnumerator TypeOutScanDescription(float interval) {
        viewScanStateVars.scanDescriptionText.text = "";
        for (int i = 0; i < viewScanStateVars.scanDescription.Length; i++) {
            viewScanStateVars.scanDescriptionText.text +=
                viewScanStateVars.scanDescription[i];
            PlaySound(viewScanStateVars.typingSfx);
            // Must use WaitForSecondsRealtime since the game is paused.
            yield return new WaitForSecondsRealtime(interval);
        }
        viewScanStateVars.scanDescriptionText.text = viewScanStateVars.scanDescription;
    }

    private void SetTextAlpha(Text text, float alpha) {
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
    }

    private void PlaySound(AudioClip clip, float pitch=1.0f, float volume=1.0f) {
        audioCoordinator.PlaySound2D(clip, pitch, volume);
    }

    private void ReRenderScannableObjects() {
        GameObject.Find("UIController").GetComponent<UserInterface>().ReInitRenderMode();
    }
}
