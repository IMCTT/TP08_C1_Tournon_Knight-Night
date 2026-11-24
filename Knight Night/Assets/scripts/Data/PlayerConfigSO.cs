using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Knight Night/Player Config")]
public class PlayerConfigSO : ScriptableObject
{
    [Header("Movement")]
    // Suggestion: Los campos public del SO se pueden mutar en runtime desde cualquier clase y el cambio persiste en el .asset en el editor.
    // Usá [SerializeField] private y properties de solo lectura.
    public float speed = 5f;
    public float jumpForce = 10f;
    public int maxJumps = 2;

    [Header("Health")]
    public int maxHealth = 4;
    public float hitInvulnerabilityTime = 1f;

    [Header("Attack")]
    public int attackDamage = 1;
    public float attackRange = 0.5f;
    public float attackCooldown = 0.4f;
}