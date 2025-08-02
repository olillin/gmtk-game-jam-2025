using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float throwStrength = 20.0f;
    public float spawnDistance = 2.0f;
    public float pullStrength = 1.0f;
    public int maxAnchors = 3;

    [Space]
    [SerializeField]
    private GameObject anchorPrefab;

    [SerializeField]
    private Rope rope;

    [SerializeField]
    private RopeSegment kinematicRopeSegment;

    private List<Anchor> anchors = new List<Anchor>();

    private Rigidbody2D rb;
    public Rigidbody2D Rigidbody
    {
        get => rb;
    }
    private Camera mainCamera;
    private RopeSegment dynamicRopeSegment;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        dynamicRopeSegment = GetComponent<RopeSegment>();
        dynamicRopeSegment.rope = rope;
    }

    void Update()
    {
        KeyCode pullButton = KeyCode.Space;
        if (Input.GetKeyUp(pullButton))
        {
            Debug.Log("End pull");
            MoveRopeToKinematic();
            DeleteAllAnchors();
            rope.contract = false;
        }
        else if (Input.GetKeyDown(pullButton))
        {
            Debug.Log("Start pull");
            MoveRopeToDynamic();
            rope.contract = true;
        }
        if (Input.GetKey(pullButton))
        {
            MoveTowardsRope();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 playerScreenPos = mainCamera.WorldToScreenPoint(transform.position);

            Vector2 mouseDelta = mousePos - playerScreenPos;
            ThrowRope(mouseDelta.normalized);
        }

        rope.autoExtend = HasUnattachedAnchors();
    }

    void ThrowRope(Vector2 direction)
    {
        if (anchors.Count >= maxAnchors)
            return;

        Vector2 position = (Vector2)transform.position + direction * spawnDistance;
        GameObject anchorGameObject = Instantiate(anchorPrefab, position, Quaternion.identity);
        Anchor anchor = anchorGameObject.GetComponent<Anchor>();
        Rigidbody2D anchorRb = anchorGameObject.GetComponent<Rigidbody2D>();

        RopeSegment anchorSegment = anchor.GetComponent<RopeSegment>();
        anchorSegment.rope = rope;
        rope.bottom.AppendAbove(anchorSegment);

        anchorRb.linearVelocity = direction * throwStrength;

        anchors.Add(anchor);
    }

    void MoveTowardsRope()
    {
        RopeSegment segment = dynamicRopeSegment;
        Rigidbody2D body = rb;
        Vector2 position = segment.transform.position;
        Vector2 abovePosition = (Vector2)segment.ConnectedAbove.transform.position - position;
        Vector2 belowPosition = (Vector2)segment.ConnectedBelow.transform.position - position;

        // float v = Mathf.Acos(
        //     Vector2.Dot(abovePosition, belowPosition)
        //         / (abovePosition.magnitude * belowPosition.magnitude)
        // );
        // float angle = Mathf.Atan(abovePosition.y / abovePosition.x) + v / 2;
        // float x = Mathf.Cos(angle);
        // float y = Mathf.Sin(angle);
        // Vector2 direction = new Vector2(x, y);
        // // Vector2 force = direction * pullStrength;

        body.AddForce(abovePosition.normalized * pullStrength);
        body.AddForce(belowPosition.normalized * pullStrength);
    }

    public bool HasUnattachedAnchors() => anchors.Any(anchor => !anchor.IsAttached);

    public void RemoveAnchor(Anchor anchor)
    {
        anchors.Remove(anchor);
    }

    void MoveRopeToDynamic()
    {
        if (rope.Contains(kinematicRopeSegment))
            rope.ReplaceSegment(kinematicRopeSegment, dynamicRopeSegment);
    }

    void MoveRopeToKinematic()
    {
        if (rope.Contains(dynamicRopeSegment))
            rope.ReplaceSegment(dynamicRopeSegment, kinematicRopeSegment);
    }

    public void DeleteAllAnchors()
    {
        foreach (Anchor anchor in anchors)
        {
            Destroy(anchor.gameObject);
        }
        anchors.Clear();
    }

    List<Anchor> GetAttachedAnchors()
    {
        return anchors.FindAll(anchor => anchor.IsAttached);
    }
}
