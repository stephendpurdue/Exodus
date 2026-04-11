//==============================================================
// HealthSystem
// HealthSystem.Instance.TakeDamage(float damage);
// HealthSystem.Instance.HealDamage(float heal);
// HealthSystem.Instance.UseMana(float mana);
// HealthSystem.Instance.RestoreMana(float mana);
// Attach to the Hero.
//==============================================================

using System.Collections;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
	public static HealthSystem Instance { get; private set; }

	[Header("Health")]
	public RectTransform currentHealthBar;
	public UnityEngine.UI.Text healthText;
	public float hitPoint = 100f;
	public float maxHitPoint = 100f;

	[Header("Mana")]
	public RectTransform currentManaBar;
	public UnityEngine.UI.Text manaText;
	public float manaPoint = 100f;
	public float maxManaPoint = 100f;

	[Header("Regeneration")]
	public bool regenerate = true;
	public float regenAmount = 0.1f;
	public float regenUpdateInterval = 1f;

	[Header("Debug")]
	public bool godMode = false;

	private float regenTimer = 0f;

	//==============================================================
	// Initialization
	//==============================================================
	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	void Start()
	{
		UpdateGraphics();
		regenTimer = regenUpdateInterval;
	}

	void Update()
	{
		if (regenerate)
			RegenerateStats();
	}

	//==============================================================
	// Regeneration
	//==============================================================
	private void RegenerateStats()
	{
		regenTimer -= Time.deltaTime;

		if (regenTimer <= 0f)
		{
			if (godMode)
			{
				HealDamage(maxHitPoint);
				RestoreMana(maxManaPoint);
			}
			else
			{
				HealDamage(regenAmount);
				RestoreMana(regenAmount);
			}

			UpdateGraphics();
			regenTimer = regenUpdateInterval;
		}
	}

	//==============================================================
	// Health Logic
	//==============================================================
	private void UpdateHealthBar()
	{
		if (currentHealthBar == null || healthText == null)
			return;

		float ratio = Mathf.Clamp01(hitPoint / maxHitPoint);
		currentHealthBar.localScale = new Vector3(ratio, 1f, 1f);
		healthText.text = $"{hitPoint:0}/{maxHitPoint:0}";
	}

	public void TakeDamage(float damage)
	{
		if (godMode)
			return;

		hitPoint = Mathf.Clamp(hitPoint - damage, 0f, maxHitPoint);
		UpdateGraphics();
		StartCoroutine(OnPlayerHurt());
	}

	public void HealDamage(float heal)
	{
		hitPoint = Mathf.Clamp(hitPoint + heal, 0f, maxHitPoint);
		UpdateGraphics();
	}

	public void SetMaxHealth(float percentageIncrease)
	{
		maxHitPoint += maxHitPoint * (percentageIncrease / 100f);
		hitPoint = Mathf.Min(hitPoint, maxHitPoint);
		UpdateGraphics();
	}

	//==============================================================
	// Mana Logic
	//==============================================================
	private void UpdateManaBar()
	{
		if (currentManaBar == null || manaText == null)
			return;

		float ratio = Mathf.Clamp01(manaPoint / maxManaPoint);
		currentManaBar.localScale = new Vector3(ratio, 1f, 1f);
		manaText.text = $"{manaPoint:0}/{maxManaPoint:0}";
	}

	public void UseMana(float mana)
	{
		manaPoint = Mathf.Clamp(manaPoint - mana, 0f, maxManaPoint);
		UpdateGraphics();
	}

	public void RestoreMana(float mana)
	{
		manaPoint = Mathf.Clamp(manaPoint + mana, 0f, maxManaPoint);
		UpdateGraphics();
	}

	public void SetMaxMana(float percentageIncrease)
	{
		maxManaPoint += maxManaPoint * (percentageIncrease / 100f);
		manaPoint = Mathf.Min(manaPoint, maxManaPoint);
		UpdateGraphics();
	}

	//==============================================================
	// UI Update
	//==============================================================
	private void UpdateGraphics()
	{
		UpdateHealthBar();
		UpdateManaBar();
	}

	//==============================================================
	// Death Sequence
	//==============================================================
	private IEnumerator OnPlayerHurt()
	{
		if (PopupText.Instance != null)
			PopupText.Instance.Popup("Ouch!", 1f, 1f);

		if (hitPoint <= 0f)
			yield return StartCoroutine(OnPlayerDied());
		else
			yield break;
	}

	private IEnumerator OnPlayerDied()
	{
		if (PopupText.Instance != null)
			PopupText.Instance.Popup("You have died!", 1f, 1f);

		yield break;
	}
}
