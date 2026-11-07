
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;


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
    //[SerializeField] private Animator animator;
    [SerializeField] FloorDetect floorDetect;

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
    private float attackTimer;
   //public  SpriteRenderer sr;



    private GameObject shot;
    


    void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        //sr = GetComponent<SpriteRenderer>();
        invincibilityTimer = 0;

    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
        Movevement();
        
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
            
            //animator.SetTrigger("walk");
            


        }
        else if (Input.GetKeyUp(keyLeft))
        {
            
            rb.linearVelocity = Vector3.zero;
            
        }
        else
        {
            //animator.SetTrigger("Idle");
        }
        if (Input.GetKey(keyRight))
        {
            
            //animator.SetTrigger("walk");
            Quaternion flip = Quaternion.Euler(0, 180, 0);
            transform.rotation = flip;
            rb.AddForce(Vector3.right * speed * Time.deltaTime, ForceMode2D.Impulse);
            //SpeedLimit();
        }
        else if (Input.GetKeyUp(keyRight))
        {
           
            rb.linearVelocity = Vector3.zero;
            
        }
        else
        {
            //animator.SetTrigger("Idle");
        }
        if (Input.GetKeyDown(attack) && attackTimer <= 0)
        {
            Debug.Log("tried attack");
            Attack();
            //AudioSource.clip = SfxAttack;
            //AudioSource.Play();
            //animator.SetTrigger("attack");
        }
        
           
    }
    



    private void OnTriggerEnter2D(Collider2D other) // cuando toca un enemigo, insertar danio
    {
        if (FloorDetect.CheckLayerInMask(enemyMask, other.gameObject.layer))
        {
            if (invincible)
            {
                
            }
            else 
            {
                playerLifebar.fillAmount -= 0.25f;

                //animator.SetTrigger("hit");
            }
            
        }

    }
    private void Attack()
    {
        attackTimer = attackCooldown;

        //AudioSource.clip = SfxAttack;
        //AudioSource.Play();
        //animator.SetTrigger("Attack");

        
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



}

