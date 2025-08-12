using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Sensitivity")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityValue;
    [SerializeField] private PlayerController playerController;

    [Header("Audio")]
    [SerializeField] private Slider mainSlider;
    [SerializeField] private TextMeshProUGUI mainValue;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI bgmValue;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxValue;

    void Start()
    {
        // sensitivity
        if (playerController != null)
        {
            sensitivitySlider.value = playerController.mouseSensitivity;
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            sensitivityValue.text = sensitivitySlider.value.ToString("F2") + "%";
        }

        mainSlider.value = PlayerPrefs.GetFloat("MainVolume", 0.75f);
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.75f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        mainValue.text = (mainSlider.value * 100f).ToString("F0") + "%";
        bgmValue.text = (bgmSlider.value * 100f).ToString("F0") + "%";
        sfxValue.text = (sfxSlider.value * 100f).ToString("F0") + "%";

        mainSlider.onValueChanged.AddListener(OnMainChanged);
        bgmSlider.onValueChanged.AddListener(OnBGMChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
    }

    void OnSensitivityChanged(float value)
    {
        if (playerController != null)
        {
            playerController.SetMouseSensitivity(value);
            sensitivityValue.text = value.ToString("F2") + "%";
        }
    }

    void OnMainChanged(float value)
    {
        AudioManager.Instance.SetMainVolume(value);
        mainValue.text = (value * 100f).ToString("F0") + "%";
    }

    void OnBGMChanged(float value)
    {
        AudioManager.Instance.SetBGMVolume(value);
        bgmValue.text = (value * 100f).ToString("F0") + "%";
    }

    void OnSFXChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
        sfxValue.text = (value * 100f).ToString("F0") + "%";
    }
}