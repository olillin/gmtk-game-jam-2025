using UnityEngine;

public class Anchor : MonoBehaviour
{
    public float maxAge = 5.0f;
    public float minDistance = 0.5f;

    private bool attached = false;
    public bool IsAttached
    {
        get => attached;
    }

    public bool IsPulling
    {
        get => pullingPlayer != null;
    }

    private PlayerController pullingPlayer = null;

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

    public void StartPullingPlayer(PlayerController player)
    {
        float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
        pullingPlayer = player;
    }

    public void ConnectToRope(Rope rope)
    {
        rope.bottom.connectedBelow = gameObject;
        HingeJoint2D hinge = GetComponent<HingeJoint2D>();
        RopeSegment segment = rope.bottom;
        hinge.connectedBody = segment.GetComponent<Rigidbody2D>();
        hinge.connectedAnchor = new Vector2(0, -rope.SegmentLength);

        hinge.enabled = true;
    }
}
