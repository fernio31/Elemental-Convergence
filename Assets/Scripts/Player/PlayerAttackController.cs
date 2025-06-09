using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerAttackController : MonoBehaviour
{
    [Header("Elementos")]
    
    private List<ElementType> currentElements = new List<ElementType>(2);
    public int maxElements = 2;

    [Header("UI Elementos")]
    public Image elementIcon1; // Arrastra aquí la Imagen UI del primer slot
    public Image elementIcon2; // Arrastra aquí la Imagen UI del segundo slot
    public Sprite noneSprite;  // Sprite para cuando no hay elemento
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite earthSprite;
    public Sprite airSprite;

    [Header("Controles de Ataque")]
    [SerializeField] private KeyCode attack1Key = KeyCode.Mouse0; // Botón izquierdo del ratón
    [SerializeField] private KeyCode attack2Key = KeyCode.Mouse1; // Botón derecho del ratón
    [SerializeField] private KeyCode synergyAttackKey = KeyCode.Q;  // Tecla Q para sinergia (ejemplo)
    [SerializeField] private KeyCode discardElementKey = KeyCode.R; // Tecla R para descartar el último elemento (opcional)


    [Header("Prefabs de Ataque")]
    public GameObject fireAttackPrefab;
    public GameObject waterAttackPrefab;
    public GameObject earthAttackPrefab;
    public GameObject airAttackPrefab;
    // --- Prefabs de Sinergia ---
    public GameObject mudAttackPrefab; // Water + Earth
    public GameObject steamAttackPrefab; // Fire + Water (Ejemplo)
    public GameObject lavaAttackPrefab; // Fire + Earth (Ejemplo)
    public GameObject stormAttackPrefab; // Water + Air (Ejemplo)
    // Añade más según tus combinaciones...

    [Header("Configuración de Ataque")]
    public Transform attackSpawnPoint; // Punto desde donde nacen los ataques
    public float attackCooldown = 0.5f; // Tiempo mínimo entre ataques
    private float lastAttackTime = -1f; // Para controlar el cooldown


    // --- Inicialización ---
    void Start()
    {
        // Empezar sin elementos
        InitializeElements();
        UpdateElementUI(); // Actualizar UI si la tienes
    }

    void InitializeElements()
    {
         currentElements.Clear();
         
    }

   
    void Update()
    {
       
        bool canAttack = Time.time >= lastAttackTime + attackCooldown;

        // Ataque Primario 1 (Primer elemento equipado)
        if (Input.GetKeyDown(attack1Key) && canAttack)
        {
            PerformAttack(0); // Usa el elemento en el índice 0
        }

        // Ataque Primario 2 (Segundo elemento equipado)
        if (Input.GetKeyDown(attack2Key) && canAttack)
        {
            PerformAttack(1); // Usa el elemento en el índice 1
        }

        // Ataque de Sinergia
        if (Input.GetKeyDown(synergyAttackKey) && canAttack)
        {
            PerformSynergyAttack(); // Intenta usar la sinergia de los dos elementos
        }

         // Opcional: Descartar último elemento
        if (Input.GetKeyDown(discardElementKey))
        {
            DiscardLastElement();
        }
    }

    
    public bool TryPickupElement(ElementType newElement)
    {
        if (newElement == ElementType.None) return false; // No recoger "nada"

        // Si ya tenemos el elemento, no hacemos nada (o podrías recargar munición si tuvieras)
        if (currentElements.Contains(newElement))
        {
             Debug.Log($"Ya tienes el elemento {newElement}.");
             return false; // O true si quieres que el item desaparezca igual
        }


        // Si hay espacio, simplemente añadir
        if (currentElements.Count < maxElements)
        {
            currentElements.Add(newElement);
            Debug.Log($"Añadido {newElement}. Elementos actuales: {string.Join(", ", currentElements)}");
            UpdateElementUI(); // Actualizar UI
            return true;
        }
        // Si no hay espacio, reemplazar el MÁS ANTIGUO (el primero de la lista)
        else
        {
            ElementType removedElement = currentElements[0];
            currentElements.RemoveAt(0); // Quita el primero
            currentElements.Add(newElement); // Añade el nuevo al final
            Debug.Log($"Inventario lleno. Reemplazado {removedElement} por {newElement}. Elementos actuales: {string.Join(", ", currentElements)}");
            UpdateElementUI(); // Actualizar UI
            return true;
        }
    }

     // --- Lógica para Descartar (Opcional) ---
    void DiscardLastElement()
    {
        if (currentElements.Count > 0)
        {
            ElementType discarded = currentElements[currentElements.Count - 1]; // El último añadido
            currentElements.RemoveAt(currentElements.Count - 1);
            Debug.Log($"Descartado {discarded}. Elementos actuales: {string.Join(", ", currentElements)}");
            UpdateElementUI();
            // Podrías instanciar el item descartado en el suelo aquí
        }
    }


    // --- Lógica de Ataque ---
    void PerformAttack(int elementIndex)
    {
        // Verificar si existe un elemento en ese índice
        if (elementIndex >= 0 && elementIndex < currentElements.Count)
        {
            ElementType elementToUse = currentElements[elementIndex];
            if (elementToUse != ElementType.None)
            {
                ExecutePrimaryAttack(elementToUse);
                lastAttackTime = Time.time; // Reiniciar cooldown
            }
            else
            {
                 Debug.Log($"Slot de ataque {elementIndex + 1} está vacío.");
                 // Podrías tener un ataque básico por defecto aquí
            }
        }
         else
        {
             Debug.Log($"No hay elemento en el slot {elementIndex + 1}.");
             // Podrías tener un ataque básico por defecto aquí
        }
    }

    void PerformSynergyAttack()
    {
        // Solo posible si tenemos exactamente 2 elementos
        if (currentElements.Count == maxElements)
        {
            // Intentar ejecutar la sinergia
            bool synergyUsed = ExecuteSynergyAttack(currentElements[0], currentElements[1]);
            if (synergyUsed)
            {
                lastAttackTime = Time.time; // Reiniciar cooldown si la sinergia se usó
            }
            else
            {
                 Debug.Log("No hay sinergia definida para esta combinación.");
            }
        }
        else
        {
            Debug.Log("Necesitas 2 elementos para un ataque de sinergia.");
        }
    }

    // Ejecuta el ataque primario de UN elemento
    void ExecutePrimaryAttack(ElementType element)
    {
        GameObject prefabToSpawn = null;
        switch (element)
        {
            case ElementType.Fire:
                prefabToSpawn = fireAttackPrefab;
                Debug.Log("Ataque de Fuego!");
                break;
            case ElementType.Water:
                prefabToSpawn = waterAttackPrefab;
                Debug.Log("Ataque de Agua!");
                break;
            case ElementType.Earth:
                prefabToSpawn = earthAttackPrefab;
                Debug.Log("Ataque de Tierra!");
                break;
            case ElementType.Air:
                prefabToSpawn = airAttackPrefab;
                Debug.Log("Ataque de Aire!");
                break;
        }

        if (prefabToSpawn != null && attackSpawnPoint != null)
        {
            // Instanciar el prefab del ataque en el punto de spawn
            // La rotación debe calcularse para apuntar (ej. al ratón)
            Instantiate(prefabToSpawn, attackSpawnPoint.position, CalculateAttackRotation());
        }
        else if(attackSpawnPoint == null)
        {
             Debug.LogError("AttackSpawnPoint no asignado en el PlayerAttackController!");
        }
         else
        {
             Debug.LogError($"Prefab de ataque para {element} no asignado!");
        }
    }

    
    bool ExecuteSynergyAttack(ElementType el1, ElementType el2)
    {
        GameObject prefabToSpawn = null;
        string synergyName = "Ninguna";

        

        if ((el1 == ElementType.Water && el2 == ElementType.Earth) || (el1 == ElementType.Earth && el2 == ElementType.Water))
        {
            prefabToSpawn = mudAttackPrefab;
            synergyName = "Lodo";
        }
        else if ((el1 == ElementType.Fire && el2 == ElementType.Water) || (el1 == ElementType.Water && el2 == ElementType.Fire))
        {
            prefabToSpawn = steamAttackPrefab;
            synergyName = "Vapor";
        }
        else if ((el1 == ElementType.Fire && el2 == ElementType.Earth) || (el1 == ElementType.Earth && el2 == ElementType.Fire))
        {
             prefabToSpawn = lavaAttackPrefab;
             synergyName = "Lava";
        }
        else if ((el1 == ElementType.Water && el2 == ElementType.Air) || (el1 == ElementType.Air && el2 == ElementType.Water))
        {
             prefabToSpawn = stormAttackPrefab; // O 'Ice'? Depende de tu diseño
             synergyName = "Tormenta/Hielo";
        }
        


        // --- Ejecución ---
        if (prefabToSpawn != null && attackSpawnPoint != null)
        {
            Debug.Log($"¡Ataque de Sinergia: {synergyName}!");
            Instantiate(prefabToSpawn, attackSpawnPoint.position, CalculateAttackRotation());
            return true; // Se usó una sinergia
        }
         else if (prefabToSpawn != null && attackSpawnPoint == null)
        {
             Debug.LogError("AttackSpawnPoint no asignado en el PlayerAttackController!");
             return false;
        }
        else
        {
            
             return false;
        }
    }

    
    Quaternion CalculateAttackRotation()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(mousePos.x - attackSpawnPoint.position.x, mousePos.y - attackSpawnPoint.position.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, angle); // Rotación en el eje Z para 2D
    }

    // --- Interfaz de Usuario  ---
    void UpdateElementUI()
    {
        // Actualizar primer icono
        if (elementIcon1 != null)
        {
            if (currentElements.Count > 0)
            {
                elementIcon1.sprite = GetSpriteForElement(currentElements[0]); // Mostrar el primer elemento
                elementIcon1.enabled = true; 
            }
            else // No hay elementos
            {
                elementIcon1.sprite = noneSprite; 
                elementIcon1.enabled = (noneSprite != null); 
            }
        }

        // Actualizar segundo icono
        if (elementIcon2 != null)
        {
            if (currentElements.Count > 1) 
            {
                elementIcon2.sprite = GetSpriteForElement(currentElements[1]); // Mostrar el segundo elemento
                elementIcon2.enabled = true;
            }
            else // Hay cero o un elemento
            {
                elementIcon2.sprite = noneSprite; 
                elementIcon2.enabled = (noneSprite != null);
            }
        }
         
    }

    
    Sprite GetSpriteForElement(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire:  return fireSprite;
            case ElementType.Water: return waterSprite;
            case ElementType.Earth: return earthSprite;
            case ElementType.Air:   return airSprite;
            case ElementType.None:
            default:                return noneSprite;
        }
    }

    
    void OnDrawGizmosSelected()
    {
        if (attackSpawnPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackSpawnPoint.position, 0.3f);
        }
    }
}