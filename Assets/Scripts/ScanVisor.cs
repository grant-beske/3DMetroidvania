using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScanVisor : MonoBehaviour {

    enum State {NORMAL, VIEWSCAN}
    private State activeState = State.NORMAL;

    // UI controller
    public GameObject userInterface;
    private UserInterface uiController;

    // SFX for scanning
    public AudioClip scanHighlightSound;
    public AudioClip scanViewScanSound;

    // Scan state gameobjects
    public GameObject normalStateObj;
    public GameObject viewScanStateObj;

    // Scan crosshair gameobjects
    public GameObject scanTargetBlank;
    public GameObject scanTargetHighlighted;
    public GameObject scanTargetSelected;
    private bool targetIsHighlighted = false;

    // View scan gameobjects
    [SerializeField] Text scanDescription;

    void Start() {
        InitializeNormalState();
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
                        1 << 6, // Only look at layer 6: Scannable
                        QueryTriggerInteraction.Collide)) {
                    SwitchToViewScanState(hit);
                }
            } else {
                RaycastHit hit;
                if (Physics.Raycast(
                        Camera.main.transform.position,
                        Camera.main.transform.forward,
                        out hit,
                        Mathf.Infinity,
                        1 << 6, // Only look at layer 6: Scannable
                        QueryTriggerInteraction.Collide)) {
                    ToggleScanHighlight(true);
                } else {
                    ToggleScanHighlight(false);
                }
            }
        } else if (activeState == State.VIEWSCAN) {
            if (Input.GetMouseButtonDown(0)) {
                SwitchToNormalState();
            }
        }
    }

    private void InitializeNormalState() {
        normalStateObj.SetActive(true);
        scanTargetBlank.SetActive(true);
        scanTargetHighlighted.SetActive(false);
        scanTargetSelected.SetActive(false);
    }

    private void InitializeViewScanState() {
        viewScanStateObj.SetActive(false);
    }

    private void SwitchToNormalState() {
        viewScanStateObj.SetActive(false);
        normalStateObj.SetActive(true);
        activeState = State.NORMAL;
        uiController.DisableCursor();
        uiController.EnableGameControls();

        // Time flows regularly in normal scan mode.
        Time.timeScale = 1f;
    }

    private void SwitchToViewScanState(RaycastHit hit) {
        normalStateObj.SetActive(false);
        viewScanStateObj.SetActive(true);
        activeState = State.VIEWSCAN;
        uiController.EnableCursor();
        uiController.DisableGameControls();

        // Set scan interface based on item scanned
        Scannable scan = hit.collider.gameObject.GetComponent<Scannable>();
        scanDescription.text = scan.scanDescription;

        // Play view scan sound effect
        PlaySound(scanViewScanSound);

        // Pause time while viewing scan description.
        Time.timeScale = 0f;
    }

    private void ToggleScanHighlight(bool shouldHighlight) {
        if (targetIsHighlighted) {
            if (!shouldHighlight) {
                scanTargetHighlighted.SetActive(false);
                scanTargetBlank.SetActive(true);
                targetIsHighlighted = false;
            }
        } else {
            if (shouldHighlight) {
                scanTargetBlank.SetActive(false);
                scanTargetHighlighted.SetActive(true);
                targetIsHighlighted = true;
                PlaySound(scanHighlightSound);
            }
        }
    }

    private void PlaySound(AudioClip clip) {
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }
}
