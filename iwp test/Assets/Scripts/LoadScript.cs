using UnityEngine;
using UnityEngine.Playables;
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
            Time.timeScale = 0f;
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
        SceneManager.LoadScene(sceneName);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void End()
    {
        Application.Quit();
    }

    private void OnTriggerEnter(Collider other)
    {
        LoadScene("Main");
    }
}
