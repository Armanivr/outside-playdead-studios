using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Blijft nodig voor de knop 'Return to Main Menu'

public class HealthManager : MonoBehaviour
{
    [Header("Gezondheid UI Instellingen")]
    [Tooltip("Sleep de UI Image objecten hierheen, 1 voor elk hart.")]
    public List<Image> healthIcons;

    [Header("Game Over Instellingen")]
    [Tooltip("Het UI-paneel dat verschijnt bij Game Over (moet de 'Return to Main Menu' knop bevatten).")]
    public GameObject gameOverUIPanel; // Referentie naar het Game Over UI Paneel

    private int maxHealth;
    private int currentHealth;

    void Awake()
    {
        if (healthIcons.Count == 0)
        {
            Debug.LogError("HealthManager mist Health Icons in de lijst.");
            return;
        }
        maxHealth = healthIcons.Count;
        currentHealth = maxHealth;

        // Zorg ervoor dat het Game Over Paneel bij de start is uitgeschakeld
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
            // We laten de QTE.CompleteEvent de Game Over functie aanroepen ná de animatie.
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

    // De functie die Game Over afhandelt (NU: activeert UI paneel)
    public void GameOver()
    {
        Debug.Log("GAME OVER! Speler overleden. Activeer Game Over UI.");

        // Schakel het Game Over UI paneel in
        if (gameOverUIPanel != null)
        {
            gameOverUIPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("GameOver UI Paneel is niet toegewezen in HealthManager!");
        }

        // Optioneel: Pauzeer de tijd zodat het spel stopt
        Time.timeScale = 0f;
    }

    // VOEG DEZE FUNCTIE TOE AAN DE KNOP 'Return to Main Menu' OP JE UI PANEEL
    public void ReturnToMainMenu(string mainMenuSceneName)
    {
        // Herstel de tijdsschaal
        Time.timeScale = 1f;

        // Laad het hoofdmenu
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError("Hoofdmenu scène naam mist!");
        }
    }
}