using UnityEngine;

public class ElementalChest : MonoBehaviour
{
    [Header("Contenido")]
    public GameObject[] elementItemPrefabs; // Asigna los 4 prefabs de items elementales aquí

    [Header("Estado")]
    public Sprite openSprite; // Sprite opcional para el cofre abierto
    private bool isOpen = false;

    

    
     private void OnTriggerEnter2D(Collider2D other)
     {
         if (!isOpen && other.CompareTag("Player")) // Solo abrir si está cerrado y es el jugador
         {
             Open();
         }
     }


    void Open()
    {
        if (isOpen) return; // Ya está abierto
        isOpen = true;

        Debug.Log("Abriendo cofre elemental...");

        // 1. Soltar un elemento aleatorio
        if (elementItemPrefabs != null && elementItemPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, elementItemPrefabs.Length);
            GameObject itemToSpawn = elementItemPrefabs[randomIndex];

            if (itemToSpawn != null)
            {
                // Spawnear un poco por encima del cofre
                Instantiate(itemToSpawn, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogWarning("El cofre no tiene asignados los prefabs de items elementales.", this);
        }

        // 2. Cambiar apariencia / Desactivar
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && openSprite != null)
        {
            sr.sprite = openSprite; // Cambiar a sprite abierto
        }

        // Desactivar el collider para no poder abrirlo de nuevo
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
    }
}