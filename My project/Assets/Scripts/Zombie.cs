using UnityEngine;
using System.Collections;

public class Zombie : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] int maxHealth = 3;
    [SerializeField] float moveSpeed = 1.2f;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] float attackAnimLockTime = 1.0f;
    [SerializeField] float attackRange = 1.6f;

    [Header("Animation")]
    [SerializeField] Animator animator;
    [SerializeField] float deathDestroyDelay = 2f;

    int currentHealth;
    Transform player;
    float lastAttackTime = -10f;

    bool isDead = false;
    bool playerContact = false;
    bool isAttacking = false;

    void Start()
    {
        currentHealth = maxHealth;

        // find animator (on child model)
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // find the player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (isDead || player == null) return;

        // distance to player
        float dist = Vector3.Distance(transform.position, player.position);

        // failsafe: if player is clearly out of range, stop attacking
        if (playerContact && dist > attackRange)
        {
            playerContact = false;
            isAttacking = false;
        }

        // attack behavior
        if (playerContact)
        {
            if (animator != null)
                animator.SetFloat("speedf", 0f);

            // attack only if cooldown finished and not mid-animation
            if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
            }

            return;
        }

        // move toward the player
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // stay on the ground

        transform.position += direction * moveSpeed * Time.deltaTime;

        // face the player
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        // update movement animation
        if (animator != null)
            animator.SetFloat("speedf", moveSpeed);
    }

    void Attack()
    {
        if (isDead || isAttacking || player == null)
            return;

        // double-check range before attacking
        float dist = Vector3.Distance(transform.position, player.position);
        if (!playerContact || dist > attackRange)
        {
            playerContact = false;
            return;
        }

        isAttacking = true;
        lastAttackTime = Time.time;

        // trigger attack animation
        if (animator != null)
            animator.SetTrigger("attackTrigger");

        // deal damage
        PlayerHealth hp = player.GetComponent<PlayerHealth>();
        if (hp != null)
        {
            hp.TakeDamage(attackDamage);
        }

        // unlock after animation delay
        StartCoroutine(EndAttackLock());
    }

    IEnumerator EndAttackLock()
    {
        yield return new WaitForSeconds(attackAnimLockTime);
        isAttacking = false;
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
        if (isDead) return;

        isDead = true;
        playerContact = false;
        isAttacking = false;

        // trigger random death animation
        if (animator != null)
        {
            animator.SetFloat("speedf", 0f);
            animator.SetBool("isDead", true);
            animator.SetInteger("deathType", Random.Range(0, 2));
        }

        // disable colliders so zombie stops interacting
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
            col.enabled = false;

        // stop movement
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = Vector3.zero;

        // tell the wave manager we died
        WaveManager wm = FindObjectOfType<WaveManager>();
        if (wm != null)
            wm.ZombieKilled();

        if (GameManager.Instance != null)
            GameManager.Instance.AddKill();

        StartCoroutine(DestroyAfterDeath());
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(deathDestroyDelay);
        Destroy(gameObject);
    }

    // let the wave manager set our stats when spawning
    public void SetStats(int health, float speed)
    {
        maxHealth = health;
        currentHealth = health;
        moveSpeed = speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            playerContact = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            playerContact = false;
            isAttacking = false;
        }
    }
}