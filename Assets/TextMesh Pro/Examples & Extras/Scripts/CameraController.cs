using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Configuración de Transición")]
    [SerializeField] private float smoothSpeed = 0.125f; // Velocidad de la transición (más bajo = más lento). Ajusta en Inspector.
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); // Distancia Z de la cámara al plano 2D. Ajusta si es necesario.

    private Vector3 targetPosition;     // Posición del centro de la sala actual a la que la cámara quiere ir.
    private bool isMoving = false;      // Indicador para saber si la cámara está en medio de una transición.

    // Opcional: Si prefieres SmoothDamp para una transición diferente (más amortiguada)
    // public float smoothTime = 0.3f;
    // private Vector3 velocity = Vector3.zero; // Necesario para SmoothDamp

    void Start()
    {
        // Es buena idea inicializar la targetPosition a la posición inicial
        // para evitar un salto al empezar. El MapGenerator puede llamar a SnapToPosition.
        targetPosition = transform.position - offset; // Asume que empieza ya centrada o será corregida.
    }

    // LateUpdate se ejecuta después de todas las funciones Update.
    // Es el lugar ideal para el movimiento de cámaras, asegurando que el jugador ya se ha movido.
    void LateUpdate()
    {
        // Solo necesitamos actualizar si la cámara debe moverse a un nuevo objetivo
        // o si todavía no ha llegado al objetivo actual.
        if (isMoving || transform.position != targetPosition + offset)
        {
            // Posición final deseada (centro de la sala + offset en Z)
            Vector3 desiredPosition = targetPosition + offset;

            // --- Opción 1: Interpolación Lineal (Lerp) ---
            // Simple y efectivo para muchas transiciones.
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            // --- Opción 2: SmoothDamp (Alternativa, descomentar si se prefiere) ---
            // Proporciona una deceleración suave al acercarse al objetivo.
            // transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

            // Comprobar si la cámara ha llegado (o está muy cerca) a su destino
            // Usamos una pequeña tolerancia para evitar problemas con precisión flotante.
            if (Vector3.Distance(transform.position, desiredPosition) < 0.01f)
            {
                transform.position = desiredPosition; // Clavar la posición final exacta.
                isMoving = false;                     // Marcar que ya no está en movimiento activo.
            }
            else
            {
                 isMoving = true; // Sigue en movimiento si no ha llegado
            }
        }
    }

    /// <summary>
    /// Método público que será llamado por el RoomController cuando el jugador entre.
    /// </summary>
    /// <param name="newRoomCenter">El centro (transform.position) de la sala a la que se ha entrado.</param>
    public void MoveToNewRoom(Vector3 newRoomCenter)
    {
        // Calculamos la nueva posición objetivo (solo X e Y del centro de la sala).
        Vector3 newTarget = new Vector3(newRoomCenter.x, newRoomCenter.y, 0); // Ignoramos Z aquí, se usa el offset

        // Solo actualizamos el objetivo si es realmente una sala diferente
        // (evita reiniciar la transición si el jugador roza el trigger de nuevo)
        if (Vector3.Distance(targetPosition, newTarget) > 0.1f) // Umbral pequeño
        {
            targetPosition = newTarget;
            isMoving = true; // Activa el movimiento en LateUpdate.
             // Si usas SmoothDamp, no necesitas resetear 'velocity'.
             // velocity = Vector3.zero; // Podría ser necesario si quieres un inicio más brusco
        }
    }

    /// <summary>
    /// Método para colocar la cámara instantáneamente en una posición.
    /// Útil para la configuración inicial por parte del MapGenerator.
    /// </summary>
    /// <param name="roomCenter">Centro de la sala inicial.</param>
    public void SnapToPosition(Vector3 roomCenter)
    {
        targetPosition = new Vector3(roomCenter.x, roomCenter.y, 0);
        transform.position = targetPosition + offset; // Colocación inmediata
        isMoving = false;
        // Si usas SmoothDamp:
        // velocity = Vector3.zero;
    }
}