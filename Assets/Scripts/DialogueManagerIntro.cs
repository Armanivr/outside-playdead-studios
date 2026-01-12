using System.Collections; // Needed for the typewriter timer
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroDialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class Character
    {
        public string characterName;
        public UnityEngine.UI.Image headImage;
        public Animator headAnimator;
    }

    [System.Serializable]
    public class DialogueLine
    {
        public Character speaker;
        [TextArea] public string text;
        public bool pauseAfterLine;
    }

    [Header("UI Elements")]
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueBox;

    [Header("Settings")]
    public float typingSpeed = 0.05f; // Speed of the typewriter effect
    public string playerTag = "Player";

    [Header("Dialogue Data")]
    public Character[] characters;
    public DialogueLine[] dialogueLines;

    [Header("Scene Settings")]
    public string nextSceneName;

    private playerMovementScript activePlayerScript;
    private int currentLine = 0;
    private bool dialogueActive = false;
    private bool dialoguePaused = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        // Since this is for the intro, we start it immediately!
        StartDialogueAtLine(0);
    }

    void Update()
    {
        // Space or Controller Button South (A/Cross)
        bool skipInput = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0);

        if (dialogueActive && !dialoguePaused && skipInput)
        {
            if (isTyping)
            {
                // Instant finish if player presses button while typing
                FinishLineInstantly();
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    public void StartDialogueAtLine(int lineIndex)
    {
        currentLine = lineIndex;
        dialogueActive = true;
        dialoguePaused = false;

        if (dialogueBox != null) dialogueBox.SetActive(true);

        // Lock player movement if one exists in the intro scene
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            activePlayerScript = player.GetComponent<playerMovementScript>();
            if (activePlayerScript != null) activePlayerScript.canMove = false;
        }

        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (currentLine >= dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueLines[currentLine];

        // UI/Head Updates
        foreach (var c in characters)
            if (c.headImage != null) c.headImage.gameObject.SetActive(false);

        if (line.speaker != null && line.speaker.headImage != null)
        {
            line.speaker.headImage.gameObject.SetActive(true);
            if (line.speaker.headAnimator != null)
                line.speaker.headAnimator.Play("Talking");
        }

        // Start Typewriter
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentence(line.text));

        // Logic for pausing
        if (line.pauseAfterLine)
        {
            StartCoroutine(WaitForTypewriterToPause());
            currentLine++;
            return;
        }

        currentLine++;
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        isTyping = true;

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void FinishLineInstantly()
    {
        StopCoroutine(typingCoroutine);
        dialogueText.text = dialogueLines[currentLine - 1].text;
        isTyping = false;
    }

    IEnumerator WaitForTypewriterToPause()
    {
        while (isTyping) yield return null;
        yield return new WaitForSeconds(0.5f);
        PauseDialogue();
    }

    private void PauseDialogue()
    {
        dialoguePaused = true;
        if (dialogueBox != null) dialogueBox.SetActive(false);
        if (activePlayerScript != null) activePlayerScript.canMove = true;
    }

    private void EndDialogue()
    {
        dialogueActive = false;
        if (dialogueBox != null) dialogueBox.SetActive(false);
        if (activePlayerScript != null) activePlayerScript.canMove = true;

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}