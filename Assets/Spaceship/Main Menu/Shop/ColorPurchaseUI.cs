using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorPurchaseUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private TMP_Text buyButtonText;
    [SerializeField] private Image previewImage;

    [Header("Data")]
    [SerializeField] private PurchaseData purchaseData;
    [SerializeField] private ShipUpgradeData shipData;

    private int _currentIndex = 0;

    private void OnEnable()
    {
        previousButton.onClick.AddListener(SelectPrevious);
        nextButton.onClick.AddListener(SelectNext);
        buyButton.onClick.AddListener(BuyCurrentColor);

        _currentIndex = 0;
        RefreshDisplay();
    }

    private void OnDisable()
    {
        previousButton.onClick.RemoveListener(SelectPrevious);
        nextButton.onClick.RemoveListener(SelectNext);
        buyButton.onClick.RemoveListener(BuyCurrentColor);
    }

    private void SelectPrevious()
    {
        if (purchaseData.UnpurchasedColours.Count == 0) return;

        _currentIndex--;
        if (_currentIndex < 0)
            _currentIndex = purchaseData.UnpurchasedColours.Count - 1;

        RefreshDisplay();
    }

    private void SelectNext()
    {
        if (purchaseData.UnpurchasedColours.Count == 0) return;

        _currentIndex++;
        if (_currentIndex >= purchaseData.UnpurchasedColours.Count)
            _currentIndex = 0;

        RefreshDisplay();
    }

    private void BuyCurrentColor()
    {
        if (purchaseData.UnpurchasedColours.Count == 0) return;

        int cost = purchaseData.ColourCost;
        if (shipData.Money < cost)
        {
            Debug.Log("[ColorPurchaseUI] Not enough money to buy this colour.");
            return;
        }

        Color chosen = purchaseData.UnpurchasedColours[_currentIndex];

        // Move colour from Unpurchased to Purchased
        purchaseData.UnpurchasedColours.RemoveAt(_currentIndex);
        purchaseData.PurchasedColours.Add(chosen);

        // Deduct cost
        shipData.Money -= cost;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(purchaseData);
        UnityEditor.EditorUtility.SetDirty(shipData);
#endif

        // Clamp index in case the list shrank past it, then refresh
        if (_currentIndex >= purchaseData.UnpurchasedColours.Count)
            _currentIndex = Mathf.Max(0, purchaseData.UnpurchasedColours.Count - 1);

        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        bool hasColours = purchaseData.UnpurchasedColours.Count > 0;

        previewImage.enabled = hasColours;
        buyButton.interactable = hasColours;
        previousButton.interactable = hasColours;
        nextButton.interactable = hasColours;

        if (!hasColours)
        {
            buyButtonText.text = "Sold Out";
            return;
        }

        Color current = purchaseData.UnpurchasedColours[_currentIndex];
        previewImage.color = current;
        buyButtonText.text = "Buy: " + purchaseData.ColourCost;
    }
}