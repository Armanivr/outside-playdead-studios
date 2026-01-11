using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class QTEIntroManager : MonoBehaviour
{
    [Header("Instellingen Intro")]
    private const string StartControlPath = "<Gamepad>/buttonSouth"; // De knop om de QTE te starten

    public TextMeshProUGUI startPromptText;
    public GameObject quickTimePanel; // Het paneel met de QuickTimeEvent.cs component

    private InputAction startAction;
    private AudioSource audioSource;
    public AudioClip warningSound;

    private bool isWaitingForInput = false;

    // Zodra het IntroPanel actief wordt (door TriggerSpawnEvent)
    private void OnEnable()
    {
        // Start de intro sequentie
        StartCoroutine(RunIntroSequence());
    }

    // Zodra het IntroPanel gedeactiveerd wordt
    private void OnDisable()
    {
        CleanupInputAction();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource component mist op QTEIntroManager.");
        }
    }

    private void SetupInputAction()
    {
        if (startAction != null) CleanupInputAction();

        startAction = new InputAction(binding: StartControlPath);
        startAction.performed += ctx => OnStartButtonPressed();
        startAction.Enable();
    }

    private void CleanupInputAction()
    {
        if (startAction != null)
        {
            startAction.performed -= ctx => OnStartButtonPressed();
            startAction.Disable();
            startAction.Dispose();
            startAction = null;
        }
    }

    private void OnStartButtonPressed()
    {
        if (isWaitingForInput)
        {
            isWaitingForInput = false;
        }
    }

    IEnumerator RunIntroSequence()
    {
        // 1. Visuele/Auditieve waarschuwing
        if (startPromptText != null)
        {
            startPromptText.text = "QTE - Druk op X om te starten";
        }
        if (audioSource != null && warningSound != null)
        {
            audioSource.PlayOneShot(warningSound);
        }

        // 2. Wachten op input
        isWaitingForInput = true;
        SetupInputAction();

        // Wacht totdat de speler de knop drukt
        while (isWaitingForInput)
        {
            yield return null;
        }

        // Korte pauze na knopdruk
        yield return null;
        CleanupInputAction();

        // 3. Start het echte Quick Time Event
        if (quickTimePanel != null)
        {
            quickTimePanel.SetActive(true); // Dit activeert QuickTimeEvent.OnEnable()
        }

        // 4. Schakel het Intro Panel uit
        gameObject.SetActive(false);
    }
}