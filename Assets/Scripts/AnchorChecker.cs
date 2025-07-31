using UnityEngine;

public class AnchorChecker : MonoBehaviour
{
    [SerializeField]
    private Anchor parent;

    [SerializeField]
    private LayerMask snapLayers;

    void OnTriggerEnter2D(Collider2D collider)
    {
        bool isSnapLayer = (snapLayers.value & (1 << collider.gameObject.layer)) != 0;
        if (isSnapLayer)
            parent.Attach(collider.transform);
        else
            parent.Attach();
    }
}
