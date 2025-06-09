using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))] 
[RequireComponent(typeof(EnemyHealth))] 
public class EnemyAI : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 2.5f; 

    [Header("Ataque (Melee por Contacto)")]
    public int attackDamage = 1;         
    public float attackCooldown = 1.0f; 

    // --- Referencias Internas ---
    private Rigidbody2D rb;
    private Transform playerTarget;    
    private EnemyHealth enemyHealth;   
    private float lastAttackTime = -1f; 
    private bool canMove = true;       

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<EnemyHealth>();

        // Configuración Rigidbody 
        if (rb.gravityScale != 0)
        {
            rb.gravityScale = 0;
            Debug.LogWarning($"Ajustando Gravity Scale a 0 para {gameObject.name}", this);
        }
        
         Collider2D col = GetComponent<Collider2D>();
         if (col.isTrigger) {
             Debug.LogWarning($"El collider de {gameObject.name} es Trigger, pero EnemyAI usa colisiones físicas. Considera desactivar 'Is Trigger' o usar OnTrigger... para el ataque.", this);
         }

    }

    void Start()
    {
        // Buscar al jugador al iniciar.
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
            Debug.Log($"{gameObject.name} encontró al jugador: {playerObject.name}", this);
        }
        else
        {
            Debug.LogError($"EnemyAI en {gameObject.name} no pudo encontrar al jugador. ¡Asegúrate de que el jugador tenga la etiqueta 'Player'!", this);
        }
    }

    void FixedUpdate()
    {
        // Solo moverse si está vivo, puede moverse y tiene un objetivo
        if (enemyHealth != null && enemyHealth.IsAlive() && canMove && playerTarget != null)
        {
            MoveTowardsPlayer();
        }
        else
        {
            
            rb.linearVelocity = Vector2.zero;
        }
    }

    void MoveTowardsPlayer()
    {
        
        Vector2 direction = (playerTarget.position - transform.position).normalized;

       
        rb.linearVelocity = direction * moveSpeed;

        
    }


   
    private void OnCollisionStay2D(Collision2D collision)
    {
        
        if (enemyHealth != null && !enemyHealth.IsAlive()) return;

        // Comprobar si hemos chocado con el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            
             if(Time.time >= lastAttackTime + attackCooldown)
             {
                 AttemptAttack(collision.gameObject);
                 lastAttackTime = Time.time; 
             }
        }
    }

    
    void AttemptAttack(GameObject targetPlayer)
    {
        // Buscar el componente de vida del jugador
        PlayerHealth playerHealth = targetPlayer.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            Debug.Log($"{gameObject.name} atacando a {targetPlayer.name} por {attackDamage} daño.");
            playerHealth.TakeDamage(attackDamage); // Llamar al método para quitarle vida
            
        }
         else {
              Debug.LogWarning($"El objeto {targetPlayer.name} tiene tag 'Player' pero no el script PlayerHealth.", targetPlayer);
         }
    }

    
     public void StopMovement()
     {
         canMove = false;
         rb.linearVelocity = Vector2.zero; // Detener inmediatamente
     }

     

}