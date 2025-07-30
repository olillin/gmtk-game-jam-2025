using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float throwStrength = 20.0f;
    public float spawnDistance = 2.0f;
    public float pullStrength = 1.0f;

    [SerializeField]
    private GameObject anchorPrefab;

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
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 playerScreenPos = mainCamera.WorldToScreenPoint(transform.position);

            Vector2 mouseDelta = mousePos - playerScreenPos;
            ThrowRope(mouseDelta.normalized);
        }

        if (Input.GetMouseButton(1))
        {
            PullAnchors();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            DeleteAllAnchors();
        }
    }

    void ThrowRope(Vector2 direction)
    {
        Vector2 position = (Vector2)transform.position + direction * spawnDistance;
        GameObject anchor = Instantiate(anchorPrefab, position, Quaternion.identity);
        Rigidbody2D anchorRb = anchor.GetComponent<Rigidbody2D>();

        anchorRb.linearVelocity = direction * throwStrength;
    }

    void PullAnchors()
    {
        List<Transform> anchors = FindAnchors();

        if (anchors.Count == 0)
            return;

        List<Vector2> relativePositions = anchors.ConvertAll<Vector2>(anchor =>
            anchor.position - transform.position
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
        foreach (Transform anchor in FindAnchors())
        {
            Destroy(anchor.gameObject);
        }
    }

    List<Transform> FindAnchors()
    {
        List<Transform> anchors = new List<Transform>();

        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Anchor"))
        {
            Anchor anchor = gameObject.GetComponent<Anchor>();
            if (anchor.IsAttached)
            {
                anchors.Add(gameObject.transform);
            }
        }

        return anchors;
    }
}
