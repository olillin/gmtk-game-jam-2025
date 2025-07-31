using UnityEngine;

public class Rope : MonoBehaviour
{
    public Rigidbody2D hook;
    public GameObject segmentPrefab;
    public int segmentCount = 10;

    void Start()
    {
        GenerateRope();
    }

    void GenerateRope()
    {
        Rigidbody2D prevBody = hook;
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject newSegment = Instantiate(segmentPrefab);
            newSegment.transform.parent = transform;
            newSegment.transform.position = prevBody.position + Vector2.down * 0.5f;
            newSegment.transform.name += $" {i}";
            HingeJoint2D hinge = newSegment.GetComponent<HingeJoint2D>();
            hinge.connectedBody = prevBody;

            prevBody = newSegment.GetComponent<Rigidbody2D>();
        }
    }

    /** Add a new segment to the rope */
    void ExtendRope() { }

    /** Remove a segment from the rope */
    void ShortenRope() { }
}
