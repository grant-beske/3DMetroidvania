using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour {

    enum Visor {SELECT, COMBAT, SCAN};
    private Visor activeVisor = Visor.COMBAT;

    public GameObject visorSelect;
    public GameObject combatVisor;
    public GameObject scanVisor;
    private GameObject _activeVisorObj;

    public Texture2D mouseCursor;

    void Start() {
        visorSelect.SetActive(false);
        scanVisor.SetActive(false);
        combatVisor.SetActive(true);
        _activeVisorObj = combatVisor;
        Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.Auto);
    }

    void Update() {
        if (activeVisor != Visor.SELECT && Input.GetMouseButtonDown(2)) {
            SetVisorSelect();
        } else if (activeVisor == Visor.SELECT && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(2))) {
            SetCombatVisor();
        }
    }

    public void SetVisorSelect() {
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
        DisableCursor();
        activeVisor = Visor.COMBAT;
        _activeVisorObj.SetActive(false);
        _activeVisorObj = scanVisor;
        scanVisor.SetActive(true);
    }

    private void EnableCursor() {
        // TODO - reset cursor position to center of screen.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void DisableCursor() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
