using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public int health;
    public int maxHealth;
    private float stamina;
    public float maxStamina;

    public Sprite emptyHeart;
    public Sprite fullHeart;
    public Image[] hearts;
    public Slider staminaSlider;

    public TextMeshProUGUI questText;

    public PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        health = playerController.currentHealth;
        maxHealth = playerController.maxHealth;
        stamina = playerController.currentStamina;
        maxStamina = playerController.maxStamina;

        staminaSlider.value = stamina / maxStamina;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < maxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    public void UpdateQuest(string title, string message)
    {
        questText.text = $"<b>{title}</b>\n{message}";
    }

    public void Restarted()
    {
        playerController.restartPanel.SetActive(false);
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
