using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeypadUI : MonoBehaviour
{
    public TextMeshProUGUI displayText; // UI Text showing the input
    public string correctCode = "7253";
    private string currentInput = "";

    public GameObject keypadCanvas;
    public GameObject cutsceneTriggerObject; // Assign a Timeline or Animation Trigger

    public void PressButton(string number)
    {
        if (currentInput.Length >= 4) return;

        currentInput += number;
        displayText.text = currentInput;

        if (currentInput.Length == 4)
        {
            CheckCode();
        }
    }

    public void ClearInput()
    {
        currentInput = "";
        displayText.text = "";
    }

    public void ExitKeypad()
    {
        keypadCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        ClearInput();
    }

    void CheckCode()
    {
        if (currentInput == correctCode)
        {
            Debug.Log("Correct code!");

            // Hide UI
            ExitKeypad();

            // Trigger cutscene
            if (cutsceneTriggerObject != null)
            {
                cutsceneTriggerObject.GetComponent<Animator>()?.SetTrigger("Play");
            }
        }
        else
        {
            Debug.Log("Wrong code!");
            Invoke(nameof(ClearInput), 1f);
        }
    }
}