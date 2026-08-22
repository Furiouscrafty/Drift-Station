using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeLevelUI : MonoBehaviour
{
    public enum UpgradeType { Battery, SolarPanel, Shield, Comm }

    [Header("UI References")]
    [SerializeField] private Button downButton;
    [SerializeField] private Button upButton;
    [SerializeField] private TMP_Text levelText;

    [Header("Target")]
    [SerializeField] private ShipUpgradeData shipData;
    [SerializeField] private PurchaseData purchaseData;
    [SerializeField] private UpgradeType upgradeType;

    private int _minLevel = 0;
    private int _maxLevel;



    private void OnEnable()
    {
        _maxLevel = GetMaxLevel();
        downButton.onClick.AddListener(DecreaseLevel);
        upButton.onClick.AddListener(IncreaseLevel);
        RefreshDisplay();
    }

    private void OnDisable()
    {
        downButton.onClick.RemoveListener(DecreaseLevel);
        upButton.onClick.RemoveListener(IncreaseLevel);
    }

    private void DecreaseLevel()
    {
        // Moving down is always free - you already own this level
        int current = GetLevel();
        current = Mathf.Clamp(current - 1, _minLevel, _maxLevel);
        SetLevel(current);
        RefreshDisplay();
    }

    private void IncreaseLevel()
    {
        int current = GetLevel();
        int maxOwned = GetMaxOwnedLevel();

        if (current >= _maxLevel)
            return; // already at the absolute cap

        if (current < maxOwned)
        {
            // Already paid for this level before - just re-equip it, no charge
            SetLevel(current + 1);
            RefreshDisplay();
            return;
        }

        // Needs to be purchased - current == maxOwned here
        int cost = GetCostForLevel(current);
        if (shipData.Money < cost)
        {
            Debug.Log($"UpgradeLevelUI: Not enough money to upgrade {upgradeType} (needs {cost}).");
            return;
        }

        shipData.Money -= cost;
        SetMaxOwnedLevel(current + 1);
        SetLevel(current + 1);
        RefreshDisplay();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(purchaseData);
#endif
    }

    private void RefreshDisplay()
    {
        levelText.text = GetLevel().ToString();
    }

    private int GetMaxLevel()
    {
        switch (upgradeType)
        {
            case UpgradeType.Battery: return 3;
            case UpgradeType.SolarPanel: return 2;
            case UpgradeType.Shield: return 3;
            case UpgradeType.Comm: return 2;
            default: return 0;
        }
    }

    private int GetLevel()
    {
        switch (upgradeType)
        {
            case UpgradeType.Battery: return shipData.BattLevel;
            case UpgradeType.SolarPanel: return shipData.SolPanLevel;
            case UpgradeType.Shield: return shipData.ShieldLevel;
            case UpgradeType.Comm: return shipData.CommLevel;
            default: return 0;
        }
    }

    private void SetLevel(int value)
    {
        switch (upgradeType)
        {
            case UpgradeType.Battery: shipData.BattLevel = value; break;
            case UpgradeType.SolarPanel: shipData.SolPanLevel = value; break;
            case UpgradeType.Shield: shipData.ShieldLevel = value; break;
            case UpgradeType.Comm: shipData.CommLevel = value; break;
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(shipData);
#endif
    }

    private int GetMaxOwnedLevel()
    {
        switch (upgradeType)
        {
            case UpgradeType.Battery: return purchaseData.MaxOwnedBattLevel;
            case UpgradeType.SolarPanel: return purchaseData.MaxOwnedSolPanLevel;
            case UpgradeType.Shield: return purchaseData.MaxOwnedShieldLevel;
            case UpgradeType.Comm: return purchaseData.MaxOwnedCommLevel;
            default: return 0;
        }
    }

    private void SetMaxOwnedLevel(int value)
    {
        switch (upgradeType)
        {
            case UpgradeType.Battery: purchaseData.MaxOwnedBattLevel = value; break;
            case UpgradeType.SolarPanel: purchaseData.MaxOwnedSolPanLevel = value; break;
            case UpgradeType.Shield: purchaseData.MaxOwnedShieldLevel = value; break;
            case UpgradeType.Comm: purchaseData.MaxOwnedCommLevel = value; break;
        }
    }

    private int GetCostForLevel(int currentLevel)
    {
        var costList = GetCostList();
        if (costList == null || currentLevel >= costList.CostPerLevel.Count)
        {
            Debug.LogWarning($"UpgradeLevelUI: No cost defined for {upgradeType} at level {currentLevel}.");
            return int.MaxValue; // block the purchase rather than allow it for free
        }
        return costList.CostPerLevel[currentLevel];
    }

    private PurchaseData.LevelUpgradeCost GetCostList()
    {
        switch (upgradeType)
        {
            case UpgradeType.Battery: return purchaseData.BatteryCosts;
            case UpgradeType.SolarPanel: return purchaseData.SolarPanelCosts;
            case UpgradeType.Shield: return purchaseData.ShieldCosts;
            case UpgradeType.Comm: return purchaseData.CommCosts;
            default: return null;
        }
    }
}