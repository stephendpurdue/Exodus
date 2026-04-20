//==============================================================
// HealthSystem
// HealthSystem.Instance.TakeDamage(float damage);
// HealthSystem.Instance.HealDamage(float heal);
// HealthSystem.Instance.UseMana(float mana);
// HealthSystem.Instance.RestoreMana(float mana);
//==============================================================

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

	[Header("References")]
	public Animator playerAnimator;

	[Header("UI Menus")]
	public GameObject gameOverMenu;

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
	// Regeneration Logic
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
	public void TakeDamage(float damage)
	{
		if (godMode)
			return;

		hitPoint = Mathf.Clamp(hitPoint - damage, 0f, maxHitPoint);
		UpdateGraphics();

		if (hitPoint <= 0f)
			OnPlayerDied();
		else if (PopupText.Instance != null)
			PopupText.Instance.Popup("Ouch!", 1f, 1f);
	}

	public void HealDamage(float heal)
	{
		hitPoint = Mathf.Clamp(hitPoint + heal, 0f, maxHitPoint);
		UpdateGraphics();
	}

	public void SetMaxHealth(float percentageIncrease)
	{
		IncreaseMaxStat(ref maxHitPoint, ref hitPoint, percentageIncrease);
	}

	//==============================================================
	// Mana Logic
	//==============================================================
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
		IncreaseMaxStat(ref maxManaPoint, ref manaPoint, percentageIncrease);
	}

	//==============================================================
	// UI Update
	//==============================================================
	private void UpdateGraphics()
	{
		UpdateBar(currentHealthBar, healthText, hitPoint, maxHitPoint);
		UpdateBar(currentManaBar, manaText, manaPoint, maxManaPoint);
	}

	private void UpdateBar(RectTransform barTransform, UnityEngine.UI.Text barText, float current, float max)
	{
		if (barTransform == null || barText == null)
			return;

		float ratio = Mathf.Clamp01(current / max);
		barTransform.localScale = new Vector3(ratio, 1f, 1f);
		barText.text = $"{current:0}/{max:0}";
	}

	private void IncreaseMaxStat(ref float maxStat, ref float currentStat, float percentageIncrease)
	{
		maxStat += maxStat * (percentageIncrease / 100f);
		currentStat = Mathf.Min(currentStat, maxStat);
		UpdateGraphics();
	}

	//==============================================================
	// Death Sequence
	//==============================================================
	private void OnPlayerDied()
	{
		if (playerAnimator != null)
		{
			// Make sure the animation can play even when time is paused
			playerAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
			playerAnimator.SetTrigger("Death");
		}

		if (gameOverMenu != null)
		{
			gameOverMenu.SetActive(true);
			Time.timeScale = 0f; // Pause the game
		}
		else if (PopupText.Instance != null)
		{
			PopupText.Instance.Popup("You have died!", 1f, 1f);
		}
	}
}
