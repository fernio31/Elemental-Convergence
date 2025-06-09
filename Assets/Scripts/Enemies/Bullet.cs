using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }
}
