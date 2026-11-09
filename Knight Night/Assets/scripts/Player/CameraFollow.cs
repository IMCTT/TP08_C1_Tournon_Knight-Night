using UnityEngine;

//script hecho con IA
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;                    
    [SerializeField] private Vector2 offset = new Vector2(0f, 1.5f); 
    [SerializeField] private float smoothTime = 0.15f;              

    private Vector3 velocity; 

    
    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}