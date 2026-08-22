using TMPro;
using UnityEngine;
using static ShipUpgradeData;

public class PlanetSelector : MonoBehaviour
{
    [Header("Data")]
    public ShipUpgradeData shipData;
    public PurchaseData purchaseData;

    [Header("Planet GameObjects (Mercury -> Pluto order, 9 total)")]
    public GameObject[] planetObjects;

    [Header("UI")]
    public TMP_Text planetNameText;
    public GameObject PlanetUI;

    private int currentIndex;
    private bool wasPlanetUIActive;

    private void Start()
    {
        if (shipData == null || planetObjects.Length != 9)
        {
            Debug.LogWarning("PlanetSelector: Missing ShipUpgradeData or planetObjects isn't set to 9 entries.");
            return;
        }
        if (purchaseData == null)
        {
            Debug.LogWarning("PlanetSelector: PurchaseData is not assigned.");
            return;
        }

        currentIndex = (int)shipData.ActivePlanet;
        // If the saved active planet isn't actually owned (or isn't Earth, the free default),
        // fall back to Earth if owned, otherwise the first owned planet found.
        if (!IsOwned((Planet)currentIndex))
        {
            currentIndex = IsOwned(Planet.Earth) ? (int)Planet.Earth : FindFirstOwnedIndex();
        }

        wasPlanetUIActive = PlanetUI.activeSelf;
        RefreshVisibility();
    }

    private void Update()
    {
        bool isActiveNow = PlanetUI.activeSelf;

        // Only react when the UI's active state actually changes, rather than
        // redoing this every single frame regardless of change.
        if (isActiveNow != wasPlanetUIActive)
        {
            wasPlanetUIActive = isActiveNow;
            RefreshVisibility();
        }
    }

    private void RefreshVisibility()
    {
        if (!PlanetUI.activeSelf)
        {
            // UI closed: all planets off, regardless of which was selected.
            for (int i = 0; i < planetObjects.Length; i++)
            {
                if (planetObjects[i] != null)
                    planetObjects[i].SetActive(false);
            }
        }
        else
        {
            // UI open: show whichever planet is currently selected.
            ShowPlanet(currentIndex);
        }
    }

    // Hook this up to your "Left" UI Button's OnClick
    public void ScrollLeft()
    {
        int nextIndex = FindNextOwnedIndex(-1);
        if (nextIndex == -1) return; // no owned planet in that direction
        currentIndex = nextIndex;
        ShowPlanet(currentIndex);
    }

    // Hook this up to your "Right" UI Button's OnClick
    public void ScrollRight()
    {
        int nextIndex = FindNextOwnedIndex(1);
        if (nextIndex == -1) return; // no owned planet in that direction
        currentIndex = nextIndex;
        ShowPlanet(currentIndex);
    }

    // Searches from currentIndex in the given direction (-1 or 1) for the next owned planet.
    // Earth (index 2) is always treated as owned, same as the free default colour.
    private int FindNextOwnedIndex(int direction)
    {
        int index = currentIndex;
        while (true)
        {
            index += direction;
            if (index < 0 || index >= planetObjects.Length)
                return -1; // hit the end of the array with nothing owned
            if (IsOwned((Planet)index))
                return index;
        }
    }

    private int FindFirstOwnedIndex()
    {
        for (int i = 0; i < planetObjects.Length; i++)
        {
            if (IsOwned((Planet)i))
                return i;
        }
        Debug.LogWarning("PlanetSelector: No owned planets found, defaulting to Earth.");
        return (int)Planet.Earth;
    }

    private bool IsOwned(Planet planet)
    {
        if (planet == Planet.Earth)
            return true; // Earth is always available, same as the free default colour
        return purchaseData.PurchasedPlanets.Contains(planet);
    }

    private void ShowPlanet(int index)
    {
        for (int i = 0; i < planetObjects.Length; i++)
        {
            if (planetObjects[i] != null)
                planetObjects[i].SetActive(i == index);
        }
        shipData.ActivePlanet = (Planet)index;
        if (planetNameText != null)
            planetNameText.text = shipData.ActivePlanet.ToString();
    }
}