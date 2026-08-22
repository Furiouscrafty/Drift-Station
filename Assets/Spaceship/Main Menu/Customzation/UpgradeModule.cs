using UnityEngine;
using UnityEngine.UI;

public class UpgradeToggleUI : MonoBehaviour
{
    public enum ToggleType { UpgradeSolarPanel, BackupBattery }

    [Header("UI References")]
    [SerializeField] private Toggle toggle;

    [Header("Target")]
    [SerializeField] private ShipUpgradeData shipData;
    [SerializeField] private PurchaseData purchaseData;
    [SerializeField] private ToggleType toggleType;

    private void OnEnable()
    {
        toggle.isOn = GetValue(); // reflect saved state when this UI appears
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnDisable()
    {
        toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool value)
    {
        if (!value)
        {
            // Turning off is always free - no purchase needed either way
            SetValue(false);
            return;
        }

        // Turning on
        if (GetOwned())
        {
            // Already purchased - just flip it on, no charge
            SetValue(true);
            return;
        }

        // Not owned yet - attempt to purchase
        int cost = GetCost();
        if (shipData.Money < cost)
        {
            Debug.Log($"UpgradeToggleUI: Not enough money to purchase {toggleType} (needs {cost}).");
            toggle.SetIsOnWithoutNotify(false); // revert the visual toggle, purchase failed
            return;
        }

        shipData.Money -= cost;
        SetOwned(true);
        SetValue(true);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(purchaseData);
#endif
    }

    private bool GetValue()
    {
        switch (toggleType)
        {
            case ToggleType.UpgradeSolarPanel: return shipData.UpgradeSol;
            case ToggleType.BackupBattery: return shipData.BackupBat;
            default: return false;
        }
    }

    private void SetValue(bool value)
    {
        switch (toggleType)
        {
            case ToggleType.UpgradeSolarPanel: shipData.UpgradeSol = value; break;
            case ToggleType.BackupBattery: shipData.BackupBat = value; break;
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(shipData);
#endif
    }

    private bool GetOwned()
    {
        switch (toggleType)
        {
            case ToggleType.UpgradeSolarPanel: return purchaseData.UpgradeSolOwned;
            case ToggleType.BackupBattery: return purchaseData.BackupBatteryOwned;
            default: return false;
        }
    }

    private void SetOwned(bool value)
    {
        switch (toggleType)
        {
            case ToggleType.UpgradeSolarPanel: purchaseData.UpgradeSolOwned = value; break;
            case ToggleType.BackupBattery: purchaseData.BackupBatteryOwned = value; break;
        }
    }

    private int GetCost()
    {
        switch (toggleType)
        {
            case ToggleType.UpgradeSolarPanel: return purchaseData.UpgradeSolCost;
            case ToggleType.BackupBattery: return purchaseData.BackupBatteryCost;
            default: return int.MaxValue;
        }
    }
}