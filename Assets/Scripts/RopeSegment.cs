using UnityEngine;

public class RopeSegment : MonoBehaviour
{
    private HingeJoint2D hinge;

    public GameObject connectedAbove,
        connectedBelow;

    public Rope rope;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hinge = GetComponent<HingeJoint2D>();
        connectedAbove = hinge.connectedBody.gameObject;
        RopeSegment aboveSegment = connectedAbove.GetComponent<RopeSegment>();
        if (aboveSegment != null)
        {
            aboveSegment.connectedBelow = gameObject;
            hinge.connectedAnchor = new Vector2(0, -rope.SegmentLength);
        }
        else
        {
            hinge.connectedAnchor = new Vector2(0, 0);
        }
    }

    void OnDestroy()
    {
        var aboveSegment = connectedAbove.GetComponent<RopeSegment>();
        if (aboveSegment != null)
            aboveSegment.connectedBelow = connectedBelow;

        var belowSegment = connectedBelow.GetComponent<RopeSegment>();
        if (belowSegment != null)
        {
            belowSegment.connectedAbove = connectedAbove;
            belowSegment.hinge.connectedBody = hinge.connectedBody;
        }

        rope.segmentCount--;
    }
}
