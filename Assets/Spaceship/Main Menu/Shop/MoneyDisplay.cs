using UnityEngine;
using TMPro;

public class MoneyDisplay : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ShipUpgradeData shipData;

    [Header("UI")]
    [SerializeField] private TMP_Text moneyText;

    private void Update()
    {
        if (shipData != null && moneyText != null)
        {
            moneyText.text = "Money: " + shipData.Money;
        }
    }
}
