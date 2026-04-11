using System.Collections;
using UnityEngine;

/// <summary>
/// Simple 3-state FSM for a top-down 2D enemy.
///
/// States:
///   Idle    — enemy stands still, briefly patrolling if a wander radius is set.
///   Chase   — player entered detection range; enemy steers directly toward them.
///   Attack  — player is within melee range; enemy deals damage on a cooldown.
///
/// Requires: Rigidbody2D (Dynamic, gravity 0, FreezeRotation) on the same GameObject.
/// The player must be on a layer included in playerLayer.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    // ── States ───────────────────────────────────────────────────────────────
    private enum State { Idle, Chase, Attack }
    private State currentState = State.Idle;

    // ── References ───────────────────────────────────────────────────────────
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // ── Detection & Movement ─────────────────────────────────────────────────
    [Header("Detection")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float loseAggroRange = 8f;   // give up chasing beyond this

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;

    // ── Attack ───────────────────────────────────────────────────────────────
    [Header("Attack")]
    [SerializeField] private float attackRange  = 0.8f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1.2f;
    private float attackTimer = 0f;

    // ── Idle Wander ──────────────────────────────────────────────────────────
    [Header("Idle Wander (optional)")]
    [SerializeField] private float wanderRadius  = 2f;
    [SerializeField] private float wanderInterval = 3f;
    private Vector2 wanderTarget;
    private float wanderTimer;
    private Vector2 spawnPosition;

    // ─────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure correct Rigidbody2D settings regardless of prefab configuration
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Start()
    {
        spawnPosition = transform.position;
        wanderTarget  = spawnPosition;

        // Find the player by tag — make sure your player GameObject is tagged "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("EnemyAI: No GameObject tagged 'Player' found. Tag your player prefab.");
    }

    private void Update()
    {
        if (player == null) return;

        attackTimer -= Time.deltaTime;
        wanderTimer -= Time.deltaTime;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // ── State transitions ─────────────────────────────────────────────
        switch (currentState)
        {
            case State.Idle:
                if (distToPlayer <= detectionRange)
                    currentState = State.Chase;
                break;

            case State.Chase:
                if (distToPlayer <= attackRange)
                    currentState = State.Attack;
                else if (distToPlayer > loseAggroRange)
                    currentState = State.Idle;
                break;

            case State.Attack:
                if (distToPlayer > attackRange)
                    currentState = State.Chase;
                break;
        }

        // ── State behaviour ───────────────────────────────────────────────
        switch (currentState)
        {
            case State.Idle:   HandleIdle();   break;
            case State.Chase:  HandleChase();  break;
            case State.Attack: HandleAttack(); break;
        }
    }

    // ── Idle: optional gentle wander near spawn ───────────────────────────
    private void HandleIdle()
    {
        if (wanderRadius <= 0f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (wanderTimer <= 0f)
        {
            // Pick a new random point within wanderRadius of spawn
            Vector2 offset = Random.insideUnitCircle * wanderRadius;
            wanderTarget = spawnPosition + offset;
            wanderTimer  = wanderInterval;
        }

        MoveToward(wanderTarget, moveSpeed * 0.5f);

        // Stop wandering once close enough
        if (Vector2.Distance(transform.position, wanderTarget) < 0.2f)
            rb.linearVelocity = Vector2.zero;
    }

    // ── Chase: steer directly toward the player ───────────────────────────
    private void HandleChase()
    {
        MoveToward(player.position, moveSpeed);
    }

    // ── Attack: stand still and deal damage on cooldown ──────────────────
    private void HandleAttack()
    {
        rb.linearVelocity = Vector2.zero;

        if (attackTimer <= 0f)
        {
            DealDamage();
            attackTimer = attackCooldown;
        }
    }

    private void DealDamage()
    {
        // Try the player's HealthSystem (your existing singleton)
        if (HealthSystem.Instance != null)
        {
            HealthSystem.Instance.TakeDamage(attackDamage);
        }
        else
        {
            // Fallback: look for any IDamageable on the player
            Debug.LogWarning("EnemyAI: HealthSystem.Instance is null — player took no damage.");
        }
    }

    // ── Shared movement helper ────────────────────────────────────────────
    private void MoveToward(Vector2 target, float speed)
    {
        Vector2 dir = ((Vector2)target - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * speed;

        // Flip sprite to face movement direction
        if (spriteRenderer != null)
        {
            if (dir.x > 0.01f)       spriteRenderer.flipX = false;
            else if (dir.x < -0.01f) spriteRenderer.flipX = true;
        }
    }

    // ── Gizmos: visualise ranges in the Scene view ───────────────────────
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, loseAggroRange);
    }
}
