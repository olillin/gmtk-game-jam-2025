using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public int maxSegments = 50;

    [SerializeField]
    private float segmentLength = 0.6f;
    public float SegmentLength
    {
        get => segmentLength;
    }

    public RopeSegment hook;

    [Space]
    /// <summary>
    /// How far segments may be apart before they are considered stretched
    /// </summary>
    public float maxStretchDistance = 1.0f;
    public bool autoExtend = false;

    public float squishDistance = 0.1f;

    [Space]
    public float contractForce = 1.0f;
    public bool contract = false;

    [Space]
    public RopeSegment top;
    public RopeSegment bottom;
    public GameObject segmentPrefab;
    public int segmentCount = 2;

    private LineRenderer lineRenderer;

    private float manualTimer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        GenerateRopeLoop();
    }

    void Update()
    {
        manualTimer += Time.deltaTime;

        if (manualTimer > 0.1)
        {
            if (Input.GetKey(KeyCode.W))
                ExtendFromBottom();
            else if (Input.GetKey(KeyCode.S))
                ShortenRope();
            manualTimer = 0;
        }

        // if (contract)
        //     ContractSegments();
    }

    private void ContractSegments()
    {
        foreach (RopeSegment segment in GetSegments())
        {
            Rigidbody2D body = segment.GetComponent<Rigidbody2D>();
            Vector2 position = segment.transform.position;
            Vector2 abovePosition = (Vector2)segment.ConnectedAbove.transform.position - position;
            Vector2 belowPosition = (Vector2)segment.ConnectedBelow.transform.position - position;

            float angle =
                Mathf.Atan(abovePosition.y / abovePosition.x)
                + Mathf.Acos(Vector2.Dot(abovePosition, belowPosition)) / 2;
            float x = Mathf.Cos(angle);
            float y = Mathf.Sin(angle);
            Vector2 direction = new Vector2(x, y);
            Vector2 force = direction * contractForce;

            body.AddForce(force);
        }
    }

    void LateUpdate()
    {
        if (autoExtend && IsBottomStretched())
            ExtendFromBottom();
        if (autoExtend && IsTopStretched())
            ExtendFromTop();

        UpdateLine();

        int i = 0;
        foreach (var segment in GetSegments())
        {
            segment.transform.name = i++.ToString();
        }
    }

    void GenerateRopeLoop()
    {
        RopeSegment prevSegment = RopeHelper.CreateSegment(this);
        Rigidbody2D hookBody = hook.GetComponent<Rigidbody2D>();
        prevSegment.ConnectAbove(hookBody);
        top = prevSegment;

        RopeSegment newSegment = prevSegment.AppendBelow();
        prevSegment = newSegment;

        bottom = prevSegment;
        bottom.ConnectBelow(hookBody);

        hook.ConnectAbove(bottom.GetComponent<Rigidbody2D>());
        hook.ConnectBelow(top.GetComponent<Rigidbody2D>());
    }

    /** Add a new segment to the bottom of the rope */
    public void ExtendFromBottom()
    {
        if (segmentCount >= maxSegments)
            return;

        RopeSegment newSegment = bottom.AppendBelow();
        bottom = newSegment;

        segmentCount++;
    }

    /** Add a new segment to the top of the rope */
    public void ExtendFromTop()
    {
        if (segmentCount >= maxSegments)
            return;

        RopeSegment newSegment = top.AppendAbove();
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
        foreach (RopeSegment segment in GetSegments())
        {
            segment.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }

    bool IsTopStretched()
    {
        Anchor anchor = null;
        var segments = hook.GetSegmentsBelowUntil(segment =>
        {
            if (segment.gameObject.tag != "Anchor")
                return false;
            anchor = segment.GetComponent<Anchor>();
            return true;
        });
        Debug.Log(
            "Top segments: "
                + string.Join(
                    ',',
                    Array.ConvertAll(segments.ToArray(), segment => segment.transform.name)
                )
        );
        if (anchor != null && anchor.IsAttached)
            return false;
        return CalculateDistances(segments).Any(distance => distance > maxStretchDistance);
    }

    bool IsBottomStretched()
    {
        Anchor anchor = null;
        var segments = hook.GetSegmentsAboveUntil(segment =>
        {
            if (segment.gameObject.tag != "Anchor")
                return false;
            anchor = segment.GetComponent<Anchor>();
            return true;
        });
        Debug.Log(
            "Bottom segments: "
                + string.Join(
                    ',',
                    Array.ConvertAll(segments.ToArray(), segment => segment.transform.name)
                )
        );
        if (anchor != null && anchor.IsAttached)
            return false;
        return CalculateDistances(segments).Any(distance => distance > maxStretchDistance);
    }

    public static IEnumerable<float> CalculateDistances(IEnumerable<RopeSegment> segments)
    {
        RopeSegment prevSegment = null;
        foreach (RopeSegment segment in segments)
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

        foreach (RopeSegment segment in GetSegments())
        {
            positions.Add(segment.transform.position);
        }

        lineRenderer.SetPositions(positions.ToArray());
        lineRenderer.positionCount = positions.Count;
    }

    public IEnumerable<RopeSegment> GetSegments() => top.GetSegmentsBelow();

    public void RemoveSegment(RopeSegment segment)
    {
        if (segment == top)
            top = segment.ConnectedBelow?.GetComponent<RopeSegment>();
        if (segment == bottom)
            bottom = segment.ConnectedAbove?.GetComponent<RopeSegment>();

        var aboveSegment = segment.ConnectedAbove?.GetComponent<RopeSegment>();
        var belowSegment = segment.ConnectedBelow?.GetComponent<RopeSegment>();

        if (aboveSegment != null)
            aboveSegment.ConnectBelow(segment.ConnectedBelow);

        if (belowSegment != null)
            belowSegment.ConnectAbove(segment.ConnectedAbove);

        segment.ConnectBelow(null);
        segment.ConnectAbove(null);

        segmentCount--;
        segment.rope = null;
    }

    public void ReplaceSegment(RopeSegment oldSegment, RopeSegment newSegment)
    {
        RopeSegment above = oldSegment.ConnectedAbove.GetComponent<RopeSegment>();
        RemoveSegment(oldSegment);
        above.AppendBelow(newSegment);
    }

    public bool Contains(RopeSegment segment) => GetSegments().Any(s => s == segment);
}
