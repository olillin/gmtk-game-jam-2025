using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rope : MonoBehaviour
{
    /// <summary>
    /// How many percent over the length of a segment a segment is allowed to
    /// stretch before adding a new one.
    /// </summary>
    public int maxSegmentStretch = 10;
    public int maxSegments = 50;
    public bool autoExtend = true;

    [SerializeField]
    private float segmentLength = 0.6f;
    public float SegmentLength
    {
        get => segmentLength;
    }

    [Space]
    public Rigidbody2D hook;

    [Space]
    public RopeSegment top;
    public RopeSegment bottom;
    public GameObject segmentPrefab;
    public int segmentCount = 10;

    private bool wasStretched = false;

    private LineRenderer lineRenderer;

    private float manualTimer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        GenerateRope();
    }

    void Update()
    {
        manualTimer += Time.deltaTime;

        if (manualTimer > 0.1)
        {
            if (Input.GetKey(KeyCode.W))
                ExtendRope();
            else if (Input.GetKey(KeyCode.S))
                ShortenRope();
            manualTimer = 0;
        }
    }

    void LateUpdate()
    {
        // bool isStretched = IsStretched();
        // if (autoExtend && isStretched && wasStretched)
        //     ExtendRope();
        // wasStretched = isStretched && !wasStretched;

        UpdateLine();
    }

    void GenerateRope()
    {
        RopeSegment prevSegment = RopeHelper.CreateSegment(this);
        prevSegment.ConnectAbove(hook);
        top = prevSegment;

        // for (int i = 0; i < segmentCount; i++)
        // {
        RopeSegment newSegment = prevSegment.AppendBelow() as RopeSegment;

        prevSegment = newSegment;
        // }
        newSegment = prevSegment.AppendBelow() as RopeSegment;
        bottom = newSegment;
    }

    /** Add a new segment to the rope */
    public void ExtendRope()
    {
        if (segmentCount >= maxSegments)
            return;

        RopeSegment newSegment = top.AppendAbove() as RopeSegment;
        top = newSegment;

        segmentCount++;
    }

    /** Remove a segment from the rope */
    public void ShortenRope()
    {
        if (segmentCount <= 1)
            return;

        RopeSegment newTop = top.ConnectedBelow.GetComponent<RopeSegment>();
        GameObject oldTop = top.gameObject;
        top = newTop;
        Destroy(oldTop.gameObject);
    }

    public void CancelRopeVelocity()
    {
        RopeSegment segment = top;
        while (segment != null)
        {
            GameObject next = segment.ConnectedBelow.gameObject;
            if (next == null)
                break;
            segment.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            segment = next.GetComponent<RopeSegment>();
        }
    }

    float AverageSegmentDistance()
    {
        float sum = 0;
        int count = 0;
        foreach (float distance in GetSegmentDistances())
        {
            sum += distance;
            count++;
        }
        return sum / count;
    }

    bool IsStretched()
    {
        float maxStretchDistance = (1 + maxSegmentStretch / 100.0f) * segmentLength;

        return GetSegmentDistances().Any(distance => distance > maxStretchDistance);
    }

    public IEnumerable<float> GetSegmentDistances()
    {
        IRopeSegment prevSegment = null;
        foreach (IRopeSegment segment in GetSegments())
        {
            if (prevSegment != null)
            {
                float distance = Vector2.Distance(
                    segment.transform.position,
                    prevSegment.transform.position
                );
                yield return distance;
            }
            prevSegment = segment;
        }
    }

    public void AttachTop(Rigidbody2D body)
    {
        top.ConnectAbove(body);
    }

    void UpdateLine()
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (IRopeSegment segment in GetSegments())
        {
            positions.Add(segment.transform.position);
        }

        lineRenderer.SetPositions(positions.ToArray());
        lineRenderer.positionCount = positions.Count;
    }

    public IEnumerable<IRopeSegment> GetSegments()
    {
        IRopeSegment segment = top;
        HashSet<IRopeSegment> visited = new HashSet<IRopeSegment>();
        int maxIterations = 100;
        int iterations = 0;
        while (segment != null && !visited.Contains(segment) && maxIterations < iterations)
        {
            iterations++;
            visited.Add(segment);
            yield return segment;
            GameObject next = segment.ConnectedBelow?.gameObject;
            if (next == null)
                break;
            segment = next.GetComponent<RopeSegment>();
        }
    }
}
