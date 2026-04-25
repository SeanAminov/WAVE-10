using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public int currentWave = 0;
    [HideInInspector] public int killCount = 0;
    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public bool isPaused = false;

    void Awake()
    {
        // basic singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (isGameOver)
            return;

        // pause / resume with escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
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

        // show cursor for pause menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowPauseMenu();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        // lock cursor again for FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (UIManager.Instance != null)
            UIManager.Instance.HidePauseMenu();
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        // show cursor again
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameOver();
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
}