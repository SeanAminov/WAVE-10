using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Zombie : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] int maxHealth = 3;
    [SerializeField] float moveSpeed = 1.2f;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] float attackAnimLockTime = 1.0f;
    [SerializeField] float attackRange = 2.0f;

    [Header("Animation")]
    [SerializeField] Animator animator;
    [SerializeField] float deathDestroyDelay = 3f;

    [Header("Audio")]
    [SerializeField] AudioClip[] groanSounds;
    [SerializeField] AudioClip[] attackSounds;
    [SerializeField] AudioClip[] hurtSounds;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip biteSound;

    [Header("References")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] AudioSource audioSource;

    int currentHealth;
    Transform player;
    float lastAttackTime = -10f;

    bool isDead = false;
    bool isAttacking = false;
    float nextGroanTime;

    void Start()
    {
        currentHealth = maxHealth;

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = attackRange * 0.7f;
            agent.angularSpeed = 360f;
            agent.acceleration = 8f;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = 30f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;

        nextGroanTime = Time.time + Random.Range(2f, 8f);
    }

    void Update()
    {
        if (isDead || player == null) return;

        // random groaning
        if (groanSounds != null && groanSounds.Length > 0 && Time.time >= nextGroanTime)
        {
            PlayRandomClip(groanSounds, 0.2f);
            nextGroanTime = Time.time + Random.Range(5f, 15f);
        }

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            StopMoving();

            if (animator != null)
                animator.SetFloat("speedf", 0f);

            // face the player
            Vector3 lookDir = (player.position - transform.position);
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);

            if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
                Attack();
        }
        else
        {
            ChasePlayer();

            float currentSpeed = agent != null ? agent.velocity.magnitude : 0f;
            if (animator != null)
                animator.SetFloat("speedf", currentSpeed);
        }
    }

    void ChasePlayer()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    void StopMoving()
    {
        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;
    }

    void Attack()
    {
        if (isDead || isAttacking || player == null)
            return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackRange)
            return;

        isAttacking = true;
        lastAttackTime = Time.time;

        if (animator != null)
            animator.SetTrigger("attackTrigger");

        if (attackSounds != null && attackSounds.Length > 0)
            PlayRandomClip(attackSounds, 0.35f);
        if (biteSound != null)
            audioSource.PlayOneShot(biteSound, 0.28f);

        PlayerHealth hp = player.GetComponent<PlayerHealth>();
        if (hp != null)
            hp.TakeDamage(attackDamage);

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

        if (hurtSounds != null && hurtSounds.Length > 0)
            PlayRandomClip(hurtSounds, 0.28f);

        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator HitFlash()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            foreach (var mat in r.materials)
            {
                if (mat.HasProperty("_Color"))
                    mat.color = Color.red;
            }
        }

        yield return new WaitForSeconds(0.1f);

        foreach (var r in renderers)
        {
            foreach (var mat in r.materials)
            {
                if (mat.HasProperty("_Color"))
                    mat.color = Color.white;
            }
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        isAttacking = false;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (deathSound != null)
            audioSource.PlayOneShot(deathSound, 0.4f);

        if (animator != null)
        {
            animator.SetFloat("speedf", 0f);
            animator.SetBool("isDead", true);
            animator.SetInteger("deathType", Random.Range(0, 2));
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
            col.enabled = false;

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

    public void SetStats(int health, float speed)
    {
        maxHealth = health;
        currentHealth = health;
        moveSpeed = speed;

        if (agent != null && agent.isOnNavMesh)
            agent.speed = speed;
    }

    void PlayRandomClip(AudioClip[] clips, float volume)
    {
        if (clips == null || clips.Length == 0) return;
        audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)], volume);
    }
}
