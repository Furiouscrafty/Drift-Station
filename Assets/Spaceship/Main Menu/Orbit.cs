using UnityEngine;

public class Orbit : MonoBehaviour
{
    [Header("Orbit Target")]
    public Transform target;

    [Header("Orbit Settings")]
    public float orbitRadius = 5f;
    public float orbitSpeed = 20f;

    [Header("Tilt / Plane")]
    public Vector3 orbitAxis = Vector3.up;   // Default = horizontal orbit

    private float angle;

    private void Update()
    {
        if (target == null)
            return;

        // Increase angle over time
        angle += orbitSpeed * Time.deltaTime;

        // Build rotation around chosen axis
        Quaternion rotation = Quaternion.AngleAxis(angle, orbitAxis.normalized);

        // Apply orbit
        Vector3 offset = rotation * (Vector3.forward * orbitRadius);
        transform.position = target.position + offset;

        // Optional: face the target
        transform.LookAt(target);
    }
}
