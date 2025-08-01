using UnityEngine;

public class Anchor : MonoBehaviour
{
    public float maxAge = 5.0f;

    private bool attached = false;
    public bool IsAttached
    {
        get => attached;
    }

    private float age = 0;

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

    void OnDestroy()
    {
        PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        player?.RemoveAnchor(this);
    }

    public void Attach()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        attached = true;
    }

    public void Attach(Transform snapTo)
    {
        Attach();
        transform.position = snapTo.position;
    }
}
