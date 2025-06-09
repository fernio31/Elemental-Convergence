
using UnityEngine;

public class Spell : MonoBehaviour
{
    public ElementType element;
    public int damage = 10;
    public float lifeTime = 3f;
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        Destroy(gameObject, lifeTime);
        SetElementVisual();
    }

    void SetElementVisual()
    {
        switch (element)
        {
            case ElementType.Fire:
                spriteRenderer.color = Color.red;
                break;
            case ElementType.Water:
                spriteRenderer.color = Color.cyan;
                break;
            case ElementType.Earth:
                spriteRenderer.color = new Color(0.5f, 0.25f, 0f);
                break;
            case ElementType.Air:
                spriteRenderer.color = Color.white;
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<EnemyHealth>(out var enemy))
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
