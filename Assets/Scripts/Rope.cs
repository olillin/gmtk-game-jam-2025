using UnityEngine;

public class Rope : MonoBehaviour
{
    /// <summary>
    /// How many percent over the length of a segment a segment is allowed to
    /// stretch before adding a new one.
    /// </summary>
    public int maxSegmentStretch = 10;
    public int maxSegments = 50;

    public Rigidbody2D hook;

    [Space]
    public RopeSegment top;
    public RopeSegment bottom;
    public GameObject segmentPrefab;
    public int segmentCount = 10;

    private float segmentLength;
    public float SegmentLength
    {
        get => segmentLength;
    }

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

        if (IsStretched())
        {
            ExtendRope();
        }
    }

    void Start()
    {
        var collider = segmentPrefab.GetComponent<BoxCollider2D>();
        segmentLength = collider.size.y;

        GenerateRope();
    }

    void GenerateRope()
    {
        Rigidbody2D prevBody = hook;
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject newBody = Instantiate(segmentPrefab);
            newBody.transform.parent = transform;
            newBody.transform.position = transform.position;
            newBody.transform.name += $" {i}";
            RopeSegment newSegment = newBody.GetComponent<RopeSegment>();
            newSegment.rope = this;
            newSegment.GetComponent<HingeJoint2D>().connectedBody = prevBody;

            prevBody = newBody.GetComponent<Rigidbody2D>();
            bottom = newSegment;

            if (i == 0)
            {
                top = newSegment;
            }
        }
    }

    /** Add a new segment to the rope */
    void ExtendRope()
    {
        if (segmentCount >= maxSegments)
            return;

        // Create new segment
        GameObject newBody = Instantiate(segmentPrefab);
        newBody.transform.parent = transform;
        newBody.transform.position = transform.position;
        RopeSegment newSegment = newBody.GetComponent<RopeSegment>();
        newSegment.GetComponent<HingeJoint2D>().connectedBody = hook;
        newSegment.connectedBelow = top.gameObject;
        newSegment.rope = this;

        // Move previous top down
        float bottom = newBody.GetComponent<BoxCollider2D>().bounds.size.y;
        top.connectedAbove = newBody;
        HingeJoint2D topHinge = top.GetComponent<HingeJoint2D>();
        topHinge.connectedBody = newBody.GetComponent<Rigidbody2D>();
        topHinge.connectedAnchor = new Vector2(0, -bottom);

        // Assign new top segment
        top = newBody.GetComponent<RopeSegment>();

        segmentCount++;
    }

    /** Remove a segment from the rope */
    void ShortenRope()
    {
        if (top.connectedBelow == null)
            return;

        RopeSegment newTop = top.connectedBelow.GetComponent<RopeSegment>();
        newTop.connectedAbove = hook.gameObject;
        var newTopHinge = newTop.GetComponent<HingeJoint2D>();
        newTopHinge.connectedBody = hook;
        newTopHinge.connectedAnchor = new Vector2(0, 0);

        // Replace top
        GameObject oldTop = top.gameObject;
        top = newTop;
        Destroy(oldTop.gameObject);

        segmentCount--;
    }

    float AverageSegmentDistance()
    {
        float sum = 0;
        int count = 0;
        RopeSegment segment = top;
        while (segment != null)
        {
            GameObject next = segment.connectedBelow;
            if (next == null)
                break;
            sum += Vector2.Distance(segment.transform.position, next.transform.position);
            count++;
            segment = next.GetComponent<RopeSegment>();
        }
        return sum / count;
    }

    bool IsStretched()
    {
        float maxStretchDistance = (1 + maxSegmentStretch / 100.0f) * segmentLength;

        RopeSegment segment = top;
        while (segment != null)
        {
            GameObject next = segment.connectedBelow;
            if (next == null)
                break;
            float distance = Vector2.Distance(segment.transform.position, next.transform.position);
            if (distance > maxStretchDistance)
                return true;
            segment = next.GetComponent<RopeSegment>();
        }
        return false;
    }
}
