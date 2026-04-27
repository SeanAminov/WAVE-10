using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public int currentWave;
    [HideInInspector] public int killCount;
    [HideInInspector] public bool isGameOver;
    [HideInInspector] public bool isPaused;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (isGameOver) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void AddKill()
    {
        killCount++;
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateKills(killCount);
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        SetCursorVisible(true);

        if (UIManager.Instance != null)
            UIManager.Instance.ShowPauseMenu();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SetCursorVisible(false);

        if (UIManager.Instance != null)
            UIManager.Instance.HidePauseMenu();
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        SetCursorVisible(true);

        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameOver();
    }

    public void WinGame()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        SetCursorVisible(true);

        if (UIManager.Instance != null)
            UIManager.Instance.ShowWinScreen();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
        SceneManager.LoadScene("GameScene");
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
        SceneManager.LoadScene("MainMenu");
    }

    void SetCursorVisible(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }
}
