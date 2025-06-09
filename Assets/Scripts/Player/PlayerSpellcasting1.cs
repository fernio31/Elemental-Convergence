
using UnityEngine;

public class PlayerSpellcasting : MonoBehaviour
{
    public GameObject[] spellPrefabs; 

    public Transform castPoint;
    public ElementType selectedElement = ElementType.Fire;

    public float fireRate = 0.3f;
    private float nextFireTime = 0f;


    

    void Update()
    {
        HandleElementSwitch();

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && Time.time >= nextFireTime)
        {
            CastSpell();
            nextFireTime = Time.time + fireRate;
        }
    }

    void HandleElementSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            selectedElement = ElementType.Fire;

        if (Input.GetKeyDown(KeyCode.Alpha2))
            selectedElement = ElementType.Water;

        if (Input.GetKeyDown(KeyCode.Alpha3))
            selectedElement = ElementType.Earth;

        if (Input.GetKeyDown(KeyCode.Alpha4))
            selectedElement = ElementType.Air;
    }

    void CastSpell()
    {   
         Debug.Log("Intentando disparar: " + selectedElement);
        int index = (int)selectedElement;
        if (index < 0 || index >= spellPrefabs.Length)
        {
            Debug.LogError("Índice fuera de rango: " + index + " — spellPrefabs.Length = " + spellPrefabs.Length);
            return;
        }

        if (spellPrefabs[index] == null)
        {
            Debug.LogError("Prefab del hechizo en la posición " + index + " está vacío.");
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - castPoint.position).normalized;

        GameObject spell = Instantiate(spellPrefabs[index], castPoint.position, Quaternion.identity);
        spell.GetComponent<Rigidbody2D>().linearVelocity = direction * 10f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        spell.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
