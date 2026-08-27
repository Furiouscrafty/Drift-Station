using UnityEngine;
using TMPro;
using System.Text;
using static ShipUpgradeData;

public class PlanetOrbitDisplay : MonoBehaviour
{
    [System.Serializable]
    public struct PlanetResourceEntry
    {
        public Planet planet;
        public ShipResourceData resourceData;
    }

    [Header("Data")]
    [Tooltip("Used to read ActivePlanet so the current planet can be highlighted or shown alone.")]
    public ShipUpgradeData shipData;

    [Tooltip("One entry per planet, each pointing at that planet's own ShipResourceData asset.")]
    public PlanetResourceEntry[] planetResources;

    [Header("UI")]
    public TMP_Text orbitDisplayText;

    [Tooltip("If true, only shows the active planet's orbit count. If false, lists every planet with the active one marked.")]
    public bool showActivePlanetOnly = false;

    [Tooltip("Marker appended next to the active planet's line when listing all planets.")]
    public string activeMarker = "  <-- current";

    private void OnEnable()
    {
        RefreshDisplay();
    }

    // Call this whenever orbit counts might have changed (e.g. after CompleteOrbit,
    // or when this UI becomes visible) rather than polling every frame in Update().
    public void RefreshDisplay()
    {
        if (orbitDisplayText == null || planetResources == null || planetResources.Length == 0)
            return;

        if (showActivePlanetOnly)
        {
            ShowActiveOnly();
        }
        else
        {
            ShowAllPlanets();
        }
    }

    private void ShowActiveOnly()
    {
        if (shipData == null)
        {
            orbitDisplayText.text = "";
            return;
        }

        ShipResourceData activeData = FindResourceData(shipData.ActivePlanet);
        if (activeData == null)
        {
            orbitDisplayText.text = $"{shipData.ActivePlanet}: no data";
            return;
        }

        orbitDisplayText.text = $"Highest Number of Orbits with {shipData.ActivePlanet}: {activeData.HighestNumOrbit}";
    }

    private void ShowAllPlanets()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var entry in planetResources)
        {
            if (entry.resourceData == null)
                continue;

            bool isActive = shipData != null && entry.planet == shipData.ActivePlanet;
            string marker = isActive ? activeMarker : "";

            sb.AppendLine($"Highest Number of Orbits: {entry.resourceData.HighestNumOrbit}{marker}");
        }

        orbitDisplayText.text = sb.ToString();
    }

    private ShipResourceData FindResourceData(Planet planet)
    {
        foreach (var entry in planetResources)
        {
            if (entry.planet == planet)
                return entry.resourceData;
        }
        return null;
    }
}
