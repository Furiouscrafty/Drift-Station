using UnityEngine;

public class BrickLaser : MonoBehaviour
{
    [Header("Laser Settings")]
    public bool laserActive = true;
    public float laserDistance = 20f;
    public Vector3 laserDirection = Vector3.forward;
    [Tooltip("Laser ONLY collides with these layers. Make sure ALL of SpaceShip, Shield, and Solar Panel are ticked here!")]
    public LayerMask hitMask;

    [Header("Ship Data")]
    [Tooltip("Used to check upgrade state (e.g. UpgradeSol) so hit effects can vary accordingly.")]
    public ShipUpgradeData shipData;
    public ShipResourceData shipResourceData;

    [Header("Damage Settings")]
    public float heatPerHit = 5f;
    public float heatPerSolarHit = 2f;              // used when solar panel IS upgraded
    public float heatPerSolarHitUnupgraded = 5f;     // used when solar panel is NOT upgraded
    public float powerGainPerSolarHit = 5f;
    public float powerDrainPerShieldHit = 5f;
    public float ShieldDrainPerShieldHit = 5f;

    // Cached layer indices, resolved once instead of every frame
    private int spaceShipLayer;
    private int shieldLayer;
    private int solarPanelLayer;

    private void Awake()
    {
        spaceShipLayer = ResolveLayer("SpaceShip");
        shieldLayer = ResolveLayer("Shield");
        solarPanelLayer = ResolveLayer("Solar Panel");

        if (shipData == null)
            Debug.LogWarning("[BrickLaser] No ShipUpgradeData assigned — solar panel hits will fall back to the unupgraded heat value.");

        if (shipResourceData == null)
            Debug.LogWarning("[BrickLaser] No ShipResourceData assigned — hits will not apply.");

        // Print exactly which layers hitMask currently includes, so you can
        // confirm in the Console that all 3 are actually turned on.
        Debug.Log($"[BrickLaser] hitMask includes: {LayerMaskToString(hitMask)}");
    }

    private int ResolveLayer(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer == -1)
        {
            Debug.LogError($"[BrickLaser] Layer \"{layerName}\" does not exist (check spelling/case in Tags & Layers settings).");
        }
        return layer;
    }

    private void Update()
    {
        if (laserActive)
            ShootLaser();
    }

    private void ShootLaser()
    {
        Vector3 worldDir = transform.TransformDirection(laserDirection);
        if (Physics.Raycast(transform.position, worldDir, out RaycastHit hit, laserDistance, hitMask))
        {
            int layer = hit.collider.gameObject.layer;

            // Collider may live on a child object (e.g. Blender-imported meshes),
            // so search up the hierarchy for the handler rather than assuming
            // it's on the exact object the ray hit.
            SpaceshipHandler handler = hit.collider.GetComponentInParent<SpaceshipHandler>();

            if (layer == spaceShipLayer)
            {
                shipResourceData.ishit = true;
                Debug.Log("Laser hit SPACE SHIP: " + hit.collider.name);
                if (handler != null)
                    shipResourceData.Heat += heatPerHit;
            }
            else if (layer == shieldLayer)
            {
                Debug.Log("Laser hit SHIELD: " + hit.collider.name);
                // Shield blocks heat buildup entirely, but takes power drain instead.
                if (handler != null)
                {
                    handler.resources.Power = Mathf.Max(0f, handler.resources.Power - powerDrainPerShieldHit);
                    handler.resources.ShieldHealth = Mathf.Max(0f, handler.resources.ShieldHealth - ShieldDrainPerShieldHit);
                }
            }
            else if (layer == solarPanelLayer)
            {
                Debug.Log("Laser hit SOLAR PANEL: " + hit.collider.name);
                if (handler != null)
                {
                    shipResourceData.ishit = true;
                    bool isUpgraded = shipData != null && shipData.UpgradeSol;
                    float appliedHeat = isUpgraded ? heatPerSolarHit : heatPerSolarHitUnupgraded;

                    handler.resources.Heat += appliedHeat;
                    handler.resources.Power += powerGainPerSolarHit;
                }
            }
            else
            {
                Debug.Log($"Laser hit OTHER: {hit.collider.name} (layer: {LayerMask.LayerToName(layer)})");
                shipResourceData.ishit = false;
            }

            Debug.DrawRay(transform.position, worldDir * hit.distance, Color.yellow);
            return;
        }
        Debug.DrawRay(transform.position, worldDir * laserDistance, Color.red);
    }

    // Utility: turns a LayerMask into a readable list of layer names for debugging
    private static string LayerMaskToString(LayerMask mask)
    {
        string result = "";
        for (int i = 0; i < 32; i++)
        {
            if ((mask.value & (1 << i)) != 0)
            {
                string name = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(name))
                    result += name + ", ";
            }
        }
        return string.IsNullOrEmpty(result) ? "(none)" : result.TrimEnd(',', ' ');
    }
}