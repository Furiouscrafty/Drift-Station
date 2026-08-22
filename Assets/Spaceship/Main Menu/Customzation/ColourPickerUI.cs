using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ColorPickerUI : MonoBehaviour
{
    public enum ShipPart { Hull, HubConnector, Connector, Ring, Shield }

    [Header("UI References")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Image previewImage;

    [Header("Transparency (Shield only)")]
    [Tooltip("Only used when part is Shield. Developer-set alpha (0–255).")]
    [SerializeField] private float shieldTransparency255 = 255f;

    [Header("Target")]
    [SerializeField] private ShipUpgradeData shipData;
    [SerializeField] private ShipPart part;

    [Header("Palette")]
    [Tooltip("Element 0 is the default colour, always available for free. Remaining selectable colours come from PurchaseData.PurchasedColours at runtime.")]
    [SerializeField] private Color[] presetColors;
    [SerializeField] private PurchaseData purchaseData;

    private List<Color> _availableColors = new List<Color>();
    private int _currentIndex = 0;

    private void OnEnable()
    {
        previousButton.onClick.AddListener(SelectPrevious);
        nextButton.onClick.AddListener(SelectNext);
        confirmButton.onClick.AddListener(ConfirmSelection);

        BuildAvailableColors();
        _currentIndex = GetStartingIndex();

        // Load saved shield alpha into the developer field
        if (part == ShipPart.Shield)
            shieldTransparency255 = shipData.ShieldColour.a * 255f;

        ApplyColor(_currentIndex);
    }

    private void OnDisable()
    {
        previousButton.onClick.RemoveListener(SelectPrevious);
        nextButton.onClick.RemoveListener(SelectNext);
        confirmButton.onClick.RemoveListener(ConfirmSelection);
    }

    private void BuildAvailableColors()
    {
        _availableColors.Clear();

        if (presetColors.Length == 0)
        {
            Debug.LogWarning("ColorPickerUI: presetColors is empty, needs at least a default colour at index 0.");
            return;
        }

        _availableColors.Add(presetColors[0]);

        if (purchaseData != null)
        {
            foreach (Color owned in purchaseData.PurchasedColours)
                _availableColors.Add(owned);
        }
        else
        {
            Debug.LogWarning("ColorPickerUI: PurchaseData is not assigned, only the default colour will be available.");
        }
    }

    private int GetStartingIndex()
    {
        Color savedColor = GetSavedColor();
        for (int i = 0; i < _availableColors.Count; i++)
        {
            if ((Vector3)(Vector4)_availableColors[i] == (Vector3)(Vector4)savedColor)
                return i;
        }
        return 0;
    }

    private void SelectPrevious()
    {
        if (_availableColors.Count == 0) return;

        _currentIndex--;
        if (_currentIndex < 0)
            _currentIndex = _availableColors.Count - 1;

        ApplyColor(_currentIndex);
    }

    private void SelectNext()
    {
        if (_availableColors.Count == 0) return;

        _currentIndex++;
        if (_currentIndex >= _availableColors.Count)
            _currentIndex = 0;

        ApplyColor(_currentIndex);
    }

    private void ApplyColor(int index)
    {
        Color chosen = _availableColors[index];

        if (part == ShipPart.Shield)
            chosen.a = Mathf.Clamp01(shieldTransparency255 / 255f);
        else
            chosen.a = 1f;

        Material targetMaterial = GetTargetMaterial();
        targetMaterial.color = chosen;

        if (previewImage != null)
            previewImage.color = chosen;
    }

    private void ConfirmSelection()
    {
        Color chosen = _availableColors[_currentIndex];

        if (part == ShipPart.Shield)
            chosen.a = Mathf.Clamp01(shieldTransparency255 / 255f);
        else
            chosen.a = 1f;

        SetSavedColor(chosen);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(shipData);
#endif
    }

    private Color GetSavedColor()
    {
        switch (part)
        {
            case ShipPart.Hull: return shipData.HullColour;
            case ShipPart.HubConnector: return shipData.HubConnectorColour;
            case ShipPart.Connector: return shipData.ConnectorColour;
            case ShipPart.Ring: return shipData.RingColour;
            case ShipPart.Shield: return shipData.ShieldColour;
            default: return Color.white;
        }
    }

    private void SetSavedColor(Color color)
    {
        switch (part)
        {
            case ShipPart.Hull: shipData.HullColour = color; break;
            case ShipPart.HubConnector: shipData.HubConnectorColour = color; break;
            case ShipPart.Connector: shipData.ConnectorColour = color; break;
            case ShipPart.Ring: shipData.RingColour = color; break;
            case ShipPart.Shield: shipData.ShieldColour = color; break;
        }
    }

    private Material GetTargetMaterial()
    {
        switch (part)
        {
            case ShipPart.Hull: return shipData.HullMaterial;
            case ShipPart.HubConnector: return shipData.HubConnectorMaterial;
            case ShipPart.Connector: return shipData.ConnectorMaterial;
            case ShipPart.Ring: return shipData.RingMaterial;
            case ShipPart.Shield: return shipData.ShieldMaterial;
            default: return null;
        }
    }
}
