using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogMessages : MonoBehaviour {

    public UserInterface userInterface;
    public AudioCoordinator audioCoordinator;

    // TODO - select and play sounds.
    public AudioClip openDialogSound;
    public AudioClip iterateDialogSound;
    
    public GameObject dialogBox;
    [SerializeField] public Text dialogMessageText;
    [SerializeField] public Text clickToContinueText;

    public string[] dialogMessages;
    private int currentMessageIndex = 0;

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            currentMessageIndex += 1;
            if (currentMessageIndex == dialogMessages.Length) {
                StopAllCoroutines();
                StartCoroutine(ExitDialogBox());
            } else {
                StopAllCoroutines();
                StartCoroutine(GoToDialogBoxNextMessage());
            }
        }
    }

    public void InitializeMessages(string[] messages) {
        dialogMessages = messages;
        currentMessageIndex = 0;
        dialogMessageText.text = "";
        StartCoroutine(InitializeDialogBoxFirstMessage());
    }

    private IEnumerator InitializeDialogBoxFirstMessage() {
        PlaySound(openDialogSound, 0.8f, 0.4f);
        Coroutine flashInDialogBox = StartCoroutine(FlashInDialogBox());
        Coroutine fadeInClickText = StartCoroutine(FadeInText(clickToContinueText, 0.8f));
        yield return flashInDialogBox;
        yield return fadeInClickText;
        yield return StartCoroutine(TypeOutText());
    }

    private IEnumerator GoToDialogBoxNextMessage() {
        PlaySound(iterateDialogSound);
        ResetClickToContinueText();
        yield return StartCoroutine(FadeOutText(dialogMessageText, 0.5f));
        dialogMessageText.text = dialogMessages[currentMessageIndex];
        yield return StartCoroutine(FadeInText(dialogMessageText, 0.5f));
    }

    private IEnumerator ExitDialogBox() {
        PlaySound(iterateDialogSound);
        yield return StartCoroutine(FadeOutText(dialogMessageText, 0.5f));
        Coroutine flashOutDialogBox = StartCoroutine(FlashOutDialogBox());
        Coroutine fadeOutClickText = StartCoroutine(FadeOutText(clickToContinueText, 0.8f));
        yield return flashOutDialogBox;
        yield return fadeOutClickText;
        userInterface.ExitDialogMessages();
    }

    private IEnumerator TypeOutText() {
        dialogMessageText.gameObject.SetActive(true);
        string currentMessage = dialogMessages[currentMessageIndex];
        for (int i = 0; i < currentMessage.Length; i++) {
            dialogMessageText.text += currentMessage[i];
            // Must use WaitForSecondsRealtime since the game is paused.
            yield return new WaitForSecondsRealtime(0.015f);
        }
        dialogMessageText.text = currentMessage;
    }

    private IEnumerator FadeInText(Text text, float duration) {
        float targetAlpha = text.color.a;
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        text.gameObject.SetActive(true);
        while (text.color.a < targetAlpha) {
            // Use fixedDeltaTime since we are paused.
            text.color = new Color(
                text.color.r, text.color.g, text.color.b,
                text.color.a + (Time.fixedDeltaTime / duration));
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, targetAlpha);
    }

    private IEnumerator FadeOutText(Text text, float duration) {
        float initialAlpha = text.color.a;
        while (text.color.a > 0.0f) {
            // Use fixedDeltaTime since we are paused.
            text.color = new Color(
                text.color.r, text.color.g, text.color.b,
                text.color.a - (Time.fixedDeltaTime / duration));
            yield return null;
        }
        text.gameObject.SetActive(false);
        text.color = new Color(text.color.r, text.color.g, text.color.b, initialAlpha);
    }

    private IEnumerator FlashInDialogBox() {
        float dialogEntryTime = 0.8f;
        dialogBox.transform.localScale = new Vector3(1.0f, 0, 1.0f);
        dialogBox.SetActive(true);
        while (dialogBox.transform.localScale.y < 1.0f) {
            // Use fixed delta time here since the game is paused.
            dialogBox.transform.localScale +=
                new Vector3(0, Time.fixedDeltaTime / dialogEntryTime, 0);
            yield return null;
        }
        dialogBox.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    private IEnumerator FlashOutDialogBox() {
        float dialogExitTime = 0.8f;
        dialogBox.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        while (dialogBox.transform.localScale.y > 0) {
            // Use fixed delta time here since the game is paused.
            dialogBox.transform.localScale -=
                new Vector3(0, Time.fixedDeltaTime / dialogExitTime, 0);
            yield return null;
        }
        dialogBox.SetActive(false);
        dialogBox.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    private void ResetClickToContinueText() {
        clickToContinueText.color =
            new Color(
                clickToContinueText.color.r,
                clickToContinueText.color.g,
                clickToContinueText.color.b,
                0.5f);
    }

    private void PlaySound(AudioClip clip, float pitch=1.0f, float volume=1.0f) {
        audioCoordinator.PlaySound2D(clip, pitch, volume);
    }
}
