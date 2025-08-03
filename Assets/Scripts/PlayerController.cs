using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float throwStrength = 20.0f;
    public float spawnDistance = 2.0f;
    public float pullStrength = 1.0f;
    public int maxAnchors = 2;

    [SerializeField]
    private GameObject anchorPrefab;

    [SerializeField]
    private Transform spawnPoint;

    [Header("Controls")]
    public KeyCode pullButton = KeyCode.Space;
    public KeyCode throwButton = KeyCode.Mouse0;
    public KeyCode undoButton = KeyCode.Q;
    public KeyCode undoAllButton = KeyCode.A;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private LineRenderer lineRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(pullButton))
        {
            DeleteAllAnchors();
        }

        if (Input.GetKey(pullButton))
        {
            PullAnchors();
        }
        else if (Input.GetKeyUp(throwButton))
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 playerScreenPos = mainCamera.WorldToScreenPoint(transform.position);

            Vector2 mouseDelta = mousePos - playerScreenPos;
            ThrowRope(mouseDelta.normalized);
        }

        if (Input.GetKeyDown(undoButton))
        {
            DeleteLatestAnchor();
        }
        if (Input.GetKeyDown(undoAllButton))
        {
            DeleteAllAnchors();
        }

        UpdateLine();
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

        rb.AddForce(direction * pullStrength * Time.deltaTime);
    }

    public void DeleteAllAnchors()
    {
        foreach (Anchor anchor in FindAnchors())
        {
            Destroy(anchor.gameObject);
        }
    }

    public void DeleteLatestAnchor()
    {
        Anchor latestAnchor = null;
        foreach (Anchor anchor in FindAnchors())
        {
            if (latestAnchor == null || anchor.Age < latestAnchor.Age)
                latestAnchor = anchor;
        }
        if (latestAnchor != null)
            Destroy(latestAnchor.gameObject);
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

    void UpdateLine(float lineZ = 1.0f)
    {
        List<Vector3> positions = new List<Vector3>()
        {
            new Vector3(transform.position.x, transform.position.y, lineZ),
        };
        FindAnchors()
            .ForEach(anchor =>
            {
                positions.Add(
                    new Vector3(anchor.transform.position.x, anchor.transform.position.y, lineZ)
                );
            });
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
        lineRenderer.numCornerVertices = positions.Count > 2 ? 1 : 0;
    }
}
