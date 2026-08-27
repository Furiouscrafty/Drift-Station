using UnityEngine;

[CreateAssetMenu(fileName = "ShipUpgradeData", menuName = "Game/Ship Upgrade Data")]
public class ShipUpgradeData : ScriptableObject
{
    [Header("Upgrade Levels (0-3)")]
    [Range(0, 3)] public int BattLevel;
    [Range(0, 2)] public int SolPanLevel;
    [Range(0, 3)] public int ShieldLevel;
    [Range(0, 2)] public int CommLevel;

    [Header("Toggles")]
    public bool UpgradeSol;
    public bool BackupBat;

    [Header("Currency")]
    public int Money;

    [Header("Appearance - Hull")]
    public Color HullColour = Color.white;
    public Material HullMaterial;

    [Header("Appearance - Hub Connector")]
    public Color HubConnectorColour;
    public Material HubConnectorMaterial;

    [Header("Appearance - Connector")]
    public Color ConnectorColour;
    public Material ConnectorMaterial;

    [Header("Appearance - Ring")]
    public Color RingColour;
    public Material RingMaterial;

    [Header("Appearance - Shield")]
    public Color ShieldColour;
    public Material ShieldMaterial;

    [Header("Planets")]
    public Planet ActivePlanet = Planet.Earth;

    private void OnValidate()
    {
        BattLevel = Mathf.Clamp(BattLevel, 0, 3);
        SolPanLevel = Mathf.Clamp(SolPanLevel, 0, 3);
        ShieldLevel = Mathf.Clamp(ShieldLevel, 0, 3);
        CommLevel = Mathf.Clamp(CommLevel, 0, 3);
        Money = Mathf.Max(Money, 0);
    }

    public enum Planet
    {
        Mercury,
        Venus,
        Earth,
        Mars,
        Jupiter,
        Saturn,
        Uranus,
        Neptune,
        Pluto
    }
}