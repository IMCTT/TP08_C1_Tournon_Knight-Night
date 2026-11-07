using UnityEngine;

// ScriptableObject that holds ALL of the knight's settings.
// Create one from: right click in Project > Create > Knight Night > Player Config.
// This lets us tweak values from the Inspector without touching code (required by the assignment).
[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Knight Night/Player Config")]
public class PlayerConfigSO : ScriptableObject
{
    [Header("Movement")]
    [Tooltip("Max horizontal speed while walking.")]
    public float maxSpeed = 7f;

    [Tooltip("How fast the knight reaches max speed.")]
    public float acceleration = 70f;

    [Tooltip("How fast the knight stops when you release the key.")]
    public float deceleration = 90f;

    [Tooltip("How much control you have in the air (1 = same as on the ground, 0 = none).")]
    [Range(0f, 1f)] public float airControl = 0.75f;

    [Header("Jump")]
    [Tooltip("Normal gravity scale of the Rigidbody2D.")]
    public float gravityScale = 3f;

    [Tooltip("Strength of the first jump (from the ground).")]
    public float jumpForce = 14f;

    [Tooltip("Strength of the air jumps (double and triple jump).")]
    public float airJumpForce = 12f;

    [Tooltip("Number of jumps without power-up. 2 = double jump.")]
    [Min(1)] public int maxJumps = 2;

    [Tooltip("Extra time to jump after walking off a platform (coyote time).")]
    public float coyoteTime = 0.1f;

    [Tooltip("If you press jump slightly before landing, it still jumps.")]
    public float jumpBufferTime = 0.12f;

    [Tooltip("When you release the jump key, upward speed is multiplied by this. Short press = short jump.")]
    [Range(0f, 1f)] public float jumpCutMultiplier = 0.5f;

    [Tooltip("Extra gravity while falling so the jump doesn't feel floaty.")]
    public float fallGravityMultiplier = 1.6f;

    [Tooltip("Max falling speed.")]
    public float maxFallSpeed = 22f;

    [Header("Health")]
    [Min(1)] public int maxHealth = 5;

    [Tooltip("Seconds without taking damage after being hit.")]
    public float invulnerabilityAfterHit = 1f;

    [Tooltip("Knockback when hit. X is applied away from the enemy.")]
    public Vector2 knockbackForce = new Vector2(6f, 6f);

    [Tooltip("Time the player has no movement control after the knockback.")]
    public float knockbackDuration = 0.2f;
}
