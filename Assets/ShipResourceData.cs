using UnityEngine;

[CreateAssetMenu(fileName = "NewShipResourceData", menuName = "Game/Ship Resource Data")]
public class ShipResourceData : ScriptableObject
{
    [Header("Resources")]
    public float Power = 100;
    public float ShieldHealth = 100;
    public float HullHealth = 100;
    public float Heat;
    public bool ishit;

    [Header("Orbits")]
    public int CurrentOrbits;
    public int HighestNumOrbit;

    public void RegisterOrbitCompleted()
    {
        CurrentOrbits++;
        if (CurrentOrbits > HighestNumOrbit)
            HighestNumOrbit = CurrentOrbits;
    }

    private void OnValidate()
    {
        CurrentOrbits = Mathf.Max(CurrentOrbits, 0);
        HighestNumOrbit = Mathf.Max(HighestNumOrbit, 0);
    }
}