using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int maxHealth = 3;
    [SerializeField] float invincibilityTime = 1f;

    int currentHealth;
    float lastHitTime = -10f;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        // check if we're still in iframes
        if (Time.time - lastHitTime < invincibilityTime)
            return;

        lastHitTime = Time.time;
        currentHealth -= amount;

        // update the health UI
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHealth(currentHealth);
    }

    void Die()
    {
        currentHealth = 0;

        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
