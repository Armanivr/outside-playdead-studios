using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class Character
    {
        public string characterName;  // Character's name
        public Image headImage;       // UI Image for this character
        public Animator headAnimator; // Animator on that image
    }

    [System.Serializable]
    public class DialogueLine
    {
        public Character speaker;     // Who is speaking
        [TextArea] public string text;
    }

    [Header("UI Elements")]
    public TextMeshProUGUI dialogueText;  // The text box

    [Header("Characters")]
    public Character[] characters;    // All characters in the scene

    [Header("Dialogue Data")]
    public DialogueLine[] dialogueLines;  // Add your dialogue lines here

    [Header("Scene Settings")]
    public string nextSceneName;          // Scene to load after dialogue ends

    private int currentLine = 0;          // Tracks which line we're on

    void Start()
    {
        // Turn off all heads at the start
        foreach (var c in characters)
        {
            if (c.headImage != null)
                c.headImage.gameObject.SetActive(false);
        }

        ShowNextLine();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextLine();
        }
    }

    void ShowNextLine()
    {
        if (currentLine >= dialogueLines.Length)
        {
            // Dialogue finished
            if (dialogueText != null)
                dialogueText.text = "";

            // Turn off all heads
            foreach (var c in characters)
            {
                if (c.headImage != null)
                    c.headImage.gameObject.SetActive(false);
            }

            // Load the next scene if assigned
            if (!string.IsNullOrEmpty(nextSceneName))
                SceneManager.LoadScene(nextSceneName);

            return;
        }

        DialogueLine line = dialogueLines[currentLine];

        // Set the dialogue text
        if (dialogueText != null)
            dialogueText.text = line.text;

        // Disable all heads first
        foreach (var c in characters)
        {
            if (c.headImage != null)
            {
                c.headImage.gameObject.SetActive(false);
                Debug.Log("Disabled: " + c.characterName);
            }
        }

        // Enable the current speaker's head
        if (line.speaker != null)
        {
            if (line.speaker.headImage != null)
            {
                line.speaker.headImage.gameObject.SetActive(true);
                Debug.Log("Enabled: " + line.speaker.characterName);
            }

            // Play talking animation if assigned
            if (line.speaker.headAnimator != null)
            {
                line.speaker.headAnimator.Play("Talking"); // Make sure all animators have a "Talking" state
            }
        }

        currentLine++;
    }
}
