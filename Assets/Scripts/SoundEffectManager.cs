using UnityEngine;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance;

    [SerializeField] private Slider sfxSlider;

    private AudioSource audioSource;
    private SoundEffectLibrary soundEffectLibrary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            soundEffectLibrary = GetComponent<SoundEffectLibrary>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(SetVolume);
            SetVolume(sfxSlider.value);
        }
    }

    /* -------------------- ONE SHOT SOUNDS -------------------- */
    public static void Play(string soundName)
    {
        if (Instance == null) return;

        AudioClip clip = Instance.soundEffectLibrary.GetRandomClip(soundName);
        if (clip != null)
        {
            Instance.audioSource.PlayOneShot(clip);
        }
    }

    /* -------------------- FOOTSTEPS (LOOPING) -------------------- */
    public static void PlayFootsteps(float normalizedSpeed)
    {
        if (Instance == null) return;

        if (!Instance.audioSource.isPlaying)
        {
            Instance.audioSource.clip =
                Instance.soundEffectLibrary.GetRandomClip("Footstep");

            Instance.audioSource.loop = true;
            Instance.audioSource.Play();
        }

        // Speed-based pitch
        Instance.audioSource.pitch = Mathf.Lerp(0.8f, 1.3f, normalizedSpeed);
    }

    public static void StopFootsteps()
    {
        if (Instance == null) return;

        if (Instance.audioSource.isPlaying)
        {
            Instance.audioSource.Stop();
            Instance.audioSource.loop = false;
            Instance.audioSource.pitch = 1f;
        }
    }

    /* -------------------- VOLUME -------------------- */
    private void SetVolume(float value)
    {
        audioSource.volume = value;
    }
}
