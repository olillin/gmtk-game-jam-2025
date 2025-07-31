using Unity.VisualScripting;
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
    private SpringJoint2D joint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        joint = GetComponent<SpringJoint2D>();
    }

    void FixedUpdate()
    {
        age += Time.fixedDeltaTime;
        if (!IsAttached && age > maxAge)
        {
            Destroy(gameObject);
        }

        if (pullingPlayer != null)
        {
            joint.distance = Mathf.Max(
                joint.distance - Time.fixedDeltaTime * pullingPlayer.pullStrength,
                minDistance
            );
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
        joint.enabled = true;
        joint.connectedBody = player.Rigidbody;
        joint.distance = distanceToPlayer;
        pullingPlayer = player;
    }
}
