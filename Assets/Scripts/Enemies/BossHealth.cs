using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Salud del Jefe")]
    public int maxHealth = 500;
    public int currentHealth;

    [Header("Fases")]
    public float phase2ThresholdPercentage = 0.50f; // Cambia de fase al 50%
    public enum BossPhase { Phase1, Phase2, Defeated }
    public BossPhase currentPhase = BossPhase.Phase1;

    private BossAI_VoidWeaver bossAI; // Referencia al script de IA del jefe
    private RoomController parentRoom; // Si es notificado por la sala
    private bool isAlive = true;

    void Awake()
    {
        currentHealth = maxHealth;
        bossAI = GetComponent<BossAI_VoidWeaver>();

        // --- Comprobación de Seguridad ---
        if (bossAI == null)
        {
            Debug.LogError($"¡Error Crítico! El script 'BossAI_VoidWeaver' no se encontró en el GameObject '{gameObject.name}'. Asegúrate de que ambos scripts estén en el mismo objeto.", this);
        }
    }

    public void SetRoomController(RoomController room)
    {
        parentRoom = room;
        Debug.Log($"Referencia de la sala establecida para el jefe {gameObject.name}");
    }

    public void TakeDamage(int damageAmount)
    {
        if (!isAlive) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"{gameObject.name} recibió {damageAmount} daño. Vida: {currentHealth}/{maxHealth}");

        // Comprobar si hay que cambiar de fase
        if (currentPhase == BossPhase.Phase1)
        {
            float healthPercentage = (float)currentHealth / maxHealth;
            if (healthPercentage <= phase2ThresholdPercentage)
            {
                ChangePhase(BossPhase.Phase2);
            }
        }

        // Comprobar si ha muerto
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ChangePhase(BossPhase newPhase)
    {
        if (currentPhase == newPhase || !isAlive) return;

        currentPhase = newPhase;
        Debug.Log($"{gameObject.name} entrando en {newPhase}!");

        
    }

    void Die()
    {
        if (!isAlive) return;
        isAlive = false;
        currentPhase = BossPhase.Defeated;

        Debug.Log($"{gameObject.name} ha sido derrotado!");

        // --- Comprobación de Seguridad ---
        if (bossAI != null)
        {
            bossAI.OnDeath(); // Notificar a la IA para que detenga acciones
        }
        else
        {
            Debug.LogError($"El jefe murió, pero la referencia 'bossAI' es nula en {gameObject.name}.");
        }

        // --- Comprobación de Seguridad ---
        if (parentRoom != null)
        {
            parentRoom.EnemyDefeated(this.gameObject); // Notifica a la sala
        }
        else
        {
            Debug.LogWarning($"El jefe murió, pero no tenía una referencia a 'parentRoom'. La sala no será notificada.", this);
        }

        // Iniciar animación de muerte, efectos, etc.
        Destroy(gameObject, 5f); // Destruir después de 5s
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}