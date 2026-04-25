using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject mainButtonsPanel;
    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject guidePanel;

    void Start()
    {
        // main menu buttons shown by default
        if (mainButtonsPanel != null)
            mainButtonsPanel.SetActive(true);

        // sub panels hidden by default
        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (guidePanel != null)
            guidePanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ShowControls()
    {
        if (mainButtonsPanel != null)
            mainButtonsPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(true);

        if (guidePanel != null)
            guidePanel.SetActive(false);
    }

    public void ShowGuide()
    {
        if (mainButtonsPanel != null)
            mainButtonsPanel.SetActive(false);

        if (guidePanel != null)
            guidePanel.SetActive(true);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);
    }

    public void ClosePanels()
    {
        // return to main menu buttons
        if (mainButtonsPanel != null)
            mainButtonsPanel.SetActive(true);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (guidePanel != null)
            guidePanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}