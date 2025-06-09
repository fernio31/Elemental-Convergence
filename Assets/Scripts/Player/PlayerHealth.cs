using UnityEngine;
using UnityEngine.UI; // Para interactuar con UI (barras de vida, etc.)
// using UnityEngine.SceneManagement; // Para reiniciar nivel si muere

public class PlayerHealth : MonoBehaviour
{
    [Header("Salud")]
    public int maxHealth = 10; // Vida máxima
    [SerializeField] // Para verla en inspector pero no hacerla pública sin querer
    private int currentHealth; // Vida actual

    [Header("Invencibilidad Post-Daño")]
    public float invincibilityDuration = 1.0f; // Segundos de invencibilidad tras recibir daño
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

    [Header("Feedback Visual (Opcional)")]
    public SpriteRenderer playerSpriteRenderer; // Arrastra aquí el SpriteRenderer principal del jugador
    public Color damageFlashColor = Color.red; // Color para el parpadeo al recibir daño
    public float flashDuration = 0.1f; // Duración de cada parpadeo
    private Color originalColor;

    [Header("UI (Opcional)")]
     public Slider healthSlider; // Arrastra aquí tu Slider de UI para la vida
     public Text healthText; // Arrastra aquí tu Text de UI para la vida (ej. "10/10")

    // --- Inicialización ---
    void Awake()
    {
        currentHealth = maxHealth;
        if(playerSpriteRenderer == null) // Intentar encontrarlo si no está asignado
        {
            playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        if(playerSpriteRenderer != null)
        {
            originalColor = playerSpriteRenderer.color;
        }
         UpdateHealthUI(); // Actualizar UI al inicio
    }

    // --- Lógica de Daño ---
    public void TakeDamage(int damageAmount)
    {
        // Si es invencible, no hacer nada
        if (isInvincible)
        {
            return;
        }

        // Reducir vida
        currentHealth -= damageAmount;
        // Asegurarse de que la vida no baje de 0
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Jugador recibió {damageAmount} de daño. Vida restante: {currentHealth}/{maxHealth}");

         UpdateHealthUI(); // Actualizar UI

        // Comprobar si ha muerto
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Si no ha muerto, activar invencibilidad y feedback
            StartInvincibility();
            StartCoroutine(FlashEffect()); // Iniciar corutina de parpadeo
        }
    }

    void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
         // Opcional: Cambiar el Layer del jugador a uno que no colisione con enemigos
         // originalLayer = gameObject.layer;
         // gameObject.layer = LayerMask.NameToLayer("InvinciblePlayer"); // Necesitas crear esta layer
    }

     void Update()
    {
         // Gestionar temporizador de invencibilidad
         if (isInvincible)
         {
             invincibilityTimer -= Time.deltaTime;
             if (invincibilityTimer <= 0)
             {
                 isInvincible = false;
                 // Opcional: Restaurar layer original
                 // gameObject.layer = originalLayer;
                 // Asegurarse de que el color vuelve al original al final
                 if(playerSpriteRenderer != null) playerSpriteRenderer.color = originalColor;
             }
         }
    }

     // Corutina para el efecto de parpadeo
    System.Collections.IEnumerator FlashEffect()
    {
         float flashEndTime = Time.time + invincibilityDuration - flashDuration; // Parar el flash un poco antes de que acabe la invencibilidad
         while (isInvincible && Time.time < flashEndTime)
         {
             if (playerSpriteRenderer != null) playerSpriteRenderer.color = damageFlashColor;
             yield return new WaitForSeconds(flashDuration);
             if (playerSpriteRenderer != null) playerSpriteRenderer.color = originalColor;
             yield return new WaitForSeconds(flashDuration);
         }
         // Asegurarse de que el color es el original al final
         if(playerSpriteRenderer != null) playerSpriteRenderer.color = originalColor;
    }


    void Die()
    {
        Debug.Log("¡Jugador ha Muerto!");
        isInvincible = false; // Asegurarse de que no sea invencible al morir

        // Desactivar componentes de control del jugador (movimiento, ataque)
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerAttackController>().enabled = false;
        this.enabled = false; // Desactivar este script también

        // Aquí puedes:
        // 1. Iniciar una animación de muerte.
        // 2. Mostrar pantalla de "Game Over".
        // 3. Esperar unos segundos y reiniciar el nivel.
        // Ejemplo: Reiniciar la escena actual después de 3 segundos
        // Invoke(nameof(RestartLevel), 3f);

        // Por ahora, simplemente podríamos desactivar el objeto jugador
         gameObject.SetActive(false); // O Destroy(gameObject);
    }

    // void RestartLevel()
    // {
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    // }

     // --- Actualización UI ---
    void UpdateHealthUI()
    {
         if (healthSlider != null)
         {
             healthSlider.maxValue = maxHealth;
             healthSlider.value = currentHealth;
         }
         if (healthText != null)
         {
             healthText.text = $"{currentHealth} / {maxHealth}";
         }
    }

     // Opcional: Método para curar al jugador
    public void Heal(int amount)
    {
         currentHealth += amount;
         currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
         UpdateHealthUI();
         Debug.Log($"Jugador curado {amount}. Vida actual: {currentHealth}/{maxHealth}");
    }

} // Fin de la clase PlayerHealth