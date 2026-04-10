using UnityEngine;

/// Smooth camera follow for the player in a top-down 2D dungeon.
/// Uses LateUpdate so the camera moves after all player physics have resolved.
/// The Z offset keeps the camera at the correct rendering depth.
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Assigned automatically if left empty — finds the PlayerController in the scene.")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    private void LateUpdate()
    {
        if (target == null)
        {
            // Auto-find player if not assigned (handles first-time spawn)
            var player = FindObjectOfType<PlayerController>();
            if (player != null)
                target = player.transform;
            else
                return;
        }

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
