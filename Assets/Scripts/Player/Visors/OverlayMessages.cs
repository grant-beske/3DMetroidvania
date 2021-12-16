using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayMessages : MonoBehaviour {

    private AudioSource audioSource;

    // Player state variables to get data to display.
    public PlayerState playerState;

    public CoreUIZones coreUIZones;
    public MetricElements metricElements;
    public MessageElements messageElements;

    [System.Serializable]
    public class CoreUIZones {
        public GameObject metricsObj;
        public GameObject messageObj;
    }

    [System.Serializable]
    public class MetricElements {
        [SerializeField] public Text healthText;
        [SerializeField] public Text healthMetric;
        [SerializeField] public Text energyText;
        [SerializeField] public Text energyMetric;
        [SerializeField] public Text currentLocation;
    }

    [System.Serializable]
    public class MessageElements {
        public GameObject savedMessage;
        [SerializeField] public Text enterLocationMessage;
        public AudioClip enterLocationJingle;
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // MonoBehavior
    /////////////////////////////////////////////////////////////////////////////////////

    void Start() {
        audioSource = GetComponent<AudioSource>();
        messageElements.savedMessage.SetActive(false);
        messageElements.enterLocationMessage.gameObject.SetActive(false);
    }

    void Update() {
        UpdateHealth();
        UpdateEnergy();
        UpdateCurrentLocation();
    }

    private void UpdateHealth() {
        metricElements.healthMetric.text =
            ((int) playerState.coreStateValues.health).ToString();
    }

    private void UpdateEnergy() {
        metricElements.energyMetric.text =
            ((int) playerState.coreStateValues.energy).ToString();
    }

    private void UpdateCurrentLocation() {
        metricElements.currentLocation.text = playerState.coreLocationValues.currentArea;
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Public API
    /////////////////////////////////////////////////////////////////////////////////////

    public void TriggerSavedMessage() {
        StartCoroutine(PlaySavedMessage());
    }

    private IEnumerator PlaySavedMessage() {
        messageElements.savedMessage.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        messageElements.savedMessage.SetActive(false);
    }

    public void TriggerInitialEnterLocationMessage() {
        StartCoroutine(PlayInitialEnterLocationMessage());
    }

    private IEnumerator PlayInitialEnterLocationMessage() {
        // Initial: play jingle and hide metrics view
        PlaySound(messageElements.enterLocationJingle);
        coreUIZones.metricsObj.SetActive(false);
        
        // Phase I: show initial message
        messageElements.enterLocationMessage.text =
            playerState.coreLocationValues.currentArea;
        yield return StartCoroutine(
            TypeOutText(messageElements.enterLocationMessage, 0.075f));
        yield return new WaitForSeconds(2.5f);

        // Phase II: fade out initial message
        yield return StartCoroutine(
            FadeOutText(messageElements.enterLocationMessage, 0.6f));

        // Phase III: show overlay UI
        coreUIZones.metricsObj.SetActive(true);
        yield return StartCoroutine(FadeInBottomLeftIndicators(0.6f));
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // Private helpers
    /////////////////////////////////////////////////////////////////////////////////////

    private IEnumerator FadeInBottomLeftIndicators(float duration) {
        Coroutine fadeInHealthText =
            StartCoroutine(FadeInText(metricElements.healthText, duration));
        Coroutine fadeInHealthNum =
            StartCoroutine(FadeInText(metricElements.healthMetric, duration));
        Coroutine fadeInEnergyText =
            StartCoroutine(FadeInText(metricElements.energyText, duration));
        Coroutine fadeInEnergyNum =
            StartCoroutine(FadeInText(metricElements.energyMetric, duration));
        Coroutine fadeInCurrentLocation =
            StartCoroutine(FadeInText(metricElements.currentLocation, duration));
        yield return fadeInHealthText;
        yield return fadeInHealthNum;
        yield return fadeInEnergyText;
        yield return fadeInEnergyNum;
        yield return fadeInCurrentLocation;
    }

    private IEnumerator TypeOutText(Text text, float typeInterval) {
        string targetText = text.text;
        text.text = "";
        text.gameObject.SetActive(true);
        for (int i = 0; i < targetText.Length; i++) {
            text.text += targetText[i];
            yield return new WaitForSeconds(typeInterval);
        }
    }

    private IEnumerator FadeOutText(Text text, float duration) {
        float initialAlpha = text.color.a;
        while (text.color.a > 0.0f) {
            text.color = new Color(
                text.color.r, text.color.g, text.color.b,
                text.color.a - (Time.deltaTime / duration));
            yield return null;
        }
        text.gameObject.SetActive(false);
        text.color = new Color(text.color.r, text.color.g, text.color.b, initialAlpha);
    }

    private IEnumerator FadeInText(Text text, float duration) {
        float targetAlpha = text.color.a;
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        text.gameObject.SetActive(true);
        while (text.color.a < targetAlpha) {
            text.color = new Color(
                text.color.r, text.color.g, text.color.b,
                text.color.a + (Time.deltaTime / duration));
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, targetAlpha);
    }

    private void PlaySound(AudioClip sound) {
        audioSource.clip = sound;
        audioSource.Play();
    }
}
