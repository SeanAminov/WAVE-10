using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Health Colors")]
    [SerializeField] Color healthFull = new Color(0.85f, 0.1f, 0.1f);
    [SerializeField] Color healthEmpty = new Color(0.2f, 0.2f, 0.2f);

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