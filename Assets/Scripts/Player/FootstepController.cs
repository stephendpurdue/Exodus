using UnityEngine;

public class FootstepController : MonoBehaviour
{
    [Header("Settings")]
    public float stepInterval = 0.4f;

    [Header("References")]
    [Tooltip("Assign a Material using the URP/Particles/Unlit shader here so it gets included in the build!")]
    public Material dustMaterial;

    private Vector2 _lastPos;
    private float _distanceTravelled;


    // Initializes the last position to the player's starting position
    void Start()
    {
        _lastPos = transform.position;
    }

    // Checks the distance traveled each frame and spawns dust if the player has moved enough
    void Update()
    {
        Vector2 currentPos = transform.position;
        float delta = Vector2.Distance(currentPos, _lastPos);

        if (delta > 0.001f)
        {
            _distanceTravelled += delta;

            if (_distanceTravelled >= stepInterval)
            {
                SpawnDust(currentPos);
                _distanceTravelled = 0f;
            }
        }

        _lastPos = currentPos;
    }

    // Spawns a simple dust particle effect at the given position
    void SpawnDust(Vector2 position)
    {
        GameObject go = new GameObject("FootstepDust");
        go.transform.position = new Vector3(position.x, position.y - 0.2f, -0.8f);
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.duration = 0.4f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.2f, 0.45f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.3f, 1.0f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.07f);
        main.startColor = new Color(0.7f, 0.65f, 0.55f, 0.6f); // dusty tan, semi-transparent
        main.gravityModifier = 0.3f;   // particles fall slightly
        main.maxParticles = 10;
        main.playOnAwake = false;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
        new ParticleSystem.Burst(0f, 5)
        });

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.1f;

        // Fade out over lifetime
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
            new GradientColorKey(new Color(0.7f, 0.65f, 0.55f), 0f),
            new GradientColorKey(new Color(0.7f, 0.65f, 0.55f), 1f)
            },
            new GradientAlphaKey[] {
            new GradientAlphaKey(0.6f, 0f),   // visible at birth
            new GradientAlphaKey(0f, 1f)       // fully faded at death
            }
        );
        colorOverLifetime.color = gradient;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        if (dustMaterial != null)
        {
            renderer.material = dustMaterial;
        }
        else
        {
            Shader defaultShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (defaultShader == null) defaultShader = Shader.Find("Sprites/Default");
            renderer.material = new Material(defaultShader);
            renderer.material.color = Color.white; // let the particle color drive it
        }
        renderer.sortingOrder = 999;

        Destroy(go, 1f);
        ps.Play();
    }
}