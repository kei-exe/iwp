using UnityEngine;

public class SafeInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject keypadCanvas;

    public void OnInteract()
    {
        // Show the keypad UI
        if (keypadCanvas != null)
        {
            keypadCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
    }
}