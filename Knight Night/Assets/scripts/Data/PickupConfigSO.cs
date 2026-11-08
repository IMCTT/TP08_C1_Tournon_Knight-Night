using UnityEngine;

[CreateAssetMenu(fileName = "PickupConfig", menuName = "Knight Night/Pickup Config")]
public class PickupConfigSO : ScriptableObject
{
    public PickupType type;
    public int amount = 1;
    public float duration = 5f;
}