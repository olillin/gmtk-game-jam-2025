using UnityEngine;

public class RopeSegment : MonoBehaviour, IRopeSegment
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

    void Start()
    {
        var hinge = GetComponent<HingeJoint2D>();
        if (hinge != null)
            hinge.connectedAnchor = new Vector2(0, -rope.SegmentLength);
        ConnectAbove(connectedAbove?.GetComponent<Rigidbody2D>());
        ConnectBelow(connectedBelow?.GetComponent<Rigidbody2D>());
    }

    void OnDestroy()
    {
        RopeHelper.RemoveFromRope(this);
    }

    public IRopeSegment AppendAbove(IRopeSegment segment) => RopeHelper.AppendAbove(this, segment);

    public IRopeSegment AppendAbove() => RopeHelper.AppendAbove(this);

    public IRopeSegment AppendBelow(IRopeSegment segment) => RopeHelper.AppendBelow(this, segment);

    public IRopeSegment AppendBelow() => RopeHelper.AppendBelow(this);

    public void ConnectAbove(Rigidbody2D body)
    {
        GetComponent<HingeJoint2D>().connectedBody = body;
        Debug.Log("Connect above" + connectedAbove + body);
        // if (connectedAbove == body?.GetComponent<Rigidbody2D>())
        //     return;
        connectedAbove = body;
        RopeSegment segmentAbove = body?.GetComponent<RopeSegment>();
        if (segmentAbove != null)
            segmentAbove.ConnectBelow(GetComponent<Rigidbody2D>());
    }

    public void ConnectBelow(Rigidbody2D body)
    {
        connectedBelow = body;
        // RopeSegment segmentBelow = body?.GetComponent<RopeSegment>();
        // if (segmentBelow != null)
        //     segmentBelow.ConnectAbove(GetComponent<Rigidbody2D>());
    }
}
