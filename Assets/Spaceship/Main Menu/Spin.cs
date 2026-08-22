using UnityEngine;

public class Spin : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up;
    public float speed = 50f; // degrees per second

    void Update()
    {
        transform.Rotate(rotationAxis * speed * Time.deltaTime);
    }
}