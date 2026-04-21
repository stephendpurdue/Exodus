using UnityEngine;
using System.Collections;

public class DestructibleDecoration : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;
    private bool isDestroying = false;

    private void Awake()
    {
        // Grab all sprite renderers so we can color them red (even if the prefab has child visual parts)
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void TakeDamage()
    {
        // Prevent taking multiple hits if already flashing
        if (isDestroying) return;
        isDestroying = true;

        StartCoroutine(DestroySequence());
    }

    private IEnumerator DestroySequence()
    {
        // Flash everything red
        if (spriteRenderers != null && spriteRenderers.Length > 0)
        {
            foreach (var sr in spriteRenderers)
            {
                if (sr != null)
                {
                    sr.color = Color.red;
                }
            }
        }

        // Wait a split second so the player registers the hit flash
        yield return new WaitForSeconds(0.15f);

        // Remove the object
        Destroy(gameObject);
    }
}