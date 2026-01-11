using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[System.Serializable]
public struct ControllerButtonMapping
{
    public string controlPath;
    public Sprite sprite;
}

public class QuickTimeEvent : MonoBehaviour
{
    // ... (QTE Instellingen, UI, Variabelen, Start, OnEnable, OnDisable blijven hetzelfde) ...

    [Header("Instellingen QTE")]
    public float phaseDuration = 1.5f;
    public int requiredSuccesses = 3;

    private int currentSuccesses = 0;

    [Header("UI Elementen")]
    public Image timerFillImage;

    [Header("Controller Iconen")]
    public Image buttonIconImage;
    public ControllerButtonMapping[] buttonMappings;

    [Header("Animatie Instellingen")]
    public string vanishTrigger = "Vanish";
    public string attackTrigger = "Attack";
    public float animationDelay = 1.0f;

    [HideInInspector] public string requiredPath;

    private bool isActive = false;
    private InputAction inputAction;

    private HealthManager healthManager;

    public GameObject spawnedEnemy;
    [HideInInspector] public playerMovementScript playerMovement;

    private Animator enemyAnimator;

    // --- INITIALISATIE ---

    void Start()
    {
        healthManager = Object.FindFirstObjectByType<HealthManager>();
        if (healthManager == null)
        {
            Debug.LogError("HealthManager niet gevonden.");
        }
    }

    private void OnEnable()
    {
        isActive = true;
        currentSuccesses = 0;

        if (spawnedEnemy != null)
        {
            enemyAnimator = spawnedEnemy.GetComponent<Animator>();
        }

        StartNewQTE();
    }

    private void OnDisable()
    {
        StopListeningForInput();
    }

    void StartNewQTE()
    {
        StopAllCoroutines();
        StopListeningForInput();

        if (buttonMappings.Length > 0)
        {
            int randomIndex = Random.Range(0, buttonMappings.Length);
            requiredPath = buttonMappings[randomIndex].controlPath;
        }

        if (timerFillImage != null)
        {
            timerFillImage.type = Image.Type.Filled;
            timerFillImage.fillMethod = Image.FillMethod.Radial360;
            timerFillImage.fillAmount = 1f;
        }

        SetButtonIcon(requiredPath);
        StartListeningForInput();
        StartCoroutine(RunPhaseTimer());
    }

    // --- TIMING ---

    IEnumerator RunPhaseTimer()
    {
        float timer = phaseDuration;

        while (timer > 0)
        {
            if (timerFillImage != null)
            {
                timerFillImage.fillAmount = timer / phaseDuration;
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
        if (inputAction != null) StopListeningForInput();

        inputAction = new InputAction(binding: requiredPath);
        inputAction.performed += ctx => OnInputPerformed();
        inputAction.Enable();
    }

    private void StopListeningForInput()
    {
        if (inputAction != null)
        {
            inputAction.performed -= ctx => OnInputPerformed();
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

    // --- RESULTAAT HANTERING ---

    void Success()
    {
        currentSuccesses++;
        StopAllCoroutines();

        if (currentSuccesses >= requiredSuccesses)
        {
            StartCoroutine(CompleteEvent(true));
        }
        else
        {
            StartCoroutine(StartNextQTEAfterDelay());
        }
    }

    IEnumerator StartNextQTEAfterDelay()
    {
        yield return null;
        StartNewQTE();
    }

    void Failure()
    {
        isActive = false;
        StopAllCoroutines();
        StartCoroutine(CompleteEvent(false));
    }

    IEnumerator CompleteEvent(bool wasSuccess)
    {
        yield return null;

        StopListeningForInput();

        bool shouldTriggerGameOver = false;

        if (wasSuccess)
        {
            // SUCCES: ALTIJD VANISH
            if (enemyAnimator != null)
            {
                enemyAnimator.SetTrigger(vanishTrigger);
            }
        }
        else // FAAL
        {
            if (healthManager != null)
            {
                int healthBeforeDamage = healthManager.GetCurrentHealth();

                if (healthBeforeDamage <= 1)
                {
                    // Laatste hart: ATTACK animatie
                    if (enemyAnimator != null)
                    {
                        enemyAnimator.SetTrigger(attackTrigger);
                    }
                    shouldTriggerGameOver = true;
                }
                else
                {
                    // Meer dan 1 hart: VANISH animatie
                    if (enemyAnimator != null)
                    {
                        enemyAnimator.SetTrigger(vanishTrigger);
                    }
                }

                // Schade toepassen 
                healthManager.TakeDamage(1);
            }
        }

        // Wacht op de animatie
        if (enemyAnimator != null)
        {
            yield return new WaitForSeconds(animationDelay);
        }

        // GAME OVER LOGICA (Activeer UI en pauzeer tijd)
        if (shouldTriggerGameOver)
        {
            // Belangrijk: De HealthManager activeert de UI en zet Time.timeScale op 0.
            if (healthManager != null)
            {
                healthManager.GameOver();
            }
            // Vernietig hier niet, want we willen dat de vijand zichtbaar blijft in het gepauzeerde scherm.

            // De QTE UI kan worden gedeactiveerd als deze boven het Game Over scherm komt
            gameObject.SetActive(false);

            yield break;
        }


        // Normale afsluiting:

        // SPELERBEWEGING WEER INSCHAKELEN
        if (playerMovement != null)
        {
            playerMovement.canMove = true;
            Debug.Log("Spelerbeweging ingeschakeld na QTE.");
        }

        // Vijand Vernietigen
        if (spawnedEnemy != null)
        {
            Destroy(spawnedEnemy);
        }

        gameObject.SetActive(false);
    }
}