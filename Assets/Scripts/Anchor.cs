using UnityEngine;

public class Anchor : MonoBehaviour
{
    private bool attached = false;
    public bool IsAttached
    {
        get => attached;
    }

    private float age = 0;

    [SerializeField]
    private float maxAge = 5.0f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        age += Time.fixedDeltaTime;
        if (!IsAttached && age > maxAge)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        rb.constraints |= RigidbodyConstraints2D.FreezePosition;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;
        attached = true;
    }
}
