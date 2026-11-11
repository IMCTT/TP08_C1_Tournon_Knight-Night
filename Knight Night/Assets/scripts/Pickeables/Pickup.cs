using UnityEngine;


public enum PickupType
{
    Heal, 
    Damage, 
    Invulnerability, 
    Coin,
    Cloud,
    Key
}

public class Pickup : MonoBehaviour
{
    [SerializeField] private PickupConfigSO config;
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        Playermovement player = other.GetComponentInParent<Playermovement>();
        if (player == null) return; 

        bool used = true;

        switch (config.type)
        {
            case PickupType.Heal:
                used = player.Heal(config.amount); 
                break;
            case PickupType.Damage:
                player.AddDamage(config.amount);
                break;
            case PickupType.Invulnerability:
                player.ActivateInvulnerability(config.duration);
                break;
            case PickupType.Coin:
                player.AddCoins(config.amount);
                break;
            case PickupType.Cloud:
                player.ActivateCloud(config.duration);
                break;
            case PickupType.Key:
                player.AddKey();
                break;
        }

        if (used)
        {
            Destroy(gameObject);
        }
    }
}