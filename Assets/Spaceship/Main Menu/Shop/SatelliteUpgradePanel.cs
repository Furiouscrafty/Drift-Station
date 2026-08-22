using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SatelliteUpgradePanel : MonoBehaviour
{
    public enum ToggleUpgradeType { BackupBattery, AdditionalSolarPanel }
    public enum LevelUpgradeType { Battery, SolarPanel, Shield, Comm }

    [System.Serializable]
    public class ToggleEntry
    {
        public ToggleUpgradeType type;
        public Button button;
        public TMP_Text buttonText;
    }

    [System.Serializable]
    public class LevelEntry
    {
        public LevelUpgradeType type;
        public Button upgradeButton;     // SINGLE BUTTON
        public TMP_Text levelText;       // SINGLE TEXT
    }

    [Header("Data")]
    [SerializeField] private ShipUpgradeData shipData;
    [SerializeField] private PurchaseData purchaseData;

    [Header("Toggle Upgrades (Buttons)")]
    [SerializeField] private ToggleEntry[] toggleEntries;

    [Header("Level Upgrades (Single Button)")]
    [SerializeField] private LevelEntry[] levelEntries;

    private void OnEnable()
    {
        foreach (var entry in toggleEntries)
        {
            var captured = entry;
            captured.button.onClick.AddListener(() => OnToggleClicked(captured));
        }

        foreach (var entry in levelEntries)
        {
            var captured = entry;
            captured.upgradeButton.onClick.AddListener(() => OnLevelUpgrade(captured));
        }

        RefreshAll();
    }

    private void OnDisable()
    {
        foreach (var entry in toggleEntries)
            entry.button.onClick.RemoveAllListeners();

        foreach (var entry in levelEntries)
            entry.upgradeButton.onClick.RemoveAllListeners();
    }

    // ---------- Toggle Upgrades ----------

    private void OnToggleClicked(ToggleEntry entry)
    {
        bool owned = GetToggleOwned(entry.type);

        if (!owned)
        {
            int cost = GetToggleCost(entry.type);
            if (shipData.Money < cost)
            {
                Debug.Log($"[SatelliteUpgradePanel] Not enough money to buy {entry.type}.");
                return;
            }

            shipData.Money -= cost;
            SetToggleOwned(entry.type, true);
            SetToggleEnabled(entry.type, true);
        }
        else
        {
            bool current = GetToggleEnabled(entry.type);
            SetToggleEnabled(entry.type, !current);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(shipData);
        UnityEditor.EditorUtility.SetDirty(purchaseData);
#endif

        RefreshToggle(entry);
    }

    private void RefreshToggle(ToggleEntry entry)
    {
        bool owned = GetToggleOwned(entry.type);
        entry.buttonText.text = owned
            ? (GetToggleEnabled(entry.type) ? "Enabled" : "Disabled")
            : "Buy: " + GetToggleCost(entry.type);
    }

    private bool GetToggleOwned(ToggleUpgradeType type)
    {
        return type == ToggleUpgradeType.BackupBattery
            ? purchaseData.BackupBatteryOwned
            : purchaseData.UpgradeSolOwned;
    }

    private void SetToggleOwned(ToggleUpgradeType type, bool value)
    {
        if (type == ToggleUpgradeType.BackupBattery)
            purchaseData.BackupBatteryOwned = value;
        else
            purchaseData.UpgradeSolOwned = value;
    }

    private int GetToggleCost(ToggleUpgradeType type)
    {
        return type == ToggleUpgradeType.BackupBattery
            ? purchaseData.BackupBatteryCost
            : purchaseData.UpgradeSolCost;
    }

    private bool GetToggleEnabled(ToggleUpgradeType type)
    {
        return type == ToggleUpgradeType.BackupBattery
            ? shipData.BackupBat
            : shipData.UpgradeSol;
    }

    private void SetToggleEnabled(ToggleUpgradeType type, bool value)
    {
        if (type == ToggleUpgradeType.BackupBattery)
            shipData.BackupBat = value;
        else
            shipData.UpgradeSol = value;
    }

    // ---------- Single-Button Level Upgrades ----------

    private void OnLevelUpgrade(LevelEntry entry)
    {
        int current = GetLevel(entry.type);
        int max = GetFieldMax(entry.type);
        int owned = GetMaxOwned(entry.type);

        if (current >= max)
        {
            Debug.Log($"[SatelliteUpgradePanel] {entry.type} is already at MAX.");
            return;
        }

        int nextLevel = current + 1;

        if (nextLevel <= owned)
        {
            SetLevel(entry.type, nextLevel);
        }
        else
        {
            var costList = GetCostList(entry.type);

            if (nextLevel - 1 >= costList.Count)
            {
                Debug.LogWarning($"[SatelliteUpgradePanel] No cost defined for {entry.type} level {nextLevel}.");
                return;
            }

            int cost = costList[nextLevel - 1];

            if (shipData.Money < cost)
            {
                Debug.Log($"[SatelliteUpgradePanel] Not enough money to buy {entry.type} level {nextLevel}.");
                return;
            }

            shipData.Money -= cost;
            SetMaxOwned(entry.type, nextLevel);
            SetLevel(entry.type, nextLevel);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(shipData);
        UnityEditor.EditorUtility.SetDirty(purchaseData);
#endif

        RefreshLevel(entry);
    }

    private void RefreshLevel(LevelEntry entry)
    {
        int current = GetLevel(entry.type);
        int max = GetFieldMax(entry.type);
        int owned = GetMaxOwned(entry.type);

        if (current >= max)
        {
            entry.levelText.text = "MAX";
            return;
        }

        int nextLevel = current + 1;

        if (nextLevel <= owned)
        {
            entry.levelText.text = $"Level {current} → {nextLevel}";
        }
        else
        {
            var costList = GetCostList(entry.type);
            int cost = costList[nextLevel - 1];
            entry.levelText.text = $"Buy L{nextLevel}: {cost}";
        }
    }

    private int GetFieldMax(LevelUpgradeType type)
    {
        switch (type)
        {
            case LevelUpgradeType.Battery: return 3;
            case LevelUpgradeType.SolarPanel: return 2;
            case LevelUpgradeType.Shield: return 3;
            case LevelUpgradeType.Comm: return 2;
            default: return 0;
        }
    }

    private int GetLevel(LevelUpgradeType type)
    {
        switch (type)
        {
            case LevelUpgradeType.Battery: return shipData.BattLevel;
            case LevelUpgradeType.SolarPanel: return shipData.SolPanLevel;
            case LevelUpgradeType.Shield: return shipData.ShieldLevel;
            case LevelUpgradeType.Comm: return shipData.CommLevel;
            default: return 0;
        }
    }

    private void SetLevel(LevelUpgradeType type, int value)
    {
        switch (type)
        {
            case LevelUpgradeType.Battery: shipData.BattLevel = value; break;
            case LevelUpgradeType.SolarPanel: shipData.SolPanLevel = value; break;
            case LevelUpgradeType.Shield: shipData.ShieldLevel = value; break;
            case LevelUpgradeType.Comm: shipData.CommLevel = value; break;
        }
    }

    private int GetMaxOwned(LevelUpgradeType type)
    {
        switch (type)
        {
            case LevelUpgradeType.Battery: return purchaseData.MaxOwnedBattLevel;
            case LevelUpgradeType.SolarPanel: return purchaseData.MaxOwnedSolPanLevel;
            case LevelUpgradeType.Shield: return purchaseData.MaxOwnedShieldLevel;
            case LevelUpgradeType.Comm: return purchaseData.MaxOwnedCommLevel;
            default: return 0;
        }
    }

    private void SetMaxOwned(LevelUpgradeType type, int value)
    {
        switch (type)
        {
            case LevelUpgradeType.Battery: purchaseData.MaxOwnedBattLevel = value; break;
            case LevelUpgradeType.SolarPanel: purchaseData.MaxOwnedSolPanLevel = value; break;
            case LevelUpgradeType.Shield: purchaseData.MaxOwnedShieldLevel = value; break;
            case LevelUpgradeType.Comm: purchaseData.MaxOwnedCommLevel = value; break;
        }
    }

    private System.Collections.Generic.List<int> GetCostList(LevelUpgradeType type)
    {
        switch (type)
        {
            case LevelUpgradeType.Battery: return purchaseData.BatteryCosts.CostPerLevel;
            case LevelUpgradeType.SolarPanel: return purchaseData.SolarPanelCosts.CostPerLevel;
            case LevelUpgradeType.Shield: return purchaseData.ShieldCosts.CostPerLevel;
            case LevelUpgradeType.Comm: return purchaseData.CommCosts.CostPerLevel;
            default: return new System.Collections.Generic.List<int>();
        }
    }

    // ---------- Shared ----------

    private void RefreshAll()
    {
        foreach (var entry in toggleEntries)
            RefreshToggle(entry);

        foreach (var entry in levelEntries)
            RefreshLevel(entry);
    }
}
