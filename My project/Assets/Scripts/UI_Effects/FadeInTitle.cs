using UnityEngine;
using TMPro;
using System.Collections;

public class FadeInTitle : MonoBehaviour
{
    [SerializeField] float fadeDuration = 1.5f;

    TextMeshProUGUI titleText;

    void Start()
    {
        titleText = GetComponent<TextMeshProUGUI>();
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        Color c = titleText.color;
        c.a = 0f;
        titleText.color = c;

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            titleText.color = c;
            yield return null;
        }

        c.a = 1f;
        titleText.color = c;
    }
}