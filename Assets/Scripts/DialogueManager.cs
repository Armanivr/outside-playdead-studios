using TMPro;
using System.Collections; // Needed for Coroutines
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    // These classes define what your data looks like in the Inspector
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
    public TMPro.TextMeshProUGUI dialogueText;

    [Header("UI Containers")]
    public GameObject dialogueBox; 

    [Header("Characters")]
    public Character[] characters;

    [Header("Dialogue Data")]
    public DialogueLine[] dialogueLines;

    [Header("Scene Settings")]
    public string nextSceneName;

    [Header("Player Settings")]
    public string playerTag = "Player";
    private playerMovementScript activePlayerScript;
    private PlayerCrouch activeCrouchScript;
    public float typingSpeed = 0.05f; // Time between letters

    private int currentLine = 0;
    private bool dialogueActive = false; 
    private bool dialoguePaused = false;
    private bool isTyping = false; // New: prevents skipping lines while typing
    private Coroutine typingCoroutine;

    void Start()
    {
        // Hide UI at start
        if (dialogueBox != null) dialogueBox.SetActive(false);
        
        foreach (var c in characters)
            if (c.headImage != null) c.headImage.gameObject.SetActive(false);
    }

    void Update()
    {
        // Checks for Space Bar OR Controller Button South (joystick button 0)
        bool skipInput = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0);

        if (dialogueActive && !dialoguePaused && skipInput)
        {
            // If typing, finish the line instantly. If finished, go to next line.
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogueLines[currentLine - 1].text;
                isTyping = false;
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    public void StartDialogueAtLine(int lineIndex)
    {
        // If we aren't already talking, set the starting point
        if (!dialogueActive) 
        {
            currentLine = lineIndex;
        }

        dialogueActive = true;
        dialoguePaused = false;

        if (dialogueBox != null) dialogueBox.SetActive(true);

        // Find the player and stop them
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            activePlayerScript = player.GetComponent<playerMovementScript>();
            if (activePlayerScript != null) activePlayerScript.canMove = false;

            activeCrouchScript = player.GetComponent<PlayerCrouch>();
            if (activeCrouchScript != null) activeCrouchScript.canMove = false;
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
            {
                // Check if the animator has the "Talking" state before playing it
                if (HasState(line.speaker.headAnimator, "Talking"))
                {
                    line.speaker.headAnimator.Play("Talking");
                }
                else
                {
                    Debug.LogWarning($"Animator on {line.speaker.characterName} does not have a 'Talking' state.");
                }
            }
        }

        // Start the Typewriter effect
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentence(line.text));

        // Logic for pausing
        if (line.pauseAfterLine)
        {
            // We'll let the typewriter finish before pausing logic kicks in
            StartCoroutine(WaitForTypewriterToPause(line));
            currentLine++;
            return;
        }

        currentLine++;
    }

    // This is the "Magic" function
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

    IEnumerator WaitForTypewriterToPause(DialogueLine line)
    {
        // Wait until typewriter is done before hiding the box
        while (isTyping) yield return null;
        yield return new WaitForSeconds(0.5f); // Brief hold so they can read it
        PauseDialogue();
    }

    private void PauseDialogue()
    {
        dialoguePaused = true;
        if (dialogueBox != null) dialogueBox.SetActive(false);
        if (activePlayerScript != null) activePlayerScript.canMove = true;
        if (activeCrouchScript != null) activeCrouchScript.canMove = true;
    }

    private void EndDialogue()
    {
        dialogueActive = false;
        if (dialogueBox != null) dialogueBox.SetActive(false);
        if (activePlayerScript != null) activePlayerScript.canMove = true;
        if (activeCrouchScript != null) activeCrouchScript.canMove = true;

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    // Helper method to check if an animator has a specific state
    private bool HasState(Animator animator, string stateName)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return false;

        // Check all layers for the state
        for (int i = 0; i < animator.layerCount; i++)
        {
            if (animator.HasState(i, Animator.StringToHash(stateName)))
            {
                return true;
            }
        }

        return false;
    }
}