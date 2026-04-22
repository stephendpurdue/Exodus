using System.Collections;
using UnityEngine;

// Attach to every enemy prefab.
// Receives damage from PlayerController's Attack() overlap check.
// Plays a hurt colour flash, then destroys the GameObject on death.

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 30f;
    [SerializeField] private float currentHealth;

    [Header("Hurt Flash")]
    [SerializeField] private Color hurtColour = Color.red;
    [SerializeField] private float hurtFlashDuration = 0.15f;

    [Header("Death")]
    [SerializeField] private float deathDelay = 2f; // small delay so death flash is visible

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color originalColour;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (spriteRenderer != null)
            originalColour = spriteRenderer.color;
    }

    // Called by PlayerController.Attack() for every enemy in the overlap circle.
    private Coroutine hurtFlashRoutine;

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth > 0f)
        {
            if (hurtFlashRoutine != null)
                StopCoroutine(hurtFlashRoutine);

            hurtFlashRoutine = StartCoroutine(HurtFlash());
        }

        if (currentHealth <= 0f)
            StartCoroutine(Die());
    }

    // Flashes the sprite to the hurt colour, then back to original after a short delay.
    private IEnumerator HurtFlash()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = hurtColour;
        yield return new WaitForSeconds(hurtFlashDuration);

        if (!isDead)
            spriteRenderer.color = originalColour;
    }

    // Plays death animation, disables components, then destroys the GameObject after a delay.
    private IEnumerator Die()
    {
        if (isDead) yield break;
        isDead = true;

        if (hurtFlashRoutine != null)
            StopCoroutine(hurtFlashRoutine);

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        var ai = GetComponent<EnemyAI>();
        if (ai != null) ai.enabled = false;

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (animator != null)
        {
            animator.ResetTrigger("Death");
            animator.SetBool("isMoving", false);
            animator.Play("Enemy_Death", 0, 2f);
        }

        if (EnemyTracker.Instance != null)
            EnemyTracker.Instance.RegisterEnemyKilled();

        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }

    public float HealthRatio => currentHealth / maxHealth;
}