private void Update()
{
    // ── Movement input ────────────────────────────────────────────────
    moveInput = Vector2.zero;

    // Keyboard (existing)
    if (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetKey(KeyCode.UpArrow))
        moveInput.y += 1f;
    if (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetKey(KeyCode.DownArrow))
        moveInput.y -= 1f;
    if (UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetKey(KeyCode.RightArrow))
        moveInput.x += 1f;
    if (UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetKey(KeyCode.LeftArrow))
        moveInput.x -= 1f;

    // Controller (left stick) — adds on top of keyboard input
    float stickX = UnityEngine.Input.GetAxisRaw("Horizontal");
    float stickY = UnityEngine.Input.GetAxisRaw("Vertical");
    moveInput.x += stickX;
    moveInput.y += stickY;

    if (moveInput.sqrMagnitude > 1f)
        moveInput.Normalize();

    // ── Sprite flip ───────────────────────────────────────────────────
    if (moveInput.x > 0.01f)
        spriteRenderer.flipX = false;
    else if (moveInput.x < -0.01f)
        spriteRenderer.flipX = true;

    // ── Animator ──────────────────────────────────────────────────────
    if (animator != null)
        animator.SetBool("isMoving", moveInput.sqrMagnitude > 0.01f);

    // ── Attack cooldown tick ──────────────────────────────────────────
    attackTimer -= Time.deltaTime;

    // ── Attack input — Space OR controller A/Cross button ────────────
    bool attackPressed = Input.GetKeyDown(KeyCode.Space)
                      || Input.GetKeyDown(KeyCode.JoystickButton0);

    if (attackPressed && attackTimer <= 0f)
    {
        Attack();
        attackTimer = attackCooldown;
    }
}
