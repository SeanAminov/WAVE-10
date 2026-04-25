using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public enum PowerUpType
    {
        Health,
        DamageBoost
    }

    [Header("Powerup Settings")]
    [SerializeField] PowerUpType powerUpType;
    [SerializeField] int healthAmount = 1;
    [SerializeField] float damageMultiplier = 1.5f;
    [SerializeField] float boostDuration = 10f;

    [Header("Pickup Settings")]
    [SerializeField] float destroyDelay = 0f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (powerUpType == PowerUpType.Health)
        {
            PlayerHealth hp = other.GetComponent<PlayerHealth>();
            if (hp != null)
                hp.Heal(healthAmount);

            if (UIManager.Instance != null)
                UIManager.Instance.FlashHealthPickup();
        }

        if (powerUpType == PowerUpType.DamageBoost)
        {
            PlayerShooting shooting = other.GetComponent<PlayerShooting>();
            if (shooting != null)
                shooting.ApplyDamageBoost(damageMultiplier, boostDuration);
            
            if (UIManager.Instance != null)
                UIManager.Instance.FlashDamagePickup();
        }

        Destroy(gameObject, destroyDelay);
    }
}