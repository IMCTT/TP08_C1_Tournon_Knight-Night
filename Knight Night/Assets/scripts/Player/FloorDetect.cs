// Suggestion: System.Collections y System.Collections.Generic no se usan. Borrá los using.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorDetect : MonoBehaviour
{
    // Warning: [SerializeField] public es redundante y deja el campo mutable desde cualquier clase. Usá [SerializeField] private LayerMask groundMask;
    [SerializeField] public LayerMask groundMask;
    [SerializeField] Vector2 boxSize = new Vector2(0.4f, 0.1f);

    public bool IsGrounded { get; private set; }
 
    void FixedUpdate()
    {
        IsGrounded = Physics2D.OverlapBox(transform.position, boxSize, 0f, groundMask);
    }
    // Suggestion: CheckLayerInMask es una utilidad genérica que Playermovement usa para detectar enemigos. Movela a una clase estática de utilidades.
    public static bool CheckLayerInMask(LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}