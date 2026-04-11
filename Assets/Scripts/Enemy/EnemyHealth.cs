using System.Collections;
using UnityEngine;

/// <summary>
/// Attach to every enemy prefab.
/// Receives damage from PlayerController's Attack() overlap check.
/// Plays a hurt colour flash, then destroys the GameObject on death.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 30f;
    [SerializeField] private float currentHealth;

    [Header("Hurt Flash")]
    [SerializeField] private Color hurtColour = Color.red;
    [SerializeField] private float hurtFlashDuration = 0.15f;

    [Header("Death")]
    [SerializeField] private float deathDelay = 0.3f; // small delay so death flash is visible

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color originalColour;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColour = spriteRenderer.color;
    }

    /// <summary>
    /// Called by PlayerController.Attack() for every enemy in the overlap circle.
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (spriteRenderer != null)
            StartCoroutine(HurtFlash());

        if (currentHealth <= 0f)
            StartCoroutine(Die());
    }

    private IEnumerator HurtFlash()
    {
        spriteRenderer.color = hurtColour;
        yield return new WaitForSeconds(hurtFlashDuration);
        if (!isDead && spriteRenderer != null)
            spriteRenderer.color = originalColour;
    }

    private IEnumerator Die()
    {
        isDead = true;

        // Disable AI and collider immediately so the enemy stops acting
        var ai = GetComponent<EnemyAI>();
        if (ai != null) ai.enabled = false;

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Flash white on death
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        if (animator != null)
            animator.SetTrigger("Death");

        yield return new WaitForSeconds(deathDelay);

        Destroy(gameObject);
    }

    /// <summary>
    /// Optional: expose current health as a 0-1 ratio for a health bar.
    /// </summary>
    public float HealthRatio => currentHealth / maxHealth;
}
