
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Specialized;
using UnityEngine.SceneManagement;
using TMPro;

public class Playermovement : MonoBehaviour
{
    #region Fields
    [Header("Parameters")]
    [SerializeField] float speed = 1.0f;
    public float jumpForce = 10;
    [SerializeField] Image playerLifebar;
    public bool powerUp = false;
    public bool invincible = false;
    private float invincibilityTimer;

    [Header("Movement")]

    [SerializeField] private KeyCode keyLeft = KeyCode.A;
    [SerializeField] private KeyCode keyRight = KeyCode.D;
    [SerializeField] private KeyCode jump = KeyCode.W;
    [SerializeField] private int maxJumps = 2; 
    private int jumpsLeft;
    [SerializeField] private Animator animator;
    [SerializeField] FloorDetect floorDetect;
    
    [Header("Health")]
    [SerializeField] private int maxHealth = 4;
    private int currentHealth;

    
    [Header("Objects")]
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private LayerMask playerMask;


    [Header("Audio")]
    [SerializeField] AudioSource AudioSource;
    [SerializeField] AudioClip SfxJump;
    [SerializeField] AudioClip SfxAttack;

    [Header("Attack")]
    [SerializeField] private KeyCode attack = KeyCode.R;
    [SerializeField] private Transform attackPoint;    
    [SerializeField] private float attackRange = 0.5f; 
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.4f; 
    
    [Header("Damage Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float hitInvulnerabilityTime = 1f; //
    [SerializeField] private float blinkInterval = 0.1f;
    private bool isHurt = false;
    
    
    [Header("Coins")]
    [SerializeField] private TMP_Text coinsText;
    private int coins = 0;

    private Coroutine invulnerabilityCoroutine;
    private Coroutine CloudCoroutine;
    
    private float attackTimer;
   

    private Rigidbody2D rb;

    public bool flip = false;


    private GameObject shot;
    #endregion
    #region Unity Methods
    void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        UpdateLifebar();
        invincibilityTimer = 0;

    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
        Movevement();
        UpdateAnimations();
        
    }
    #endregion
    #region Movement
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

            AudioSource.clip = SfxJump;
            AudioSource.Play();
        }
       

        if (Input.GetKey(keyLeft))
        {

            rb.AddForce(Vector3.left * speed * Time.deltaTime, ForceMode2D.Impulse);
            Quaternion flip = Quaternion.Euler(0, 0, 0);
            transform.rotation = flip;
            

        }
        else if (Input.GetKeyUp(keyLeft))
            rb.linearVelocity = Vector3.zero;
            
        
       
        if (Input.GetKey(keyRight))
        {
            //animator.SetTrigger("walk");
            Quaternion flip = Quaternion.Euler(0, 180, 0);
            transform.rotation = flip;
            rb.AddForce(Vector3.right * speed * Time.deltaTime, ForceMode2D.Impulse);
            //SpeedLimit();
        }
        else if (Input.GetKeyUp(keyRight))
            rb.linearVelocity = Vector3.zero;
            
        
       
        if (Input.GetKeyDown(attack) && attackTimer <= 0)
        {
            Debug.Log("tried attack");
            Attack();
            //AudioSource.clip = SfxAttack;
            //AudioSource.Play();
            animator.SetTrigger("Attack");
        }
        
           
    }
    
  
    private void UpdateAnimations()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x)); 
        animator.SetBool("IsGrounded", floorDetect.IsGrounded);
    }
    #endregion
    #region Attack
    private void Attack()
    {
        attackTimer = attackCooldown;

        
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyMask);

        foreach (Collider2D hit in hits)
        {
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
        return true;
    }

    private void UpdateLifebar()
    {
        playerLifebar.fillAmount = (float)currentHealth / maxHealth;
    }

    private void Die()
    {
        Debug.Log("player died");
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
        spriteRenderer.enabled = false; // later: death animation / particles / Game Over screen
        enabled = false;                // turns off this script (Update stops = no more input)
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (FloorDetect.CheckLayerInMask(enemyMask, other.gameObject.layer))
        {
            TakeDamage(1);
        }
    }
    #endregion
    #region Pickups
    public void AddDamage(int amount)
    {
        attackDamage += amount;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        coinsText.text = "Coins: " + coins;
    }

    public void ActivateInvulnerability(float duration)
    {
        
        if (invulnerabilityCoroutine != null)
        {
            StopCoroutine(invulnerabilityCoroutine);
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
        if (CloudCoroutine != null)
        {
            StopCoroutine(CloudCoroutine);
        }
        CloudCoroutine = StartCoroutine(CloudRoutine(duration));
    }

    private IEnumerator CloudRoutine(float duration)
    {
        maxJumps = 3;
        jumpsLeft++; 

        yield return new WaitForSeconds(duration);

        maxJumps = 2;
        jumpsLeft = Mathf.Min(jumpsLeft, maxJumps); 
    }
    
    #endregion
}

