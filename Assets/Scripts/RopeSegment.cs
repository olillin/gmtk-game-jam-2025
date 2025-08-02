using System;
using System.Collections.Generic;
using UnityEngine;

public class RopeSegment : MonoBehaviour
{
    private Rigidbody2D connectedAbove = null;
    public Rigidbody2D ConnectedAbove
    {
        get => connectedAbove;
    }
    private Rigidbody2D connectedBelow = null;
    public Rigidbody2D ConnectedBelow
    {
        get => connectedBelow;
    }

    public Rope rope { get; set; }

    public bool canContract = false;

    void Start()
    {
        var hinge = GetComponent<HingeJoint2D>();
        if (hinge != null)
            hinge.connectedAnchor = new Vector2(0, -rope.SegmentLength);
        ConnectAbove(connectedAbove?.GetComponent<Rigidbody2D>());
        ConnectBelow(connectedBelow?.GetComponent<Rigidbody2D>());
    }

    void Update()
    {
        if (canContract && rope.contract)
        {
            float distance = Vector2.Distance(
                ConnectedAbove.transform.position,
                transform.position
            );
            if (distance <= rope.squishDistance)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnDestroy()
    {
        rope.RemoveSegment(this);
    }

    public RopeSegment AppendAbove(RopeSegment segment) => RopeHelper.AppendAbove(this, segment);

    public RopeSegment AppendAbove() => RopeHelper.AppendAbove(this);

    public RopeSegment AppendBelow(RopeSegment segment) => RopeHelper.AppendBelow(this, segment);

    public RopeSegment AppendBelow() => RopeHelper.AppendBelow(this);

    public void ConnectAbove(Rigidbody2D body)
    {
        GetComponent<HingeJoint2D>().connectedBody = body;
        connectedAbove = body;
    }

    public void ConnectBelow(Rigidbody2D body)
    {
        connectedBelow = body;
    }

    public IEnumerable<RopeSegment> GetSegmentsBelow()
    {
        RopeSegment segment = this;
        HashSet<RopeSegment> visited = new HashSet<RopeSegment>();
        while (segment != null && !visited.Contains(segment))
        {
            visited.Add(segment);
            yield return segment;
            GameObject next = segment.ConnectedBelow?.gameObject;
            if (next == null)
                break;
            segment = next.GetComponent<RopeSegment>();
        }
    }

    public IEnumerable<RopeSegment> GetSegmentsBelowUntil(Predicate<RopeSegment> predicate)
    {
        foreach (RopeSegment segment in GetSegmentsBelow())
        {
            if (predicate(segment))
                break;
            else
                yield return segment;
        }
    }

    public IEnumerable<RopeSegment> GetSegmentsAbove()
    {
        RopeSegment segment = this;
        HashSet<RopeSegment> visited = new HashSet<RopeSegment>();
        while (segment != null && !visited.Contains(segment))
        {
            visited.Add(segment);
            yield return segment;
            GameObject next = segment.ConnectedAbove?.gameObject;
            if (next == null)
                break;
            segment = next.GetComponent<RopeSegment>();
        }
    }

    public IEnumerable<RopeSegment> GetSegmentsAboveUntil(Predicate<RopeSegment> predicate)
    {
        foreach (RopeSegment segment in GetSegmentsAbove())
        {
            if (predicate(segment))
                break;
            else
                yield return segment;
        }
    }
}
