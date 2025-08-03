using UnityEngine;

public class Anchor : MonoBehaviour
{
    private bool attached = false;
    public bool IsAttached
    {
        get => attached;
    }

    private float age = 0;
    public float Age
    {
        get => age;
    }

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

    public void Attach()
    {
        rb.constraints |= RigidbodyConstraints2D.FreezePosition;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;
        attached = true;
    }

    public void Attach(Transform snapTo)
    {
        Attach();
        transform.position = snapTo.position;
    }
}
