using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class Character
    {
        public string characterName;   // Character's name
        public Sprite headSprite;      // Static head/facing sprite
    }

    [System.Serializable]
    public class DialogueLine
    {
        public Character speaker;      // Who is speaking
        [TextArea] public string text; // The dialogue text
    }

    [Header("UI Elements")]
    public TextMeshProUGUI dialogueText;  // The text box
    public Image characterHeadImage;      // Image that shows the head

    [Header("Dialogue Data")]
    public DialogueLine[] dialogueLines;  // Add your dialogue lines here

    [Header("Scene Settings")]
    public string nextSceneName;          // Scene to load after dialogue ends

    private int currentLine = 0;          // Tracks which line we're on

    void Start()
    {
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
        if (currentLine < dialogueLines.Length)
        {
            DialogueLine line = dialogueLines[currentLine];

            // Set the text
            if (dialogueText != null)
                dialogueText.text = line.text;

            // Set the head sprite
            if (characterHeadImage != null && line.speaker != null && line.speaker.headSprite != null)
                characterHeadImage.sprite = line.speaker.headSprite;

            currentLine++;
        }
        else
        {
            // Dialogue finished
            if (dialogueText != null)
                dialogueText.text = "";

            // Load the next scene if assigned
            if (!string.IsNullOrEmpty(nextSceneName))
                SceneManager.LoadScene(nextSceneName);
        }
    }
}
