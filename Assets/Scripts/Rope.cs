using UnityEngine;

public class Rope : MonoBehaviour
{
    public Rigidbody2D hook;
    public RopeSegment top;
    public GameObject segmentPrefab;
    public int segmentCount = 10;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            ExtendRope();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ShortenRope();
        }
    }

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
            newSegment.transform.position = transform.position;
            newSegment.transform.name += $" {i}";
            HingeJoint2D hinge = newSegment.GetComponent<HingeJoint2D>();
            hinge.connectedBody = prevBody;

            prevBody = newSegment.GetComponent<Rigidbody2D>();

            if (i == 0)
            {
                top = newSegment.GetComponent<RopeSegment>();
            }
        }
    }

    /** Add a new segment to the rope */
    void ExtendRope()
    {
        // Create new segment
        GameObject newSegment = Instantiate(segmentPrefab);
        newSegment.transform.parent = transform;
        newSegment.transform.position = transform.position;
        HingeJoint2D hinge = newSegment.GetComponent<HingeJoint2D>();
        hinge.connectedBody = hook;
        newSegment.GetComponent<RopeSegment>().connectedBelow = top.gameObject;

        // Move previous top down
        float bottom = newSegment.GetComponent<BoxCollider2D>().bounds.size.y;
        top.connectedAbove = newSegment;
        top.Hinge.connectedBody = newSegment.GetComponent<Rigidbody2D>();
        top.Hinge.connectedAnchor = new Vector2(0, -bottom);

        // Assign new top segment
        top = newSegment.GetComponent<RopeSegment>();
    }

    /** Remove a segment from the rope */
    void ShortenRope()
    {
        if (top.connectedBelow == null)
            return;

        RopeSegment newTop = top.connectedBelow.GetComponent<RopeSegment>();
        newTop.connectedAbove = hook.gameObject;
        newTop.Hinge.connectedBody = hook;
        newTop.Hinge.connectedAnchor = new Vector2(0, 0);

        // Replace top
        GameObject oldTop = top.gameObject;
        top = newTop;
        Destroy(oldTop.gameObject);
    }
}
