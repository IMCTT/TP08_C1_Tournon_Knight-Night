using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3.0f;

    [Header("References")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private GameObject healthBar;      
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioSource audioSource;

    [Header("Feedback")]
    [SerializeField] private GameObject deathParticlesPrefab;
    [SerializeField] private AudioClip sfxHit;
    [SerializeField] private AudioClip sfxDeath;
    [SerializeField] private float shrinkDuration = 0.3f;
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private AnimationClip deathClip;
    

    private float currentHealth;
    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            PlaySound(sfxHit);
            StartCoroutine(HitFlash());
        }
    }

    private void UpdateHealthBar()
    {
        healthBarFill.fillAmount = currentHealth / maxHealth;
    }

    private IEnumerator HitFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private IEnumerator Die()
    {
        isDead = true;
        PlaySound(sfxDeath);
        healthBar.SetActive(false);

        
        if (deathParticlesPrefab != null)
        {
            Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
        }

        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;
        
        foreach (Collider2D enemyCollider in GetComponentsInChildren<Collider2D>())
        {
            enemyCollider.enabled = false;
        }
        
        

        
        
        enemyAnimator.SetTrigger("Die");
        yield return new WaitForSeconds(deathClip.length);

        spriteRenderer.enabled = false;
        float soundLength =  sfxDeath.length;

        Destroy(gameObject, soundLength);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}