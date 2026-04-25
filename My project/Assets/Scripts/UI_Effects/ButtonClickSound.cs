using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
{
    [SerializeField] AudioClip clickSound;

    Button button;
    AudioSource audioSource;

    void Awake()
    {
        button = GetComponent<Button>();

        audioSource = FindObjectOfType<AudioSource>();

        if (button != null)
            button.onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
}