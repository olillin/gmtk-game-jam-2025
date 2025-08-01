using UnityEngine;

public interface IRopeSegment
{
    public GameObject gameObject { get; }
    public Transform transform { get; }
    public Rope rope { get; set; }

    public Rigidbody2D ConnectedAbove { get; }
    public Rigidbody2D ConnectedBelow { get; }
    public void ConnectAbove(Rigidbody2D segment);
    public void ConnectBelow(Rigidbody2D segment);
    public IRopeSegment AppendAbove();
    public IRopeSegment AppendAbove(IRopeSegment segment);
    public IRopeSegment AppendBelow();
    public IRopeSegment AppendBelow(IRopeSegment segment);
}
