using UnityEngine;
using UnityEngine.UI;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]

    // Represents a single layer in the parallax background, with its own image and movement settings.
    public class ParallaxLayer
    {
        public Image image;
        [Range(0f, 20f)] public float driftAmount = 6f;      // Max pixels of ambient movement
        [Range(0f, 30f)] public float mouseStrength = 8f;    // Max pixels from mouse
        [Range(0f, 2f)] public float driftSpeed = 0.2f;      // How fast it "breathes"
        public Vector2 driftDirection = new Vector2(1f, 0.3f);
    }

    [Header("Layers")]
    public ParallaxLayer[] layers;

    [Header("Mouse")]
    public float mouseSmoothing = 6f;

    Vector2 smoothedMouse;
    Vector2[] originPositions;

    // Called once at start to record original positions of layers.
    void Start()
    {
        originPositions = new Vector2[layers.Length];

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].image != null)
                originPositions[i] = layers[i].image.rectTransform.anchoredPosition;
        }
    }

    // Called every frame to update layer positions based on time and mouse movement.
    void Update()
    {
        Vector2 mouseCentered = new Vector2(
            (Input.mousePosition.x / Screen.width) - 0.5f,
            (Input.mousePosition.y / Screen.height) - 0.5f
        );

        smoothedMouse = Vector2.Lerp(smoothedMouse, mouseCentered, Time.deltaTime * mouseSmoothing);

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].image == null) continue;

            RectTransform rt = layers[i].image.rectTransform;

            Vector2 dir = layers[i].driftDirection.normalized;

            float wave = Mathf.Sin(Time.time * layers[i].driftSpeed * Mathf.PI * 2f);
            Vector2 ambientOffset = dir * wave * layers[i].driftAmount;

            Vector2 mouseOffset = new Vector2(
                smoothedMouse.x * layers[i].mouseStrength,
                smoothedMouse.y * layers[i].mouseStrength
            );

            rt.anchoredPosition = originPositions[i] + ambientOffset + mouseOffset;
        }
    }
}