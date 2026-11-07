
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Playermovement : MonoBehaviour
{
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
    [SerializeField] private Animator animator;
    [SerializeField] FloorDetect floorDetect;
    
    [Header("Health")]
    [SerializeField] private int maxHealth = 4;
    private int currentHealth;

    private Rigidbody2D rb;

    public bool flip = false;

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
    
    private float attackTimer;
   



    private GameObject shot;
    


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
        
        if (invincible)
        {
            invincibilityTimer += Time.deltaTime;
        }
        if (invincibilityTimer > 10)
        {
            invincible = false;
            invincibilityTimer = 0;
        }
    }


    private void Movevement()
    {
        if (Input.GetKeyDown(jump) && floorDetect.IsGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            Debug.Log("jump");

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
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (FloorDetect.CheckLayerInMask(enemyMask, other.gameObject.layer))
        {
            TakeDamage(1);
        }
    }
    
    
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

    // cambiar por animacion
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void UpdateAnimations()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x)); 
        animator.SetBool("IsGrounded", floorDetect.IsGrounded);
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

    private IEnumerator RestartAfterDelay()
    {
        spriteRenderer.enabled = false; // later: death animation / particles / Game Over screen
        enabled = false;                // turns off this script (Update stops = no more input)
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}

