using System.Collections;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private GameObject needKeyMessage;
    private bool opened = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Playermovement player = other.GetComponentInParent<Playermovement>();
        if (player == null || opened) 
            return;

        if (player.HasKey)
        {
            opened = true;
            player.Win();
        }
        else
        {
            // Bug: Cada entrada al trigger arranca una corrutina nueva. Si el jugador entra dos veces en menos de 2 segundos, la primera apaga el mensaje
            // mientras la segunda sigue corriendo. Guardá la Coroutine en un campo y frená la anterior antes de arrancar otra. El 2f también conviene serializarlo.
            StartCoroutine(ShowNeedKey());
        }
    }

    private IEnumerator ShowNeedKey()
    {
        needKeyMessage.SetActive(true);
        
        yield return new WaitForSeconds(2f);
        
        needKeyMessage.SetActive(false);
    }
}