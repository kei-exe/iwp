using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScript : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    private bool isSettingsOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && settingsPanel != null)
        {
            ToggleSettings();
        }
    }

    void ToggleSettings()
    {
        isSettingsOpen = !isSettingsOpen;
        settingsPanel.SetActive(isSettingsOpen);

        if (isSettingsOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f; // Pause game (optional)
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }

    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void Settings()
    {
        if (settingsPanel != null && !settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(true);
        }

    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void End()
    {
        UnityEngine.Application.Quit();
    }

    private void OnTriggerEnter(Collider other)
    {
        LoadScene("Main");
    }
}
