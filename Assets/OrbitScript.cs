using UnityEngine;
using TMPro;

public class OrbitScript : MonoBehaviour
{
    [Header("Data")]
    public ShipUpgradeData shipData;
    public ShipResourceData shipResourceData;

    [Header("Orbit Target")]
    public Transform target; // Object to orbit around

    [Header("Orbit Settings")]
    public float orbitSpeed = 1f;
    public float orbitRadius = 5f;
    public int cashPerOrbit = 10;

    [Tooltip("Starting position on the orbit, in degrees (0-360).")]
    public float startAngleDegrees = 0f;

    [Header("UI")]
    public TMP_Text orbitRewardText;
    [Tooltip("How long the reward message stays on screen, in seconds.")]
    public float messageDuration = 2f;
    public TMP_Text orbitCountText;

    private float angle;              // used for actual position (includes start offset)
    private float traveledRadians;    // used purely to detect a completed lap (always starts at 0)
    private float messageTimer;

    private void Start()
    {
        angle = startAngleDegrees * Mathf.Deg2Rad;
        traveledRadians = 0f;

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
        if (shipResourceData != null)
            shipResourceData.RegisterOrbitCompleted();

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
        if (orbitCountText != null && shipResourceData != null)
            orbitCountText.text = $"Orbits completed: {shipResourceData.CurrentOrbits}";
    }
}