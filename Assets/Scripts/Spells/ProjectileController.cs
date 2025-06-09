using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ProjectileController : MonoBehaviour
{
    [Header("Configuración")]
    public float speed = 10f;        // Velocidad del proyectil
    public float lifetime = 3.0f;    // Segundos antes de autodestruirse
    public int damage = 1;           // Daño que inflige
    // Opcional: Podrías añadir una variable ElementType aquí si el tipo de daño importa
    // public ElementType projectileElement;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        
        rb.linearVelocity = transform.right * speed;

        
        Destroy(gameObject, lifetime);
    }

   

    // USA ESTE MÉTODO SI TU COLLIDER TIENE "Is Trigger" MARCADO
    void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other.gameObject);
    }

    

    
    void HandleCollision(GameObject hitObject)
    {
         
        if (hitObject.CompareTag("Player")) 
        {
              return; 
        }

        // Intentar dañar a un enemigo
        if (hitObject.CompareTag("Enemy")) 
        {
            
            EnemyHealth enemyHealth = hitObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                Debug.Log($"Proyectil golpeó a {hitObject.name}, infligiendo {damage} daño.");
                enemyHealth.TakeDamage(damage);
            }
             
            Destroy(gameObject);
             return; 
        }

        
        if (hitObject.CompareTag("Wall") || hitObject.layer == LayerMask.NameToLayer("Obstacles")) // Ajusta Layer name
        {
             Debug.Log("Proyectil chocó con obstáculo.");
             Destroy(gameObject);
             return;
        }

         
         if (hitObject.CompareTag("Projectile")) // Asume que los proyectiles tienen tag "Projectile"
         {
             // return; // No hacer nada
         }
    }
}