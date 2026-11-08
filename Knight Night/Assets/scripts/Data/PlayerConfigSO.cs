using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Knight Night/Player Config")]
public class PlayerConfigSO : ScriptableObject
{
    [Header("Movement")]
    public float speed = 1f;
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