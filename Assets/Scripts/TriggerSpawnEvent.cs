using UnityEngine;

public class TriggerSpawnEvent : MonoBehaviour
{
    [Header("Instellingen voor Enemy")]
    public GameObject enemyPrefab;
    public Transform spawnPoint;

    [Header("Instellingen voor UI")]
    public GameObject qteIntroPanel;

    [Header("Player Reference (Optional - auto-finds if empty)")]
    public playerMovementScript playerMovement;
    public PlayerCrouch playerCrouch;

    [Header("Animatie Trigger")]
    [Tooltip("Moet overeenkomen met de Animator Trigger op de vijand (bijv. 'Appear').")]
    public string appearTrigger = "Appear";

    private bool hasTriggered = false;
    private GameObject spawnedEnemy;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            // Auto-find player components if not assigned
            if (playerMovement == null || playerCrouch == null)
            {
                GameObject player = other.gameObject;
                if (playerMovement == null)
                {
                    playerMovement = player.GetComponent<playerMovementScript>();
                    Debug.Log($"Auto-found playerMovementScript: {playerMovement != null}");
                }
                if (playerCrouch == null)
                {
                    playerCrouch = player.GetComponent<PlayerCrouch>();
                    Debug.Log($"Auto-found PlayerCrouch: {playerCrouch != null}");
                }
            }

            // 1. Spawnen
            SpawnEnemy();

            // 2. Start Animatie direct (de gewenste actie)
            StartAppearAnimation();

            // 3. Toon Intro UI en zet referenties
            ShowIntroUI();

            hasTriggered = true;
            Debug.Log("Spawn Event & Appear Animatie Geactiveerd!");

            // De trigger wordt vernietigd om heractivering te voorkomen
            Destroy(gameObject);
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log($"Enemy gespawned: {spawnedEnemy.name}");
        }
        else
        {
            Debug.LogWarning("Enemy Prefab of SpawnPoint is niet toegewezen!");
        }
    }

    void StartAppearAnimation()
    {
        if (spawnedEnemy != null)
        {
            Animator enemyAnimator = spawnedEnemy.GetComponent<Animator>();
            if (enemyAnimator != null)
            {
                // Activeer de Appear animatie direct nadat de vijand is gespawnd
                enemyAnimator.SetTrigger(appearTrigger);
                Debug.Log($"Appear animatie getriggerd: {appearTrigger}");
            }
            else
            {
                Debug.LogWarning("Geen Animator component gevonden op de gespawnde enemy!");
            }
        }
    }

    void ShowIntroUI()
    {
        if (qteIntroPanel != null)
        {
            QTEIntroManager introManager = qteIntroPanel.GetComponent<QTEIntroManager>();

            if (introManager != null && spawnedEnemy != null && introManager.quickTimePanel != null)
            {
                // FIRST: Assign references to the QTE panel BEFORE anything else
                QuickTimeEvent qte = introManager.quickTimePanel.GetComponent<QuickTimeEvent>();

                if (qte != null)
                {
                    // Wijs de gespawnde vijand en de spelerreferenties toe FIRST
                    qte.SetSpawnedEnemy(spawnedEnemy);
                    qte.playerMovement = playerMovement;
                    qte.playerCrouch = playerCrouch;
                    Debug.Log($"=== ASSIGNED TO QTE: playerMovement={playerMovement != null}, playerCrouch={playerCrouch != null} ===");
                }
                else
                {
                    Debug.LogWarning("QuickTimeEvent component niet gevonden op het quickTimePanel!");
                    return;
                }

                // THEN: Deactiveer Spelerbeweging
                if (playerMovement != null)
                {
                    playerMovement.canMove = false;
                    Debug.Log("TriggerSpawn: Spelerbeweging gedeactiveerd voor QTE.");
                }
                else
                {
                    Debug.LogError("TriggerSpawn: PlayerMovement is NULL! Can't disable movement!");
                }

                // Deactiveer PlayerCrouch
                if (playerCrouch != null)
                {
                    playerCrouch.canMove = false;
                    Debug.Log("TriggerSpawn: PlayerCrouch gedeactiveerd voor QTE.");
                }
                else
                {
                    Debug.LogError("TriggerSpawn: PlayerCrouch is NULL! Can't disable crouch!");
                }

                // FINALLY: Activeer het Intro Paneel (this will eventually activate the QTE panel)
                qteIntroPanel.SetActive(true);
                Debug.Log("QTE Intro Panel geactiveerd.");
            }
            else
            {
                if (introManager == null)
                {
                    Debug.LogWarning("QTEIntroManager component niet gevonden op qteIntroPanel!");
                }
                if (spawnedEnemy == null)
                {
                    Debug.LogWarning("SpawnedEnemy is null!");
                }
                if (introManager != null && introManager.quickTimePanel == null)
                {
                    Debug.LogWarning("QuickTimePanel is niet toegewezen in QTEIntroManager!");
                }
            }
        }
        else
        {
            Debug.LogWarning("QTE Intro Panel is niet toegewezen!");
        }
    }
}