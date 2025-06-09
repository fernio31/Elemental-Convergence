using UnityEngine;

// Asegura que el GameObject tenga siempre un Rigidbody2D.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] // [SerializeField] permite ver y editar la variable privada en el Inspector
    private float moveSpeed = 5f; // Velocidad base del jugador. Ajusta este valor en el Inspector.

    private Rigidbody2D rb;       // Referencia al componente Rigidbody2D del jugador.
    private Vector2 moveInput;    // Vector para almacenar la entrada (ejes horizontal y vertical).

    // Awake se llama antes que Start, ideal para obtener referencias.
    void Awake()
    {
        
        rb = GetComponent<Rigidbody2D>();

        
        if (rb != null)
        {
           
            rb.gravityScale = 0f;

            
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            
        }
        else
        {
            Debug.LogError("¡Rigidbody2D no encontrado en el jugador! El script PlayerMovement lo necesita.", this);
        }
    }

    // Update se llama una vez por frame. Ideal para leer Inputs.
    void Update()
    {
        
        float moveX = Input.GetAxisRaw("Horizontal"); 
        float moveY = Input.GetAxisRaw("Vertical");   

        // Almacenamos la entrada en un Vector2.
        moveInput = new Vector2(moveX, moveY);

        
        if (moveInput.magnitude > 1)
        {
            moveInput.Normalize();
        }

        
    }

    // FixedUpdate se llama a intervalos fijos de tiempo.
    void FixedUpdate()
    {
        // --- Aplicar el Movimiento Físico ---
        if (rb != null)
        {
            
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }
}