using UnityEngine;
using UnityEngine.UI;

public class MenuAtmosphere : MonoBehaviour
{
    [Header("Vignette")]
    [Range(0f, 1f)] public float vignetteIntensity = 0.85f;
    [Range(0.1f, 1f)] public float vignetteRadius = 0.75f;

    [Header("Colour Grade")]
    public Color gradeColour = new Color(0.1f, 0f, 0.12f, 0.15f);

    [Header("Embers")]
    public int emberCount = 40;
    public Color emberColour = new Color(1f, 0.35f, 0.05f, 0.7f);
    [Range(0.001f, 0.05f)] public float emberMinSize = 0.003f;
    [Range(0.001f, 0.05f)] public float emberMaxSize = 0.008f;
    public float riseSpeed = 0.08f;
    public float driftAmount = 0.03f;
    public float flickerSpeed = 3f;

    struct Ember
    {
        public RectTransform rect;
        public Image image;
        public float speed;
        public float drift;
        public float driftOffset;
        public float baseAlpha;
        public float flickerOffset;
    }

    Ember[] embers;
    RectTransform canvasRect;

    void Start()
    {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        CreateColourGrade();
        CreateVignette();
        CreateEmbers();
    }

    // ─── Colour Grade ────────────────────────────────────────────────

    void CreateColourGrade()
    {
        GameObject go = new GameObject("ColourGrade");
        go.transform.SetParent(transform, false);

        Image img = go.AddComponent<Image>();
        img.color = gradeColour;
        img.raycastTarget = false;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        go.transform.SetAsFirstSibling();
    }

    // ─── Vignette ────────────────────────────────────────────────────

    void CreateVignette()
    {
        GameObject go = new GameObject("Vignette");
        go.transform.SetParent(transform, false);

        Image img = go.AddComponent<Image>();
        img.sprite = GenerateVignetteSprite(256);
        img.color = Color.black;
        img.raycastTarget = false;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    Sprite GenerateVignetteSprite(int res)
    {
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;

        Color[] pixels = new Color[res * res];
        Vector2 centre = new Vector2(0.5f, 0.5f);

        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                Vector2 uv = new Vector2((float)x / res, (float)y / res);
                float dist = Vector2.Distance(uv, centre);
                float alpha = Mathf.Clamp01((dist - (vignetteRadius * 0.5f)) / (1f - vignetteRadius)) * vignetteIntensity;
                pixels[y * res + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
    }

    // ─── Embers ──────────────────────────────────────────────────────

    void CreateEmbers()
    {
        embers = new Ember[emberCount];

        for (int i = 0; i < emberCount; i++)
        {
            GameObject go = new GameObject($"Ember_{i}");
            go.transform.SetParent(transform, false);

            Image img = go.AddComponent<Image>();
            float alpha = Random.Range(0.2f, emberColour.a);
            img.color = new Color(emberColour.r, emberColour.g, emberColour.b, alpha);
            img.raycastTarget = false;

            RectTransform rt = go.GetComponent<RectTransform>();
            float size = Random.Range(emberMinSize, emberMaxSize) * canvasRect.rect.width;
            rt.sizeDelta = new Vector2(size, size);

            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot = new Vector2(0.5f, 0f);

            rt.anchoredPosition = RandomScreenPosition(true);

            embers[i] = new Ember
            {
                rect = rt,
                image = img,
                speed = Random.Range(riseSpeed * 0.5f, riseSpeed * 1.5f),
                drift = Random.Range(-driftAmount, driftAmount),
                driftOffset = Random.Range(0f, Mathf.PI * 2f),
                baseAlpha = alpha,
                flickerOffset = Random.Range(0f, Mathf.PI * 2f)
            };
        }
    }

    void Update()
    {
        float screenH = canvasRect.rect.height;
        float screenW = canvasRect.rect.width;

        for (int i = 0; i < embers.Length; i++)
        {
            Ember e = embers[i];

            // Guard against uninitialised embers
            if (e.image == null || e.rect == null) continue;

            // ── Position ──
            Vector2 pos = e.rect.anchoredPosition;
            pos.y += e.speed * screenH * Time.deltaTime;
            pos.x += Mathf.Sin(Time.time + e.driftOffset) * e.drift * screenW * Time.deltaTime;
            e.rect.anchoredPosition = pos;

            // ── Flicker ──
            float flicker = Mathf.Sin((Time.time + e.flickerOffset) * flickerSpeed);
            float noise = Random.Range(-0.05f, 0.05f);
            float newAlpha = Mathf.Clamp01(e.baseAlpha + (flicker * 0.15f) + noise);

            Color c = e.image.color;
            c.a = newAlpha;
            e.image.color = c;

            // ── Respawn ──
            if (pos.y > screenH)
            {
                e.rect.anchoredPosition = RandomScreenPosition(false);
            }

            embers[i] = e;
        }
    }

    Vector2 RandomScreenPosition(bool scatter)
    {
        float screenW = canvasRect.rect.width;
        float screenH = canvasRect.rect.height;

        float x = Random.Range(-screenW * 0.5f, screenW * 0.5f);
        float y = scatter
            ? Random.Range(0f, screenH)
            : Random.Range(-50f, 0f);

        return new Vector2(x, y);
    }
}