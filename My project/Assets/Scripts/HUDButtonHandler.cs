using UnityEngine;

// hooks for the Restart and Menu buttons on the game-over panel
public class HUDButtonHandler : MonoBehaviour
{
    public void OnRestart()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
    }

    public void OnMenu()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.BackToMenu();
    }
}
