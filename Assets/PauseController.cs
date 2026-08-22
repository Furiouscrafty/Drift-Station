using UnityEngine;

public class PauseController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject playerUI;
    public GameObject pauseMenuUI;
    public GameObject Lazar;

    private bool isPaused = false;

    private void Start()
    {
        Lazar.SetActive(true);
        playerUI.SetActive(true);
        pauseMenuUI.SetActive(false);
    }
    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Lazar.SetActive(false);

        if (playerUI != null)
            playerUI.SetActive(false);

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
    }

    public void Resume()
    {
        Lazar.SetActive(true);
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (playerUI != null)
            playerUI.SetActive(true);
    }
}
