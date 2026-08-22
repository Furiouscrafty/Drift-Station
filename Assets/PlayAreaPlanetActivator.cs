using UnityEngine;

public class PlayAreaPlanetActivator : MonoBehaviour
{
    [Header("Data")]
    public ShipUpgradeData shipData;

    [Header("In-Game Planet Objects (Mercury -> Pluto order, 9 total)")]
    public GameObject[] planetObjects;

    private void Start()
    {
        ActivateCurrentPlanet();
    }

    public void ActivateCurrentPlanet()
    {
        if (shipData == null || planetObjects.Length != 9)
        {
            Debug.LogWarning("PlayAreaPlanetActivator: Missing ShipUpgradeData or planetObjects isn't set to 9 entries.");
            return;
        }

        int index = (int)shipData.ActivePlanet;

        for (int i = 0; i < planetObjects.Length; i++)
        {
            if (planetObjects[i] != null)
                planetObjects[i].SetActive(i == index);
        }
    }
}
