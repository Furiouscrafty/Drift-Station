using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RadarDish : MonoBehaviour
{
    [Header("Data")]
    public ShipUpgradeData shipData;

    [Header("Raycast Origin & Direction")]
    [Tooltip("The point the radar beam fires from. Usually the dish itself or a child transform.")]
    public Transform firePoint;

    [Header("Fire Direction")]
    [Tooltip("Local-space direction the laser fires in, relative to firePoint's rotation. Set this in the Inspector to aim the dish. (1,0,0) = right, (0,1,0) = up, (0,0,1) = forward, etc. Doesn't need to be normalized.")]
    public Vector3 fireDirection = Vector3.forward;

    [Header("Detection Settings")]
    public float maxDistance = 1000f;
    public LayerMask detectionLayers; // Set this to include your Planet and SpaceStation layers

    [Header("Visible Laser")]
    public Color hitColor = Color.green;
    public Color missColor = Color.red;
    [Tooltip("Width of the visible laser beam.")]
    public float laserWidth = 0.05f;

    [Header("Power Usage")]
    [Tooltip("How much power is drained per second while firing.")]
    public float powerDrainPerSecond = 10f;

    // Exposed results other scripts can read after a scan
    public bool IsHittingTarget { get; private set; }
    public GameObject HitObject { get; private set; }
    public string HitLayerName { get; private set; }

    private LineRenderer lineRenderer;
    private bool isFiring;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
        lineRenderer.enabled = false; // hidden until the player holds the button
    }

    private void Update()
    {
        if (!isFiring)
            return;

        if (shipData == null)
        {
            Debug.LogWarning("RadarDish: ShipUpgradeData is not assigned.");
            StopFiring();
            return;
        }

        // Stop firing automatically if power runs out
        if (shipData.Power <= 0f)
        {
            StopFiring();
            return;
        }

        shipData.Power -= powerDrainPerSecond * Time.deltaTime;
        if (shipData.Power < 0f)
            shipData.Power = 0f;

        Scan();
    }

    // Hook this to the UI button's EventTrigger -> PointerDown
    public void StartFiring()
    {
        if (firePoint == null)
        {
            Debug.LogWarning("RadarDish: firePoint is not assigned.");
            return;
        }

        if (shipData != null && shipData.Power <= 0f)
        {
            Debug.Log("RadarDish: Not enough power to fire.");
            return;
        }

        isFiring = true;
        lineRenderer.enabled = true;
    }

    // Hook this to the UI button's EventTrigger -> PointerUp (and PointerExit, for safety)
    public void StopFiring()
    {
        isFiring = false;
        lineRenderer.enabled = false;
        IsHittingTarget = false;
        HitObject = null;
        HitLayerName = null;
    }

    private void Scan()
    {
        Vector3 worldDirection = firePoint.TransformDirection(fireDirection.normalized);
        Ray ray = new Ray(firePoint.position, worldDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, detectionLayers))
        {
            IsHittingTarget = true;
            HitLayerName = LayerMask.LayerToName(hit.collider.gameObject.layer);

            // GetComponentInParent handles cases where the collider sits on a child
            // mesh from an imported FBX rather than the root object (same quirk as BrickLaser)
            HitObject = hit.collider.GetComponentInParent<Transform>().gameObject;

            HandleHit(hit, HitLayerName);

            DrawLaser(firePoint.position, hit.point, hitColor);
        }
        else
        {
            IsHittingTarget = false;
            HitObject = null;
            HitLayerName = null;

            Vector3 missEndPoint = firePoint.position + worldDirection * maxDistance;
            DrawLaser(firePoint.position, missEndPoint, missColor);
        }
    }

    private void DrawLaser(Vector3 start, Vector3 end, Color color)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    private void HandleHit(RaycastHit hit, string layerName)
    {
        switch (layerName)
        {
            case "Planet":
                OnHitPlanet(hit);
                break;

            case "SpaceStation":
                OnHitSpaceStation(hit);
                break;

            default:
                // Hit something on a detection layer that isn't specifically handled yet
                break;
        }
    }

    private void OnHitPlanet(RaycastHit hit)
    {
        // Planet-specific logic goes here (e.g. mission target confirmation, scanning progress)
        Debug.Log($"RadarDish: Detected planet - {hit.collider.name}");
    }

    private void OnHitSpaceStation(RaycastHit hit)
    {
        // Space station-specific logic goes here
        Debug.Log($"RadarDish: Detected space station - {hit.collider.name}");
    }
}