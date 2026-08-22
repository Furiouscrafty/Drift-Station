using UnityEngine;
using TMPro;

public class OrbitScript : MonoBehaviour
{
    [System.Serializable]
    public struct PlanetOrbitSettings
    {
        public ShipUpgradeData.Planet planet;
        public float orbitSpeed;
        public float orbitRadius;
        public int cashPerOrbit;
    }

    [Header("Data")]
    public ShipUpgradeData shipData;

    [Header("Orbit Target")]
    public Transform target; // Object to orbit around

    [Header("Per-Planet Orbit Settings")]
    [Tooltip("Add one entry per planet with its own speed, radius, and cash reward.")]
    public PlanetOrbitSettings[] planetSettings;

    [Tooltip("Starting position on the orbit, in degrees (0-360).")]
    public float startAngleDegrees = 0f;

    [Header("UI")]
    public TMP_Text orbitRewardText;
    [Tooltip("How long the reward message stays on screen, in seconds.")]
    public float messageDuration = 2f;
    public TMP_Text orbitCountText;

    private float angle;              // used for actual position (includes start offset)
    private float traveledRadians;    // used purely to detect a completed lap (always starts at 0)
    private float orbitSpeed;
    private float orbitRadius;
    private int cashPerOrbit;
    private float messageTimer;
    private int orbitsCompleted;

    private void Start()
    {
        angle = startAngleDegrees * Mathf.Deg2Rad;
        traveledRadians = 0f;
        ApplyPlanetSettings();

        if (orbitRewardText != null)
            orbitRewardText.text = "";

        UpdateOrbitCountText();
    }

    private void Update()
    {
        if (target == null)
            return;

        float delta = orbitSpeed * Time.deltaTime;

        // Position angle (keeps the start offset)
        angle += delta;
        if (angle >= Mathf.PI * 2f)
            angle -= Mathf.PI * 2f;

        // Distance traveled since start/last completed orbit (ignores start offset)
        traveledRadians += delta;
        if (traveledRadians >= Mathf.PI * 2f)
        {
            traveledRadians -= Mathf.PI * 2f;
            CompleteOrbit();
        }

        // Calculate orbit position
        float x = Mathf.Cos(angle) * orbitRadius;
        float z = Mathf.Sin(angle) * orbitRadius;

        // Apply position
        transform.position = target.position + new Vector3(x, 0f, z);

        // Keep the moon facing the planet
        transform.LookAt(target);

        // Countdown for hiding the reward message
        if (messageTimer > 0f)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0f && orbitRewardText != null)
                orbitRewardText.text = "";
        }
    }

    private void CompleteOrbit()
    {
        orbitsCompleted++;
        UpdateOrbitCountText();

        if (shipData == null || cashPerOrbit <= 0)
            return;

        shipData.Money += cashPerOrbit;

        if (orbitRewardText != null)
        {
            orbitRewardText.text = $"Orbit complete: +{cashPerOrbit} cash";
            messageTimer = messageDuration;
        }
    }

    private void UpdateOrbitCountText()
    {
        if (orbitCountText != null)
            orbitCountText.text = $"Orbits completed: {orbitsCompleted}";
    }

    public void ApplyPlanetSettings()
    {
        if (shipData == null || planetSettings.Length == 0)
        {
            Debug.LogWarning("OrbitScript: Missing ShipUpgradeData or planetSettings is empty.");
            return;
        }

        foreach (var settings in planetSettings)
        {
            if (settings.planet == shipData.ActivePlanet)
            {
                orbitSpeed = settings.orbitSpeed;
                orbitRadius = settings.orbitRadius;
                cashPerOrbit = settings.cashPerOrbit;
                return;
            }
        }

        Debug.LogWarning($"OrbitScript: No orbit settings found for {shipData.ActivePlanet}. Using default speed/radius/cash of 0.");
    }
}