using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogMessages : MonoBehaviour {

    public UserInterface userInterface;
    public AudioCoordinator audioCoordinator;

    // TODO - animate and play sounds.
    public AudioClip openDialogSound;
    public AudioClip closeDialogSound;
    public AudioClip iterateDialogSound;
    public AudioClip textTypeSound;
    
    public GameObject dialogBox;
    [SerializeField] public Text dialogMessageText;
    [SerializeField] public Text clickToContinueText;

    public string[] dialogMessages;
    private int currentMessageIndex = 0;

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            currentMessageIndex += 1;
            if (currentMessageIndex == dialogMessages.Length) {
                userInterface.ExitDialogMessages();
            } else {
                dialogMessageText.text = dialogMessages[currentMessageIndex];
            }
        }
    }

    public void InitializeMessages(string[] messages) {
        dialogMessages = messages;
        currentMessageIndex = 0;
        dialogMessageText.text = messages[currentMessageIndex];
    }

    private void PlaySound(AudioClip clip) {
        audioCoordinator.PlaySound2D(clip);
    }
}
