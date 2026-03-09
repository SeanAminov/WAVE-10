using UnityEngine;

public class Zombie : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] int maxHealth = 3;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] float attackRange = 1.5f;

    int currentHealth;
    Transform player;
    float lastAttackTime = -10f;
    bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        // find the player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (isDead || player == null) return;

        // move toward the player
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // stay on the ground

        transform.position += direction * moveSpeed * Time.deltaTime;

        // face the player
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        // attack if close enough
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        PlayerHealth hp = player.GetComponent<PlayerHealth>();
        if (hp != null)
        {
            hp.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // tell the wave manager we died
        WaveManager wm = FindObjectOfType<WaveManager>();
        if (wm != null)
            wm.ZombieKilled();

        if (GameManager.Instance != null)
            GameManager.Instance.AddKill();

        Destroy(gameObject);
    }

    // let the wave manager set our stats when spawning
    public void SetStats(int health, float speed)
    {
        maxHealth = health;
        currentHealth = health;
        moveSpeed = speed;
    }
}
