using UnityEngine;

public class TriggerSpawnEvent : MonoBehaviour
{
    [Header("Instellingen voor Enemy")]
    public GameObject enemyPrefab;    // Het 'blauwdruk' van je enemy
    public Transform spawnPoint;      // De plek waar de enemy moet verschijnen

    [Header("Instellingen voor UI")]
    public GameObject quickTimeUI;    // Het UI paneel (Canvas of Panel)

    [Header("Player Reference")]
    public playerMovementScript playerMovement; // Drag your Player GameObject here

    private bool hasTriggered = false; // Zorgt dat het event maar 1 keer gebeurt
    private GameObject spawnedEnemy;   // Referentie naar de gespawnde enemy

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check of het object dat binnenkomt de speler is
        // Zorg dat je speler de tag "Player" heeft!
        if (other.CompareTag("Player") && !hasTriggered)
        {
            SpawnEnemy();
            ShowUI();

            // Zet op true zodat het niet nog een keer gebeurt als je terugloopt
            hasTriggered = true;
            Debug.Log("Spawn Event Geactiveerd!");
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            // Maakt een kopie van de enemy op de positie van het spawnPoint
            spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log($"Enemy gespawned: {spawnedEnemy.name}");
        }
        else
        {
            Debug.LogWarning("Enemy Prefab of SpawnPoint is niet toegewezen!");
        }
    }

    void ShowUI()
    {
        if (quickTimeUI != null)
        {
            // Show the UI
            quickTimeUI.SetActive(true);

            // Disable player movement
            if (playerMovement != null)
            {
                playerMovement.canMove = false;
                Debug.Log("Player movement disabled for QTE");
            }

            // Assign spawned enemy to QTE
            QuickTimeEvent qte = quickTimeUI.GetComponent<QuickTimeEvent>();
            if (qte != null && spawnedEnemy != null)
            {
                qte.SetSpawnedEnemy(spawnedEnemy);

                // Also pass the player reference to the QTE so it can re-enable movement
                qte.playerMovement = playerMovement;

                Debug.Log("Enemy assigned to QTE!");
            }
            else if (qte == null)
            {
                Debug.LogWarning("QuickTimeEvent component not found on quickTimeUI!");
            }
        }
        else
        {
            Debug.LogWarning("QuickTimeUI is not assigned!");
        }
    }
}