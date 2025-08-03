using UnityEngine;

[ExecuteInEditMode]
public class ResizeColliderToSprite : MonoBehaviour
{
    public Vector3 scale = Vector2.one;

    void Update()
    {
        var collider = GetComponent<BoxCollider2D>();
        var spriteRenderer = GetComponent<SpriteRenderer>();
        collider.size = new Vector2(
            Mathf.Abs(spriteRenderer.size.x * scale.x),
            Mathf.Abs(spriteRenderer.size.y * scale.y)
        );
    }
}
