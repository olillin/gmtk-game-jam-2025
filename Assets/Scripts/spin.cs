using UnityEngine;

public class spin : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0f, 0f, 100f);

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
