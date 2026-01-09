using UnityEngine;

public class TriggerSpawnEvent : MonoBehaviour
{
    [Header("Instellingen voor Enemy")]
    public GameObject enemyPrefab;    // Het 'blauwdruk' van je enemy
    public Transform spawnPoint;      // De plek waar de enemy moet verschijnen

    [Header("Instellingen voor UI")]
    public GameObject quickTimeUI;    // Het UI paneel (Canvas of Panel)

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
            // Maakt het UI element zichtbaar
            quickTimeUI.SetActive(true);

            // Zoek het QuickTimeEvent component en wijs de gespawnde enemy toe
            QuickTimeEvent qte = quickTimeUI.GetComponent<QuickTimeEvent>();
            if (qte != null && spawnedEnemy != null)
            {
                qte.SetSpawnedEnemy(spawnedEnemy);
                Debug.Log("Enemy toegewezen aan QTE!");
            }
            else if (qte == null)
            {
                Debug.LogWarning("QuickTimeEvent component niet gevonden op het quickTimeUI!");
            }
        }
        else
        {
            Debug.LogWarning("QuickTimeUI is niet toegewezen!");
        }
    }
}