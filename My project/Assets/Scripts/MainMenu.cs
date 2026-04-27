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
        ShowMainPanel();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ShowControls()
    {
        SetPanels(false, true, false);
    }

    public void ShowGuide()
    {
        SetPanels(false, false, true);
    }

    public void ClosePanels()
    {
        ShowMainPanel();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void ShowMainPanel()
    {
        SetPanels(true, false, false);
    }

    void SetPanels(bool main, bool controls, bool guide)
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(main);
        if (controlsPanel != null) controlsPanel.SetActive(controls);
        if (guidePanel != null) guidePanel.SetActive(guide);
    }
}
