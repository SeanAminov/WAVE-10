using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
{
    [SerializeField] AudioClip clickSound;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Button button;

    void Awake()
    {
        if (button != null)
            button.onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
}
