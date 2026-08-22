using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlanetPurchaseUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private TMP_Text buyButtonText;
    [SerializeField] private RawImage previewImage; // displays the camera's RenderTexture

    [Header("Data")]
    [SerializeField] private PurchaseData purchaseData;
    [SerializeField] private ShipUpgradeData shipData;

    [Header("Scene Planet Objects")]
    [Tooltip("Scene instances shown to the camera, in the SAME order as PurchaseData.UnpurchasedPlanets.")]
    [SerializeField] private GameObject[] unpurchasedPlanetObjects;

    private int _currentIndex = 0;

    private void OnEnable()
    {
        previousButton.onClick.AddListener(SelectPrevious);
        nextButton.onClick.AddListener(SelectNext);
        buyButton.onClick.AddListener(BuyCurrentPlanet);

        _currentIndex = 0;
        RefreshDisplay();
    }

    private void OnDisable()
    {
        previousButton.onClick.RemoveListener(SelectPrevious);
        nextButton.onClick.RemoveListener(SelectNext);
        buyButton.onClick.RemoveListener(BuyCurrentPlanet);
    }

    private void SelectPrevious()
    {
        if (purchaseData.UnpurchasedPlanets.Count == 0) return;

        _currentIndex--;
        if (_currentIndex < 0)
            _currentIndex = purchaseData.UnpurchasedPlanets.Count - 1;

        RefreshDisplay();
    }

    private void SelectNext()
    {
        if (purchaseData.UnpurchasedPlanets.Count == 0) return;

        _currentIndex++;
        if (_currentIndex >= purchaseData.UnpurchasedPlanets.Count)
            _currentIndex = 0;

        RefreshDisplay();
    }

    private void BuyCurrentPlanet()
    {
        if (purchaseData.UnpurchasedPlanets.Count == 0) return;

        var option = purchaseData.UnpurchasedPlanets[_currentIndex];
        int cost = option.Cost;

        if (shipData.Money < cost)
        {
            Debug.Log("[PlanetPurchaseUI] Not enough money to buy this planet.");
            return;
        }

        // Move planet from Unpurchased to Purchased
        purchaseData.UnpurchasedPlanets.RemoveAt(_currentIndex);
        purchaseData.PurchasedPlanets.Add(option.Planet);

        // Keep the scene object list in sync with the data list
        var objectList = new System.Collections.Generic.List<GameObject>(unpurchasedPlanetObjects);
        if (_currentIndex < objectList.Count)
            objectList.RemoveAt(_currentIndex);
        unpurchasedPlanetObjects = objectList.ToArray();

        // Deduct cost
        shipData.Money -= cost;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(purchaseData);
        UnityEditor.EditorUtility.SetDirty(shipData);
#endif

        if (_currentIndex >= purchaseData.UnpurchasedPlanets.Count)
            _currentIndex = Mathf.Max(0, purchaseData.UnpurchasedPlanets.Count - 1);

        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        bool hasPlanets = purchaseData.UnpurchasedPlanets.Count > 0;

        buyButton.interactable = hasPlanets;
        previousButton.interactable = hasPlanets;
        nextButton.interactable = hasPlanets;

        // Disable all planet objects, then enable only the one being viewed
        for (int i = 0; i < unpurchasedPlanetObjects.Length; i++)
        {
            if (unpurchasedPlanetObjects[i] != null)
                unpurchasedPlanetObjects[i].SetActive(i == _currentIndex && hasPlanets);
        }

        if (!hasPlanets)
        {
            buyButtonText.text = "Sold Out";
            return;
        }

        buyButtonText.text = "Buy: " + purchaseData.UnpurchasedPlanets[_currentIndex].Cost;
    }
}