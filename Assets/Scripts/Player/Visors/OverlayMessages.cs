using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayMessages : MonoBehaviour {
    public GameObject savedMessage;

    void Start() {
        savedMessage.SetActive(false);
    }

    public void TriggerSavedMessage() {
        StartCoroutine(PlaySavedMessage());
    }

    private IEnumerator PlaySavedMessage() {
        savedMessage.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        savedMessage.SetActive(false);
    }
}
