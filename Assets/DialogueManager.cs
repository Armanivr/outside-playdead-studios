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
    public TextMeshProUGUI dialogueText;

    [Header("Characters")]
    public Character[] characters;    // All characters in the scene

    [Header("Dialogue Data")]
    public DialogueLine[] dialogueLines;

    [Header("Scene Settings")]
    public string nextSceneName;

    private int currentLine = 0;

    void Start()
    {
        // Make sure all heads are off at the start
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

            foreach (var c in characters)
                if (c.headImage != null)
                    c.headImage.gameObject.SetActive(false);

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
        }

        currentLine++;
    }
}
