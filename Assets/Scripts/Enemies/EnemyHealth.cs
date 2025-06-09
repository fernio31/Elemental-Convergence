using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    private RoomController parentRoom; // Referencia a la sala que lo contiene
    private bool isAlive = true; // Añadir flag
    private EnemyAI enemyAI; // Añadir referencia a la IA

    void Start()
    {
        currentHealth = maxHealth;
        enemyAI = GetComponent<EnemyAI>(); // Obtener la referencia a la IA
        currentHealth = maxHealth;
        isAlive = true;
    }

    // Método para que RoomController establezca la referencia
    public void SetRoomController(RoomController room)
    {
        parentRoom = room;
    }

    public void TakeDamage(int amount)
    {
        if (!isAlive) return; // No hacer daño si ya está muerto

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
         if (!isAlive) return; // Evitar morir múltiples veces

        isAlive = false; // Marcar como muerto
        Debug.Log($"{gameObject.name} died.");

        // Detener la IA
        if(enemyAI != null)
        {
            enemyAI.StopMovement();
        }

        // Notificar a la sala ANTES de destruirse (si es necesario)
        if (parentRoom != null)
        {
            parentRoom.EnemyDefeated(this.gameObject);
        }

        // Destruir el objeto enemigo (o iniciar animación de muerte)
        // Considera un pequeño retraso si tienes animación de muerte
        Destroy(gameObject, 0.1f); // Pequeño retraso para asegurar que todo se procesa
    }
    
    public bool IsAlive()
    {
        return isAlive;
    }

    // Ejemplo simple para probar el daño (puedes quitar esto después)
    void OnMouseDown()
    {
         TakeDamage(1);
    }
}