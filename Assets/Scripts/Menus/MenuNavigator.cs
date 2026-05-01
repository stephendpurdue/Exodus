using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Attach this to the Main Menu Canvas (or a persistent manager object).
/// Assign the three menu buttons in the Inspector in top-to-bottom order:
/// Play → Settings → Quit
/// </summary>
public class MenuNavigator : MonoBehaviour
{
    [Header("Menu Buttons (top to bottom)")]
    [SerializeField] private Button[] menuButtons;

    [Header("Visual Highlight")]
    [SerializeField] private Color highlightColor = new Color(1f, 0.8f, 0f, 1f); // gold tint
    [SerializeField] private Color normalColor   = Color.white;

    private int selectedIndex = 0;

    // Deadzone prevents stick drift triggering rapid navigation
    private const float StickDeadzone = 0.5f;

    // Cooldown prevents a single stick flick jumping multiple items
    private float navCooldown = 0f;
    private const float NavCooldownDuration = 0.2f;

    private void OnEnable()
    {
        // Always start with the first button selected
        selectedIndex = 0;
        UpdateHighlight();
    }

    private void Update()
    {
        navCooldown -= Time.deltaTime;

        HandleNavigation();
        HandleConfirm();
    }

    // ── Navigation ────────────────────────────────────────────────────────

    private void HandleNavigation()
    {
        if (navCooldown > 0f) return;

        // Read the left stick vertical axis (up = positive, down = negative)
        float stick = Input.GetAxisRaw("Vertical");

        // D-pad maps to Vertical axis too in Unity's default Input Manager,
        // so this covers both stick and D-pad automatically.

        if (stick > StickDeadzone)
        {
            Move(-1); // stick up = move selection up the list
        }
        else if (stick < -StickDeadzone)
        {
            Move(1);  // stick down = move selection down the list
        }
    }

    private void Move(int direction)
    {
        // Deselect current
        SetHighlight(selectedIndex, false);

        // Wrap around the button list
        selectedIndex = (selectedIndex + direction + menuButtons.Length) % menuButtons.Length;

        // Highlight new selection
        SetHighlight(selectedIndex, true);

        navCooldown = NavCooldownDuration;
    }

    // ── Confirm ───────────────────────────────────────────────────────────

    private void HandleConfirm()
    {
        // JoystickButton0 = A (Xbox) / Cross (PS)
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            menuButtons[selectedIndex].onClick.Invoke();
        }
    }

    // ── Highlight ─────────────────────────────────────────────────────────

    private void UpdateHighlight()
    {
        for (int i = 0; i < menuButtons.Length; i++)
            SetHighlight(i, i == selectedIndex);
    }

    private void SetHighlight(int index, bool highlighted)
    {
        if (index < 0 || index >= menuButtons.Length) return;

        // Tint the button's target graphic (the Image component on the button)
        var image = menuButtons[index].targetGraphic as Image;
        if (image != null)
            image.color = highlighted ? highlightColor : normalColor;

        // Also tell Unity's EventSystem which element is "selected"
        // so Unity's own navigation and accessibility stay in sync
        if (highlighted)
            EventSystem.current.SetSelectedGameObject(menuButtons[index].gameObject);
    }
}
