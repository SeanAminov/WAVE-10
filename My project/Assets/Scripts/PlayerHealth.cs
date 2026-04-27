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
        // ignore hits during invincibility frames
        if (Time.time - lastHitTime < invincibilityTime)
            return;

        lastHitTime = Time.time;
        currentHealth -= amount;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHealth(currentHealth);
    }

    void Die()
    {
        currentHealth = 0;

        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
    }

    public int GetHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}
