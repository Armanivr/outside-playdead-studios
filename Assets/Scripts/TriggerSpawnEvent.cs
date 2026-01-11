using UnityEngine;

public class TriggerSpawnEvent : MonoBehaviour
{
    [Header("Instellingen voor Enemy")]
    public GameObject enemyPrefab;
    public Transform spawnPoint;

    [Header("Instellingen voor UI")]
    public GameObject qteIntroPanel;

    [Header("Player Reference")]
    public playerMovementScript playerMovement;

    [Header("Animatie Trigger")]
    [Tooltip("Moet overeenkomen met de Animator Trigger op de vijand (bijv. 'Appear').")]
    public string appearTrigger = "Appear";

    private bool hasTriggered = false;
    private GameObject spawnedEnemy;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
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
                // Deactiveer Spelerbeweging
                if (playerMovement != null)
                {
                    playerMovement.canMove = false;
                }

                QuickTimeEvent qte = introManager.quickTimePanel.GetComponent<QuickTimeEvent>();

                if (qte != null)
                {
                    // Wijs de gespawnde vijand en de spelerreferentie toe
                    qte.spawnedEnemy = spawnedEnemy;
                    qte.playerMovement = playerMovement;
                }

                // Activeer het Intro Paneel
                qteIntroPanel.SetActive(true);
            }
        }
    }
}