using UnityEngine;

public class SimpleMenuSwitcher : MonoBehaviour
{
    [Header("Assign the menus")]
    public GameObject menuToClose;
    public GameObject menuToOpen;

    public void SwitchMenu()
    {
        if (menuToClose != null)
            menuToClose.SetActive(false);

        if (menuToOpen != null)
            menuToOpen.SetActive(true);
    }
}
