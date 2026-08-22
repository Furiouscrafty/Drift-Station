using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "NewPurchaseData", menuName = "Game/Purchase Data")]
public class PurchaseData : ScriptableObject
{
    [Header("Colours")]
    public List<Color> UnpurchasedColours = new List<Color>();
    public List<Color> PurchasedColours = new List<Color>();
    [Tooltip("All colours cost the same amount to purchase.")]
    public int ColourCost;

    [System.Serializable]
    public class PlanetPurchaseOption
    {
        public ShipUpgradeData.Planet Planet;
        public int Cost;
    }

    [Header("Planets")]
    public List<PlanetPurchaseOption> UnpurchasedPlanets = new List<PlanetPurchaseOption>();
    public List<ShipUpgradeData.Planet> PurchasedPlanets = new List<ShipUpgradeData.Planet>();

    [System.Serializable]
    public class LevelUpgradeCost
    {
        [Tooltip("Cost to go FROM this index TO index+1. E.g. element 0 = cost to go from level 0 to level 1.")]
        public List<int> CostPerLevel = new List<int>();
    }
    [Header("Level Upgrade Costs")]
    public LevelUpgradeCost BatteryCosts;
    public LevelUpgradeCost ShieldCosts;
    public LevelUpgradeCost CommCosts;
    public LevelUpgradeCost SolarPanelCosts;
    [Header("Level Upgrade Ownership")]
    [Tooltip("Highest level currently paid for for each upgrade. Player can freely move between 0 and this level; going above costs money and raises this cap.")]
    public int MaxOwnedBattLevel;
    public int MaxOwnedSolPanLevel;
    public int MaxOwnedShieldLevel;
    public int MaxOwnedCommLevel;
    [Header("Toggle Upgrades")]
    [Tooltip("Has the player purchased this upgrade at all? Permanent once true.")]
    public bool BackupBatteryOwned;
    public int BackupBatteryCost;
    [Tooltip("Has the player purchased this upgrade at all? Permanent once true.")]
    public bool UpgradeSolOwned;
    public int UpgradeSolCost;
}