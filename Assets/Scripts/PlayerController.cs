using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            DeleteAllAnchors();
        }
        if (Input.GetMouseButton(1))
        {
            if (!anchors.Any(anchor => anchor.IsPulling))
                StartPullingAnchors();
        }
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

        anchorRb.linearVelocity = direction * throwStrength;

        anchors.Add(anchor);
    }

    void StartPullingAnchors()
    {
        List<Anchor> attachedAnchors = GetAttachedAnchors();

        if (attachedAnchors.Find(anchor => anchor.IsPulling) != null)
            return;

        foreach (Anchor anchor in attachedAnchors)
        {
            anchor.StartPullingPlayer(this);
        }
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
