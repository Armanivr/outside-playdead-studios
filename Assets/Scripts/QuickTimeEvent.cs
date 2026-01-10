using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

// Dit is een 'struct' om in de Inspector een Control Path aan een Sprite te koppelen
[System.Serializable]
public struct ControllerButtonMapping
{
    public string controlPath;
    public Sprite sprite;
}

public class QuickTimeEvent : MonoBehaviour
{
    [Header("Instellingen QTE")]
    public float timeLimit = 1.5f;
    public int requiredSuccesses = 3;

    private int currentSuccesses = 0;

    [Header("UI Elementen")]
    public Image timerFillImage;

    [Header("Player Reference")]
    public playerMovementScript playerMovement;

    [Header("Controller Iconen")]
    public Image buttonIconImage;
    public ControllerButtonMapping[] buttonMappings;

    [HideInInspector] public string requiredPath;

    private bool isActive = false;
    private InputAction inputAction;

    public GameObject spawnedEnemy;

    // --- SETUP EN DEACTIVERING ---

    private void OnEnable()
    {
        isActive = true;
        currentSuccesses = 0;
        StartNewQTE();
    }

    private void OnDisable()
    {
        StopListeningForInput();
    }

    // --- PUBLIC METHODE OM ENEMY TOE TE WIJZEN ---

    /// <summary>
    /// Wijs de gespawnde enemy toe aan dit QTE event
    /// </summary>
    public void SetSpawnedEnemy(GameObject enemy)
    {
        spawnedEnemy = enemy;
        Debug.Log($"Enemy toegewezen aan QTE: {enemy.name}");
    }

    void StartNewQTE()
    {
        // Stop ALLEEN de lopende timer coroutine
        StopAllCoroutines();

        // Zorg dat de vorige actie gestopt is voordat we een nieuwe starten
        StopListeningForInput();

        // 1. Willekeurige knop (Control Path) kiezen
        if (buttonMappings.Length > 0)
        {
            int randomIndex = Random.Range(0, buttonMappings.Length);
            requiredPath = buttonMappings[randomIndex].controlPath;
        }

        // 2. Visuele timer instellen (vol)
        if (timerFillImage != null)
        {
            timerFillImage.type = Image.Type.Filled;
            timerFillImage.fillMethod = Image.FillMethod.Radial360;
            timerFillImage.fillAmount = 1f;
        }

        // 3. Toon de juiste knopafbeelding
        SetButtonIcon(requiredPath);

        // 4. Start luisteren naar input voor de nieuwe knop
        StartListeningForInput();

        // 5. Start de timer
        StartCoroutine(StartTimer());
    }

    // --- TIMING ---

    IEnumerator StartTimer()
    {
        float timer = timeLimit;

        while (timer > 0)
        {
            if (timerFillImage != null)
            {
                timerFillImage.fillAmount = timer / timeLimit;
            }

            timer -= Time.deltaTime;
            yield return null;
        }

        if (isActive)
        {
            Failure();
        }
    }

    // --- INPUT SYSTEM LOGICA ---

    private void StartListeningForInput()
    {
        // Zorg ervoor dat er geen oude acties zijn
        if (inputAction != null)
        {
            StopListeningForInput();
        }

        inputAction = new InputAction(binding: requiredPath);
        inputAction.performed += ctx => OnInputPerformed();
        inputAction.Enable();
    }

    private void StopListeningForInput()
    {
        if (inputAction != null)
        {
            inputAction.performed -= ctx => OnInputPerformed();
            inputAction.Disable();
            inputAction.Dispose();
            inputAction = null;
        }
    }

    private void OnInputPerformed()
    {
        if (isActive)
        {
            Success();
        }
    }

    // --- UI FUNCTIES ---

    void SetButtonIcon(string path)
    {
        if (buttonIconImage == null) return;

        buttonIconImage.color = Color.white;
        buttonIconImage.gameObject.SetActive(true);

        foreach (var mapping in buttonMappings)
        {
            if (mapping.controlPath == path)
            {
                buttonIconImage.sprite = mapping.sprite;
                return;
            }
        }
        buttonIconImage.gameObject.SetActive(false);
    }

    // --- RESULTAAT HANTERING (PER STAP) ---

    void Success()
    {
        currentSuccesses++;

        // Stop ALLEEN de timer
        StopAllCoroutines();

        Debug.Log($"QTE Succes! ({currentSuccesses}/{requiredSuccesses})");

        if (currentSuccesses >= requiredSuccesses)
        {
            // Alle opdrachten voltooid (hele reeks succes)
            StartCoroutine(CompleteEvent(true));
        }
        else
        {
            // Start de volgende stap met een vertraging van één frame
            StartCoroutine(StartNextQTEAfterDelay());
        }
    }

    // VEREIST VOOR DE BUGFIX: Stelt het starten van de volgende QTE uit.
    IEnumerator StartNextQTEAfterDelay()
    {
        // Wacht één frame om zeker te zijn dat de vorige input-cyclus is afgerond.
        yield return null;

        // Start nu de volgende stap veilig
        StartNewQTE();
    }

    void Failure()
    {
        isActive = false;
        StopAllCoroutines();

        Debug.Log("QTE MISLUKT!");
        // Roep de opruiming aan (hele reeks mislukt)
        StartCoroutine(CompleteEvent(false));
    }

    // --- OPZETTEN EN OPruimen (HELE REEKS) ---

    // Coroutine om de Input Action veilig uit te schakelen en de scene op te ruimen.
    IEnumerator CompleteEvent(bool wasSuccess)
    {
        yield return null;

        // Re-enable player movement
        if (playerMovement != null)
            playerMovement.canMove = true;

        StopListeningForInput();

        if (wasSuccess)
        {
            if (spawnedEnemy != null)
                Destroy(spawnedEnemy);
        }
        else
        {
            // Failure logic
        }

        gameObject.SetActive(false);
    }
}