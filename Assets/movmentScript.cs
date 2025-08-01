using Unity.VisualScripting;
using UnityEngine;

public class movmentScript : MonoBehaviour
{

    public Rigidbody2D rb;
    public float timeToDie = 0.1f;

    public float startX = -12f;
    public float endX = 12f;
    public float velocity = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.linearVelocityX = velocity;
        Object.Destroy(gameObject, timeToDie);
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.position.x > endX)
        {
            Vector3 pos = rb.position;
            pos.x = startX;
            rb.position = pos;
        }
    }
}
