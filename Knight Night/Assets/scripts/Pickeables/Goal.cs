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