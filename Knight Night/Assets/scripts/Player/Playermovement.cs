
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

// Suggestion: El nombre de la clase no respeta PascalCase (PlayerMovement) y la clase resuelve movimiento, vida, ataque, pickups y score.
// Conviene separarla en componentes (PlayerHealth, PlayerAttack, PlayerPickups) para que cada uno tenga una sola responsabilidad.
public class Playermovement : MonoBehaviour
{
    #region Fields
    
    [SerializeField] private PlayerConfigSO config;
    
    // Suggestion: Este [Header] queda sobre campos privados no serializados, no se ve en el inspector, y los valores por defecto se pisan en Awake con el SO.
    // Sacá los valores por defecto para que quede claro que la fuente de verdad es PlayerConfigSO.
    [Header("Parameters")]
    private float speed = 1.0f;
    private float jumpForce = 10;
    private int maxJumps = 2; 
    private int jumpsLeft;
    // Warning: invincible es public y cualquier clase puede mutarlo. Pasalo a private y exponé una property de solo lectura si otra clase lo necesita.
    public bool invincible = false;
    private int maxHealth = 4;
    private int currentHealth;
    private bool isHurt = false;
    private float hitInvulnerabilityTime = 1f; 
    private float attackRange = 0.5f; 
    private int attackDamage = 1;
    private float attackCooldown = 0.4f; 
    

    [Header("Movement")]

    [SerializeField] private KeyCode keyLeft = KeyCode.A;
    [SerializeField] private KeyCode keyRight = KeyCode.D;
    [SerializeField] private KeyCode jump = KeyCode.W;

    [SerializeField] private Animator animator;
    [SerializeField] FloorDetect floorDetect;
    
    
    [Header("Objects")]
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private LayerMask playerMask;


    [Header("Audio")]
    // Suggestion: Estos campos privados están en PascalCase (AudioSource, SfxJump, ...) y sin private explícito, distinto del resto del script (speed, animator).
    // Usá camelCase: [SerializeField] private AudioSource audioSource; [SerializeField] private AudioClip sfxJump;
    [SerializeField] AudioSource AudioSource;
    [SerializeField] AudioClip SfxJump;
    [SerializeField] AudioClip SfxAttack;
    [SerializeField] AudioClip SfxHurt;
    [SerializeField] AudioClip SfxDeath;
    [SerializeField] AudioClip SfxHeal;
    [SerializeField] AudioClip SfxCoin;
    [SerializeField] AudioClip SfxPowerUp;
    [SerializeField] AudioClip SfxKey;

    [Header("Attack")]
    [SerializeField] private KeyCode attack = KeyCode.R;
    [SerializeField] private Transform attackPoint;    
   
    
    [Header("Damage Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] Image playerLifebar;
    
    [SerializeField] private float blinkInterval = 0.1f;
    [SerializeField] private EndScreen endScreen;
    
    
    [Header("Coins")]
    [SerializeField] private TMP_Text coinsText;
    private int coins = 0;
    private int score = 0;
    
    [Header("Key")]
    [SerializeField] private GameObject key; 
    
    

    private Coroutine invulnerabilityCoroutine;
    private Coroutine CloudCoroutine;
    
    private float horizontalInput;
    private float attackTimer;
   

    private Rigidbody2D rb;

    // Suggestion: flip y shot (unas líneas más abajo) no se usan en ningún lado. Es código muerto, borralo.
    public bool flip = false;


    private GameObject shot;
    #endregion
    #region Unity Methods
    void Awake()
    {

        speed = config.speed;
        jumpForce = config.jumpForce;
        maxJumps = config.maxJumps;
        maxHealth = config.maxHealth;
        hitInvulnerabilityTime = config.hitInvulnerabilityTime;
        attackDamage = config.attackDamage;
        attackRange = config.attackRange;
        attackCooldown = config.attackCooldown;
        
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        UpdateLifebar();
        

    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
        Movevement();
        UpdateAnimations();
        
    }
    
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
    }
    #endregion
    #region Movement
    // Suggestion: Typo en el nombre (Movevement). Además del movimiento resuelve salto y ataque, un nombre como HandleInput describe mejor lo que hace.
    private void Movevement()
    {
        
        if (floorDetect.IsGrounded && rb.linearVelocity.y <= 0.01f)
        {
            jumpsLeft = maxJumps;
        }

        if (Input.GetKeyDown(jump) && jumpsLeft > 0)
        {
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpsLeft--;

            PlaySfx(SfxJump);
        }
       

        
        horizontalInput = 0f;

        if (Input.GetKey(keyLeft))
        {
            horizontalInput = -1f;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (Input.GetKey(keyRight))
        {
            horizontalInput = 1f;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
       
        if (Input.GetKeyDown(attack) && attackTimer <= 0)
        {
            Debug.Log("tried attack");
            Attack();
            
            animator.SetTrigger("Attack");
        }
        
           
    }
    
  
    private void UpdateAnimations()
    {
        // Warning: Strings mágicos "Speed", "IsGrounded" (y "Attack" en Movevement) pasados al Animator todos los frames.
        // Declaralos como private static readonly int SpeedHash = Animator.StringToHash("Speed"); y usá el hash.
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x)); 
        animator.SetBool("IsGrounded", floorDetect.IsGrounded);
    }
    #endregion
    #region Attack
    private void Attack()
    {
        attackTimer = attackCooldown;

        
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyMask);
        PlaySfx(SfxAttack);
        foreach (Collider2D hit in hits)
        {
            // Bug: Acá usás hit.GetComponent, pero en OnTriggerEnter2D usás GetComponentInParent. El prefab Lancer tiene el hijo DamageColl en la capa Enemy:
            // si el círculo de ataque toca solo ese hijo, GetComponent devuelve null y el golpe no hace daño. Usá hit.GetComponentInParent<EnemyHealth>().
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                Debug.Log("hit enemy");
            }
        }
    }

    
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    #endregion 
    #region Health
    public void TakeDamage(int amount)
    {
        if (invincible || isHurt || currentHealth <= 0) return;

        currentHealth -= amount;
        UpdateLifebar();
        PlaySfx(SfxHurt);

        if (currentHealth <= 0)
        {
            
            Die();
            
            
        }
        else
        {
            StartCoroutine(HurtFeedback());
        }
    }

    public bool Heal(int amount)
    {
        if (currentHealth >= maxHealth || currentHealth <= 0) return false;
        

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); 
        UpdateLifebar();
        PlaySfx(SfxHeal);
        return true;
    }

    private void UpdateLifebar()
    {
        playerLifebar.fillAmount = (float)currentHealth / maxHealth;
    }

    private void Die()
    {
        Debug.Log("player died");
        PlaySfx(SfxDeath);
        rb.linearVelocity = Vector2.zero;
        StartCoroutine(RestartAfterDelay());
    }
    
    private IEnumerator HurtFeedback()
    {
        isHurt = true;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white; 

        float timer = 0f;
        while (timer < hitInvulnerabilityTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        spriteRenderer.enabled = true;
        isHurt = false;
    }
    
   

    private IEnumerator RestartAfterDelay()
    {
        spriteRenderer.enabled = false; 
        enabled = false;
        yield return new WaitForSeconds(1f);
        endScreen.ShowGameOver(score);
    }
    public void Win()
    {
        rb.linearVelocity = Vector2.zero;
        enabled = false; 
        endScreen.ShowVictory(score);
    }
    
    // Bug: OnTriggerEnter2D se ejecuta solo al entrar. Si el jugador sigue dentro del trigger del enemigo cuando termina HurtFeedback (isHurt = false),
    // no vuelve a recibir daño hasta salir y volver a entrar. Usá OnTriggerStay2D (el chequeo de isHurt ya evita el daño repetido).
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (FloorDetect.CheckLayerInMask(enemyMask, other.gameObject.layer))
        {
            
            EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
            // Warning: enemy puede ser null si algo en la capa Enemy no tiene EnemyHealth en su jerarquía, y esta línea tira NullReferenceException. Agregá if (enemy == null) return;
            int damage = enemy.enemyDamage;
            TakeDamage(damage);
        }
    }
    #endregion
    #region Pickups
    public void AddDamage(int amount)
    {
        attackDamage += amount;
        PlaySfx(SfxPowerUp);
    }

    
    public void AddCoins(int amount)
    {
        SaveData.Coins += amount;  
        // Suggestion: El 10 es un número mágico (puntos por moneda). Llevalo a un campo del SO.
        score += amount * 10;       
        // Suggestion: coinsText solo se actualiza al juntar una moneda, al arrancar la escena muestra el texto por defecto del TMP y no el total guardado. Inicializalo en Awake.
        coinsText.text = "Coins: " + SaveData.Coins;
        PlaySfx(SfxCoin);
    }

    public void ActivateInvulnerability(float duration)
    {
        
        // Error: SfxPowerUp solo suena si invulnerabilityCoroutine != null, es decir desde el segundo pickup en adelante (la referencia nunca vuelve a null).
        // El primer pickup de invulnerabilidad no tiene sonido. Mové PlaySfx(SfxPowerUp) fuera del if.
        if (invulnerabilityCoroutine != null)
        {
            StopCoroutine(invulnerabilityCoroutine);
            PlaySfx(SfxPowerUp);
        }
        invulnerabilityCoroutine = StartCoroutine(InvulnerabilityRoutine(duration));
    }

    private IEnumerator InvulnerabilityRoutine(float duration)
    {
        invincible = true;
        spriteRenderer.color = Color.yellow; 
        yield return new WaitForSeconds(duration);
        invincible = false;
        spriteRenderer.color = Color.white;
        
    }

    public void ActivateCloud(float duration)
    {
        // Error: Mismo problema que en ActivateInvulnerability: el primer pickup de triple salto no suena porque PlaySfx está dentro del if. Sacalo del if.
        if (CloudCoroutine != null)
        {
            PlaySfx(SfxPowerUp);
            StopCoroutine(CloudCoroutine);
        }
        CloudCoroutine = StartCoroutine(CloudRoutine(duration));
    }

    private IEnumerator CloudRoutine(float duration)
    {
        // Suggestion: El 3 está hardcodeado. Leelo del PickupConfigSO (por ejemplo config.amount) o de PlayerConfigSO para que sea configurable.
        maxJumps = 3;
        jumpsLeft++; 

        yield return new WaitForSeconds(duration);

        maxJumps = config.maxJumps;
        jumpsLeft = Mathf.Min(jumpsLeft, maxJumps); 
    }
    
    public bool HasKey { get; private set; }

    public void AddKey()
    {
        HasKey = true;
        PlaySfx(SfxKey);
        key.SetActive(true);
    }
    
    private void PlaySfx(AudioClip clip)
    {
        if (clip != null) AudioSource.PlayOneShot(clip);
    }
    #endregion
}

