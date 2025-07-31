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

    [SerializeField]
    private GameObject anchorPrefab;

    [SerializeField]
    private Transform spawnPoint;

    private Rigidbody2D rb;
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
            PullAnchors();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 playerScreenPos = mainCamera.WorldToScreenPoint(transform.position);

            Vector2 mouseDelta = mousePos - playerScreenPos;
            ThrowRope(mouseDelta.normalized);
        }
    }

    public void Respawn()
    {
        DeleteAllAnchors();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = spawnPoint.position;
    }

    void ThrowRope(Vector2 direction)
    {
        List<Anchor> currentAnchors = FindAnchors();
        if (currentAnchors.Count >= maxAnchors)
            return;

        Vector2 position = (Vector2)transform.position + direction * spawnDistance;
        GameObject anchor = Instantiate(anchorPrefab, position, Quaternion.identity);
        Rigidbody2D anchorRb = anchor.GetComponent<Rigidbody2D>();

        anchorRb.linearVelocity = direction * throwStrength;
    }

    void PullAnchors()
    {
        List<Anchor> anchors = FindAttachedAnchors();

        if (anchors.Count == 0)
            return;

        List<Vector2> relativePositions = anchors.ConvertAll<Vector2>(anchor =>
            anchor.transform.position - transform.position
        );
        Vector2 direction = new Vector2(
            relativePositions.Average(pos => pos.x),
            relativePositions.Average(pos => pos.y)
        ).normalized;
        Vector2 posSum = new Vector2(
            relativePositions.Sum(pos => pos.x),
            relativePositions.Sum(pos => pos.y)
        );
        Vector2 force = direction * posSum.magnitude;

        rb.AddForce(force * pullStrength);
    }

    public void DeleteAllAnchors()
    {
        foreach (Anchor anchor in FindAnchors())
        {
            Destroy(anchor.gameObject);
        }
    }

    List<Anchor> FindAttachedAnchors()
    {
        return FindAnchors().FindAll(anchor => anchor.IsAttached);
    }

    List<Anchor> FindAnchors()
    {
        List<Anchor> anchors = new List<Anchor>();

        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Anchor"))
        {
            Anchor anchor = gameObject.GetComponent<Anchor>();
            anchors.Add(anchor);
        }

        return anchors;
    }
}
