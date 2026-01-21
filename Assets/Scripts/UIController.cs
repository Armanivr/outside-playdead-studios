using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    void Update()
    {
        // This constantly checks if the Controller "A" button is pressed
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            OnPlayClick();
        }
    }

    // This runs when you press the Controller button OR click the UI Button with a mouse
    public void OnPlayClick()
    {
        SceneManager.LoadScene("IntroScene");
    }

    public void OnQuitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}