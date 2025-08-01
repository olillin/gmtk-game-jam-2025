using System.Collections.Generic;
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

    private List<Anchor> anchors = new List<Anchor>();

    private Rigidbody2D rb;
    public Rigidbody2D Rigidbody
    {
        get => rb;
    }
    private Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(1))
            DeleteAllAnchors();

        if (Input.GetMouseButton(1))
            StartPullingAnchors();
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 playerScreenPos = mainCamera.WorldToScreenPoint(transform.position);

            Vector2 mouseDelta = mousePos - playerScreenPos;
            ThrowRope(mouseDelta.normalized);
        }
    }

    void ThrowRope(Vector2 direction)
    {
        if (anchors.Count >= maxAnchors)
            return;

        Vector2 position = (Vector2)transform.position + direction * spawnDistance;
        GameObject anchorGameObject = Instantiate(anchorPrefab, position, Quaternion.identity);
        Anchor anchor = anchorGameObject.GetComponent<Anchor>();
        Rigidbody2D anchorRb = anchorGameObject.GetComponent<Rigidbody2D>();
        rope.top.AppendAbove(anchor.GetComponent<RopeSegment>());

        anchorRb.linearVelocity = direction * throwStrength;

        anchors.Add(anchor);
    }

    public void RemoveAnchor(Anchor anchor)
    {
        anchors.Remove(anchor);
    }

    void StartPullingAnchors()
    {
        List<Anchor> attachedAnchors = GetAttachedAnchors();
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
