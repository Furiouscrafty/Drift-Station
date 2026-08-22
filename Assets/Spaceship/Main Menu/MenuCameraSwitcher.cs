using UnityEngine;

public class MenuCameraSwitcher : MonoBehaviour
{
    [Header("Menu A")]
    [SerializeField] public GameObject menuA;
    [SerializeField] public Camera cameraA;

    [Header("Menu B")]
    [SerializeField] public GameObject menuB;
    [SerializeField] public Camera cameraB;

    private void Start()
    {
        // Start on Menu A by default — adjust if needed
        ShowMenuA();
    }

    // Hook this up to Button A's OnClick()
    public void ShowMenuB()
    {
        menuA.SetActive(false);
        cameraA.gameObject.SetActive(false);

        menuB.SetActive(true);
        cameraB.gameObject.SetActive(true);
    }

    // Hook this up to Button B's OnClick()
    public void ShowMenuA()
    {
        menuB.SetActive(false);
        cameraB.gameObject.SetActive(false);

        menuA.SetActive(true);
        cameraA.gameObject.SetActive(true);
    }
}