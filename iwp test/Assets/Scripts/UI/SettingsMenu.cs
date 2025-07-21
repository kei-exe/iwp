using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityValue;
    [SerializeField] private PlayerController playerController;

    void Start()
    {
        // Set slider's default to current player sensitivity
        if (playerController != null)
        {
            sensitivitySlider.value = playerController.mouseSensitivity;
        }

        // Add listener
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        sensitivityValue.text = sensitivitySlider.value.ToString("F2") + "%";
    }

    void OnSensitivityChanged(float value)
    {
        if (playerController != null)
        {
            playerController.SetMouseSensitivity(value);
            sensitivityValue.text = sensitivitySlider.value.ToString("F2") + "%";
        }
    }
}