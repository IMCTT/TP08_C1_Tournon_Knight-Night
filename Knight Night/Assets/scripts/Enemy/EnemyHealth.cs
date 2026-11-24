using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyConfigSO config;
    private int maxHealth;

    // Suggestion: Property pública en camelCase. Por convención de C# debería ser EnemyDamage.
    public int enemyDamage => config.enemyDamage; 

    [Header("References")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private GameObject healthBar;      
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioSource audioSource;

    [Header("Feedback")]
    [SerializeField] private GameObject deathParticlesPrefab;
    [SerializeField] private AudioClip sfxHit;
    [SerializeField] private AudioClip sfxDeath;
    // Suggestion: shrinkDuration no se usa en ningún método. Es código muerto, borralo o implementá el efecto.
    [SerializeField] private float shrinkDuration = 0.3f;
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private AnimationClip deathClip;
    

    private float currentHealth;
    private bool isDead = false;

    void Awake()
    {
        maxHealth = config.maxHealth;
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
        
        

        
        
        // Warning: "Die" es un string mágico. Usá private static readonly int DieHash = Animator.StringToHash("Die");
        enemyAnimator.SetTrigger("Die");
        // Warning: deathClip.length (y sfxDeath.length más abajo) no tienen null check, mientras que PlaySound sí valida el clip.
        // Si falta alguno en el inspector, NullReferenceException y el enemigo nunca se destruye. Validá antes de usarlos.
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