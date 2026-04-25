using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD References")]
    [SerializeField] Image[] healthIcons;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] TextMeshProUGUI killText;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] Image crosshair;

    [Header("Powerup UI")]
    [SerializeField] Image screenGlow;
    [SerializeField] Image damageBoostBar;

    [Header("Health Colors")]
    [SerializeField] Color healthFull = new Color(0.85f, 0.1f, 0.1f);
    [SerializeField] Color healthEmpty = new Color(0.2f, 0.2f, 0.2f);

    Coroutine glowRoutine;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // make sure game over panel is hidden at start
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // make sure pause panel is hidden at start
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // make sure screen glow is hidden at start
        if (screenGlow != null)
        {
            Color c = screenGlow.color;
            c.a = 0f;
            screenGlow.color = c;
        }

        // make sure damage boost bar is hidden at start
        if (damageBoostBar != null)
        {
            damageBoostBar.fillAmount = 0f;
            damageBoostBar.gameObject.SetActive(false);
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        for (int i = 0; i < healthIcons.Length; i++)
        {
            if (healthIcons[i] != null)
                healthIcons[i].color = (i < currentHealth) ? healthFull : healthEmpty;
        }
    }

    public void UpdateWave(int wave)
    {
        if (waveText != null)
            waveText.text = "Wave " + wave;
    }

    public void UpdateKills(int kills)
    {
        if (killText != null)
            killText.text = "Kills: " + kills;
    }

    public void UpdateAmmo(int ammo)
    {
        if (ammoText != null)
            ammoText.text = ammo.ToString();
    }

    public void FlashHealthPickup()
    {
        FlashScreen(new Color(1f, 0f, 0f, 0.22f));
    }

    public void FlashDamagePickup()
    {
        FlashScreen(new Color(0f, 1f, 0f, 0.22f));
    }

    void FlashScreen(Color glowColor)
    {
        if (screenGlow == null)
            return;

        if (glowRoutine != null)
            StopCoroutine(glowRoutine);

        glowRoutine = StartCoroutine(ScreenGlowRoutine(glowColor));
    }

    IEnumerator ScreenGlowRoutine(Color glowColor)
    {
        float fadeTime = 0.45f;

        screenGlow.color = glowColor;

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;

            Color c = glowColor;
            c.a = Mathf.Lerp(glowColor.a, 0f, t / fadeTime);
            screenGlow.color = c;

            yield return null;
        }

        Color clear = glowColor;
        clear.a = 0f;
        screenGlow.color = clear;

        glowRoutine = null;
    }

    public void ShowDamageBoostBar(float duration)
    {
        if (damageBoostBar == null)
            return;

        StopCoroutine(nameof(DamageBoostBarRoutine));
        StartCoroutine(DamageBoostBarRoutine(duration));
    }

    IEnumerator DamageBoostBarRoutine(float duration)
    {
        damageBoostBar.gameObject.SetActive(true);
        damageBoostBar.fillAmount = 1f;

        float timer = duration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            damageBoostBar.fillAmount = timer / duration;
            yield return null;
        }

        damageBoostBar.fillAmount = 0f;
        damageBoostBar.gameObject.SetActive(false);
    }

    public void ShowPauseMenu()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (crosshair != null)
            crosshair.enabled = false;
    }

    public void HidePauseMenu()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (crosshair != null)
            crosshair.enabled = true;
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (crosshair != null)
            crosshair.enabled = false;
    }
}