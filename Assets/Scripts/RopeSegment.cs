using UnityEngine;

public class RopeSegment : MonoBehaviour
{
    private HingeJoint2D hinge;
    public GameObject connectedAbove,
        connectedBelow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hinge = GetComponent<HingeJoint2D>();
        connectedAbove = hinge.connectedBody.gameObject;
        RopeSegment aboveSegment = connectedAbove.GetComponent<RopeSegment>();
        if (aboveSegment != null)
        {
            aboveSegment.connectedBelow = gameObject;
            float bottom = connectedAbove.GetComponent<BoxCollider2D>().bounds.size.y;
            hinge.connectedAnchor = new Vector2(0, -0.5f);
        }
        else
        {
            hinge.connectedAnchor = new Vector2(0, 0);
        }
    }

    // Update is called once per frame
    void Update() { }
}
