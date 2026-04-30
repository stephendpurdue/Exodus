using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Attack")]
    public Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 0.4f;
    public LayerMask enemyLayers;
    public LayerMask destructibleLayers;
    public AudioClip attackSound;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 moveInput;
    private float attackTimer = 0f;
    private AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Start()
    {
        if (HealthSystem.Instance != null && animator != null)
        {
            HealthSystem.Instance.playerAnimator = animator;
        }

        // Debug: log connected controllers to the console
        string[] controllers = UnityEngine.Input.GetJoystickNames();
        if (controllers.Length > 0)
            Debug.Log("[PlayerController] Controllers detected: " + string.Join(", ", controllers));
        else
            Debug.Log("[PlayerController] No controllers detected.");
    }

    private void Update()
    {
        // ── Movement input ────────────────────────────────────────────────
        moveInput = Vector2.zero;

        // Keyboard
        if (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetKey(KeyCode.UpArrow))
            moveInput.y += 1f;
        if (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetKey(KeyCode.DownArrow))
            moveInput.y -= 1f;
        if (UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetKey(KeyCode.RightArrow))
            moveInput.x += 1f;
        if (UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetKey(KeyCode.LeftArrow))
            moveInput.x -= 1f;

        // Controller (left stick) — raw avoids Unity's smoothing, better for analogue input
        moveInput.x += UnityEngine.Input.GetAxisRaw("Horizontal");
        moveInput.y += UnityEngine.Input.GetAxisRaw("Vertical");

        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();

        // ── Sprite flip ───────────────────────────────────────────────────
        if (moveInput.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (moveInput.x < -0.01f)
            spriteRenderer.flipX = true;

        // ── Animator ──────────────────────────────────────────────────────
        if (animator != null)
            animator.SetBool("isMoving", moveInput.sqrMagnitude > 0.01f);

        // ── Attack cooldown tick ──────────────────────────────────────────
        attackTimer -= Time.deltaTime;

        // ── Attack input — Space OR controller A/Cross button ────────────
        bool attackPressed = Input.GetKeyDown(KeyCode.Space)
                          || Input.GetKeyDown(KeyCode.JoystickButton0);

        if (attackPressed && attackTimer <= 0f)
        {
            Attack();
            attackTimer = attackCooldown;
        }
    }

    // Apply movement in FixedUpdate for consistent physics behavior.
    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    // Attack logic.
    private void Attack()
    {
        if (animator != null)
            animator.SetTrigger("Attack");

        if (audioSource != null && attackSound != null)
            audioSource.PlayOneShot(attackSound);

        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth health = enemy.GetComponentInParent<EnemyHealth>();

            if (health != null)
            {
                Debug.Log($"Hit collider {enemy.name}, applying damage to root {health.gameObject.name}");
                health.TakeDamage(attackDamage);
            }
        }

        Collider2D[] hitDestructibles = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, destructibleLayers);

        foreach (Collider2D destructible in hitDestructibles)
        {
            DestructibleDecoration prop = destructible.GetComponentInParent<DestructibleDecoration>();

            if (prop != null)
            {
                prop.TakeDamage();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
