using UnityEngine;


public enum PickupType
{
    Heal, 
    Damage, 
    Invulnerability, 
    Coin,
    Cloud
}

public class Pickup : MonoBehaviour
{
    [SerializeField] private PickupType type;
    [SerializeField] private int amount = 1;   
    [SerializeField] private float duration = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        Playermovement player = other.GetComponentInParent<Playermovement>();
        if (player == null) return; 

        bool used = true;

        switch (type)
        {
            case PickupType.Heal:
                used = player.Heal(amount); 
                break;
            case PickupType.Damage:
                player.AddDamage(amount);
                break;
            case PickupType.Invulnerability:
                player.ActivateInvulnerability(duration);
                break;
            case PickupType.Coin:
                player.AddCoins(amount);
                break;
            case PickupType.Cloud:
                player.ActivateCloud(duration);
                break;
        }

        if (used)
        {
            Destroy(gameObject);
        }
    }
}