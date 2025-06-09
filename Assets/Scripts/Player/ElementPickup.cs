using UnityEngine;

public class ElementPickup : MonoBehaviour
{
    [Header("Configuración del Elemento")]
    public ElementType elementType = ElementType.None;
    public KeyCode pickupKey = KeyCode.E;
    
    
    public GameObject pickupPromptUI; 

    private bool playerIsClose = false;
    private PlayerAttackController playerToGiveElement;

    void Start()
    {
        
        if(pickupPromptUI != null)
        {
            pickupPromptUI.SetActive(false);
        }
    }

    void Update()
    {
        // Si el jugador está cerca y pulsa la tecla de recoger
        if (playerIsClose && Input.GetKeyDown(pickupKey))
        {
            if (playerToGiveElement != null)
            {
                Debug.Log($"Intentando recoger {elementType} pulsando {pickupKey}");
                bool pickedUp = playerToGiveElement.TryPickupElement(elementType);

                if (pickedUp)
                {
                    if (pickupPromptUI != null) pickupPromptUI.SetActive(false);
                    Destroy(gameObject); // Destruir el objeto item
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Jugador está cerca del item {elementType}.");
            playerIsClose = true;
            // Guardar la referencia al controlador de ataque del jugador
            playerToGiveElement = other.GetComponent<PlayerAttackController>();
            // Mostrar aviso para recoger
            if (pickupPromptUI != null) pickupPromptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Jugador se ha alejado del item {elementType}.");
            playerIsClose = false;
            playerToGiveElement = null; // Limpiar la referencia
            // Ocultar aviso para recoger
            if (pickupPromptUI != null) pickupPromptUI.SetActive(false);
        }
    }
}