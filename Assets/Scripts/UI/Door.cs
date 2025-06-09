using UnityEngine;

public class Door : MonoBehaviour
{
    private Collider2D col;
    private SpriteRenderer sr;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetBlocked(bool blocked)
    {
        if (col != null)
            col.enabled = blocked;

        if (sr != null)
            sr.color = blocked ? Color.red : Color.green; // Cambiar color para ver si está bloqueada
    }
}
