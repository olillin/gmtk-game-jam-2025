using UnityEngine;

public class RopeHelper
{
    public static IRopeSegment AppendAbove(IRopeSegment self, IRopeSegment segment)
    {
        segment.ConnectAbove(self.ConnectedAbove);
        self.ConnectAbove(segment.gameObject.GetComponent<Rigidbody2D>());
        segment.ConnectBelow(self.gameObject.GetComponent<Rigidbody2D>());
        return segment;
    }

    public static IRopeSegment AppendAbove(IRopeSegment self) =>
        AppendAbove(self, CreateSegment(self));

    public static IRopeSegment AppendBelow(IRopeSegment self, IRopeSegment segment)
    {
        segment.ConnectAbove(self.gameObject.GetComponent<Rigidbody2D>());
        self.ConnectBelow(segment.gameObject.GetComponent<Rigidbody2D>());
        segment.ConnectBelow(self.ConnectedBelow);
        return segment;
    }

    public static IRopeSegment AppendBelow(IRopeSegment self) =>
        AppendBelow(self, CreateSegment(self));

    public static void RemoveFromRope(RopeSegment segment)
    {
        var aboveSegment = segment.ConnectedAbove?.GetComponent<RopeSegment>();
        if (aboveSegment != null)
        {
            segment.ConnectBelow(null);
            aboveSegment.ConnectBelow(segment.ConnectedBelow);
        }

        var belowSegment = segment.ConnectedBelow?.GetComponent<RopeSegment>();
        if (belowSegment != null)
        {
            segment.ConnectBelow(null);
            belowSegment.ConnectBelow(segment.ConnectedAbove);
        }

        segment.rope.segmentCount--;
        segment.rope = null;
    }

    public static RopeSegment CreateSegment(IRopeSegment self)
    {
        self.rope.segmentCount++;

        GameObject newSegmentObject = Object.Instantiate(
            self.rope.segmentPrefab,
            self.transform.position,
            Quaternion.identity,
            self.rope.transform
        );
        RopeSegment newSegment = newSegmentObject.GetComponent<RopeSegment>();
        newSegment.rope = self.rope;
        HingeJoint2D newHinge = newSegment.GetComponent<HingeJoint2D>();
        newHinge.connectedAnchor = new Vector2(0, -self.rope.SegmentLength);
        return newSegment;
    }

    public static RopeSegment CreateSegment(Rope rope)
    {
        rope.segmentCount++;

        GameObject newSegmentObject = Object.Instantiate(
            rope.segmentPrefab,
            rope.transform.position,
            Quaternion.identity,
            rope.transform
        );
        RopeSegment newSegment = newSegmentObject.GetComponent<RopeSegment>();
        newSegment.rope = rope;
        HingeJoint2D newHinge = newSegment.GetComponent<HingeJoint2D>();
        newHinge.connectedAnchor = new Vector2(0, -rope.SegmentLength);
        return newSegment;
    }
}
