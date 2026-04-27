using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public enum PowerUpType
    {
        Health,
        DamageBoost,
        SpeedBoost
    }

    [Header("Powerup Settings")]
    [SerializeField] PowerUpType powerUpType;
    [SerializeField] int healthAmount = 1;
    [SerializeField] float damageMultiplier = 1.5f;
    [SerializeField] float speedMultiplier = 1.6f;
    [SerializeField] float boostDuration = 10f;

    [Header("Pickup Settings")]
    [SerializeField] float destroyDelay = 0f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        switch (powerUpType)
        {
            case PowerUpType.Health:
                ApplyHealth(other);
                break;
            case PowerUpType.DamageBoost:
                ApplyDamage(other);
                break;
            case PowerUpType.SpeedBoost:
                ApplySpeed(other);
                break;
        }

        Destroy(gameObject, destroyDelay);
    }

    void ApplyHealth(Collider player)
    {
        var hp = player.GetComponent<PlayerHealth>();
        if (hp != null) hp.Heal(healthAmount);

        if (UIManager.Instance != null)
            UIManager.Instance.FlashHealthPickup();
    }

    void ApplyDamage(Collider player)
    {
        var shooting = player.GetComponent<PlayerShooting>();
        if (shooting != null) shooting.ApplyDamageBoost(damageMultiplier, boostDuration);

        if (UIManager.Instance != null)
            UIManager.Instance.FlashDamagePickup();
    }

    void ApplySpeed(Collider player)
    {
        var pc = player.GetComponent<PlayerController>();
        if (pc != null) pc.ApplySpeedBoost(speedMultiplier, boostDuration);

        if (UIManager.Instance != null)
            UIManager.Instance.FlashSpeedPickup();
    }
}
