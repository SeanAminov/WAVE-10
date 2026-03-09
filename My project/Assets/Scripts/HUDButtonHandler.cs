using UnityEngine;

// handles button clicks on the game over screen
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
