using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManagerIntro : MonoBehaviour
{
    [System.Serializable]
    public class Character
    {
        public string characterName;   // Just for clarity in Inspector
        public Image headImage;        // UI Image for this character
        public Animator headAnimator;  // Animator on that image
    }

    [System.Serializable]
    public class DialogueLine
    {
        public Character speaker;      // MUST be dragged from Characters[]
        [TextArea] public string text;
    }

    [Header("UI")]
    public TextMeshProUGUI dialogueText;

    [Header("Characters")]
    public Character[] characters;    // Jamal, Quinten, Lucas

    [Header("Dialogue")]
    public DialogueLine[] dialogueLines;

    [Header("Scene")]
    public string nextSceneName;

    private int currentLine = 0;

    void Start()
    {
        // Turn off all heads at start
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
            // End of dialogue
            if (dialogueText != null)
                dialogueText.text = "";

            foreach (var c in characters)
            {
                if (c.headImage != null)
                    c.headImage.gameObject.SetActive(false);
            }

            if (!string.IsNullOrEmpty(nextSceneName))
                SceneManager.LoadScene(nextSceneName);

            return;
        }

        DialogueLine line = dialogueLines[currentLine];

        // Set text
        if (dialogueText != null)
            dialogueText.text = line.text;

        // Disable all heads
        foreach (var c in characters)
        {
            if (c.headImage != null)
                c.headImage.gameObject.SetActive(false);
        }

        // Enable speaker head
        if (line.speaker != null && line.speaker.headImage != null)
        {
            line.speaker.headImage.gameObject.SetActive(true);

            if (line.speaker.headAnimator != null)
                line.speaker.headAnimator.Play(0); // play default state
        }

        currentLine++;
    }
}
