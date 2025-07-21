using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class KeypadUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText; // UI Text showing the input
    [SerializeField] private string correctCode = "7253";
    private string currentInput = "";

    [SerializeField] private GameObject keypadCanvas;
    [SerializeField] private Timeline timeline;

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

            ExitKeypad();

            // timeline
            timeline.PlayTimelineManually();
        }
        else
        {
            Debug.Log("Wrong code!");
            Invoke(nameof(ClearInput), 1f);
        }
    }
}