using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections; // BELANGRIJK: Nodig voor Coroutines

public class HealthManager : MonoBehaviour
{
    [Header("Gezondheid UI Instellingen")]
    public List<Image> healthIcons;

    [Header("Game Over Instellingen")]
    public GameObject gameOverUIPanel;
    public string mainMenuSceneName = "MainMenu";

    [Header("Audio Instellingen")]
    public AudioClip gameOverSound;
    [Tooltip("De AudioSource die de omgevingsgeluiden (wildlife) afspeelt.")]
    public AudioSource ambientSoundSource;
    private AudioSource audioSource; // De AudioSource op dit GameObject (voor de Game Over sound)

    private const string ReturnToMenuControlPath = "<Gamepad>/buttonSouth";
    private InputAction returnAction;

    private int maxHealth;
    private int currentHealth;

    void Awake()
    {
        // Initialisatie van de lokale AudioSource (voor Game Over geluid)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("HealthManager mist een AudioSource component voor het Game Over geluid.");
        }

        // Controle of de Ambient Sound Source is toegewezen
        if (ambientSoundSource == null)
        {
            Debug.LogWarning("Ambient Sound Source is niet toegewezen in HealthManager. Wildlife geluid kan niet gestopt worden.");
        }


        if (healthIcons.Count == 0)
        {
            Debug.LogError("HealthManager mist Health Icons in de lijst.");
            return;
        }
        maxHealth = healthIcons.Count;
        currentHealth = maxHealth;

        if (gameOverUIPanel != null)
        {
            gameOverUIPanel.SetActive(false);
        }

        UpdateHealthUI();
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            UpdateHealthUI();
        }
        else
        {
            UpdateHealthUI();
        }
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i < maxHealth; i++)
        {
            healthIcons[i].gameObject.SetActive(i < currentHealth);
        }
    }


    public void GameOver()
    {
        Debug.Log("GAME OVER! Speler overleden. Activeer Game Over UI.");

        // STOP WILDLIFE GELUID
        if (ambientSoundSource != null)
        {
            ambientSoundSource.Stop();
        }

        // SPEEL GAME OVER GELUID AF
        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        if (gameOverUIPanel != null)
        {
            gameOverUIPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("GameOver UI Paneel is niet toegewezen in HealthManager!");
        }

        // Pauzeer de tijd en start input luisteren
        Time.timeScale = 0f;
        StartListeningForReturn();
    }

    // --- GEFIXTE INPUT LOGICA ---

    public void ReturnToMainMenu(string sceneName)
    {
        // START NU DE COROUTINE OM DE CLEANUP VEILIG UIT TE VOEREN
        StartCoroutine(DelayedCleanupAndSceneLoad(sceneName));
    }

    private void StartListeningForReturn()
    {
        CleanupReturnActionImmediate(); // Ruim direct op, als er al iets liep

        returnAction = new InputAction(binding: ReturnToMenuControlPath);

        // De callback roept de functie aan die de Coroutine start
        returnAction.performed += ctx => ReturnToMainMenu(mainMenuSceneName);

        returnAction.Enable();
    }

    // NIEUWE COROUTINE OM DE FOUT OP TE LOSSEN
    IEnumerator DelayedCleanupAndSceneLoad(string sceneName)
    {
        // 1. Herstel de tijdsschaal
        Time.timeScale = 1f;

        // 2. Wacht één frame. DIT LOST DE FOUT OP!
        yield return null;

        // 3. Vernietig de Action nu het veilig is (buiten de callback)
        CleanupReturnActionImmediate();

        // 4. Laad de scène
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Hoofdmenu scène naam mist!");
        }
    }

    // Functie voor directe opruiming (gebruikt bij het starten van luisteren EN in de Coroutine)
    private void CleanupReturnActionImmediate()
    {
        if (returnAction != null)
        {
            // BELANGRIJK: Zorg dat de delegate-verwijdering overeenkomt met de toevoeging
            returnAction.performed -= ctx => ReturnToMainMenu(mainMenuSceneName);
            returnAction.Dispose();
            returnAction = null;
        }
    }
}