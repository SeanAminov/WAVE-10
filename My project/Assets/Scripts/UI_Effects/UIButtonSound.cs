using UnityEngine;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip clickSound;

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
