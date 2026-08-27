using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetLoader : MonoBehaviour
{
    [Header("Data")]
    public ShipUpgradeData shipData;

    public void LoadSceneByName(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    // Hook this up wherever you currently trigger the "go to active planet" flow.
    public void LoadActivePlanetScene()
    {
        if (shipData == null)
        {
            Debug.LogWarning("SceneLoader: ShipUpgradeData is not assigned.");
            return;
        }

        string sceneName = shipData.ActivePlanet.ToString();

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}