using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public int startLine = 0;

    private bool triggered = false; // only for THIS trigger

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return; // already used

        if (other.CompareTag("Player") && dialogueManager != null)
        {
            triggered = true;

            dialogueManager.StartDialogueAtLine(startLine);

            // disable this specific trigger so it can’t be retriggered
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;
        }
    }
}
