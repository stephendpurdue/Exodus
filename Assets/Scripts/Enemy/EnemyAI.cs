using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    private enum State { Idle, Chase, Attack }
    private State currentState = State.Idle;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float loseAggroRange = 8f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1.2f;
    private float attackTimer = 0f;

    [Header("Idle Wander (optional)")]
    [SerializeField] private float wanderRadius = 2f;
    [SerializeField] private float wanderInterval = 3f;
    private Vector2 wanderTarget;
    private float wanderTimer;
    private Vector2 spawnPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        int enemyLayer = gameObject.layer;
        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer >= 0)
            Physics2D.IgnoreLayerCollision(enemyLayer, playerLayer, true);
    }

    private void Start()
    {
        spawnPosition = transform.position;
        wanderTarget = spawnPosition;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("EnemyAI: No GameObject tagged 'Player' found.");
    }

    private void Update()
    {
        if (player == null) return;

        attackTimer -= Time.deltaTime;
        wanderTimer -= Time.deltaTime;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

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

        switch (currentState)
        {
            case State.Idle: HandleIdle(); break;
            case State.Chase: HandleChase(); break;
            case State.Attack: HandleAttack(); break;
        }
    }

    private void HandleIdle()
    {
        if (wanderRadius <= 0f) { SetIdle(); return; }

        if (wanderTimer <= 0f)
        {
            wanderTarget = spawnPosition + (Vector2)(Random.insideUnitCircle * wanderRadius);
            wanderTimer = wanderInterval;
        }

        MoveToward(wanderTarget, moveSpeed * 0.5f);

        if (Vector2.Distance(transform.position, wanderTarget) < 0.2f)
            SetIdle();
    }

    private void HandleChase()
    {
        MoveToward(player.position, moveSpeed);
    }

    private void HandleAttack()
    {
        SetIdle();

        if (attackTimer <= 0f)
        {
            DealDamage();
            attackTimer = attackCooldown;
        }
    }

    private void DealDamage()
    {
        if (HealthSystem.Instance != null)
            HealthSystem.Instance.TakeDamage(attackDamage);
        else
            Debug.LogWarning("EnemyAI: HealthSystem.Instance is null.");

        if (animator != null)
        {
            animator.Play("Enemy_Attack", 0, 0f);
        }
    }
    private void MoveToward(Vector2 target, float speed)
    {
        Vector2 dir = ((Vector2)transform.position - (Vector2)target == Vector2.zero)
            ? Vector2.zero
            : ((Vector2)target - (Vector2)transform.position).normalized;

        rb.linearVelocity = dir * speed;

        if (animator != null)
            animator.SetBool("isMoving", true);

        if (spriteRenderer != null)
        {
            if (dir.x > 0.01f) spriteRenderer.flipX = false;
            else if (dir.x < -0.01f) spriteRenderer.flipX = true;
        }
    }

    private void SetIdle()
    {
        rb.linearVelocity = Vector2.zero;
        if (animator != null)
            animator.SetBool("isMoving", false);
    }

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