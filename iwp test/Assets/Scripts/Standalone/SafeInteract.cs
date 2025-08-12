using UnityEngine;

public class SafeInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject keypadCanvas;
    [SerializeField] private PlayerController playerController;

    public void OnInteract()
    {
        if (keypadCanvas != null)
        {
            keypadCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;

            if (playerController != null)
                playerController.isLookLocked = true;
        }
    }
}