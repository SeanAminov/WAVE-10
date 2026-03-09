using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public int currentWave = 0;
    [HideInInspector] public int killCount = 0;
    [HideInInspector] public bool isGameOver = false;

    void Awake()
    {
        // basic singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddKill()
    {
        killCount++;
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateKills(killCount);
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
        isGameOver = false;
        SceneManager.LoadScene("GameScene");
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene("MainMenu");
    }
}
