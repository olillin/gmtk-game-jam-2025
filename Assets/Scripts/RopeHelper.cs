using UnityEngine;

public class RopeHelper
{
    public static RopeSegment AppendAbove(RopeSegment self, RopeSegment segment)
    {
        Rigidbody2D segmentBody = segment.gameObject.GetComponent<Rigidbody2D>();
        segment.ConnectBelow(self.gameObject.GetComponent<Rigidbody2D>());
        if (self.ConnectedAbove != null)
        {
            segment.ConnectAbove(self.ConnectedAbove);
            self.ConnectedAbove.GetComponent<RopeSegment>()?.ConnectBelow(segmentBody);
        }
        self.ConnectAbove(segmentBody);
        return segment;
    }

    public static RopeSegment AppendAbove(RopeSegment self) =>
        AppendAbove(self, CreateSegment(self));

    public static RopeSegment AppendBelow(RopeSegment self, RopeSegment segment)
    {
        Rigidbody2D segmentBody = segment.gameObject.GetComponent<Rigidbody2D>();
        segment.ConnectAbove(self.gameObject.GetComponent<Rigidbody2D>());
        if (self.ConnectedBelow != null)
        {
            segment.ConnectBelow(self.ConnectedBelow);
            self.ConnectedBelow.GetComponent<RopeSegment>()?.ConnectAbove(segmentBody);
        }
        self.ConnectBelow(segmentBody);
        return segment;
    }

    public static RopeSegment AppendBelow(RopeSegment self) =>
        AppendBelow(self, CreateSegment(self));

    public static RopeSegment CreateSegment(RopeSegment self)
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
