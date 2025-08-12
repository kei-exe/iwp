using UnityEngine;

public class Torch : MonoBehaviour, IInteractable
{
    public int torchIndex; // index in sequence
    public bool isLit = false;

    private PuzzleManager manager;
    [SerializeField] private GameObject fireParticle;
    [SerializeField] private GameObject interactionCanvas;

    private void Start()
    {
        manager = FindFirstObjectByType<PuzzleManager>();
        interactionCanvas.SetActive(false);
    }

    public void OnInteract()
    {
        if (!isLit)
        {
            isLit = true;
            fireParticle.SetActive(true);
            manager.RegisterTorch(this);
        }
    }

    public void ResetTorch()
    {
        isLit = false;
        fireParticle.SetActive(false);
    }

    public void ShowPrompt()
    {
        interactionCanvas.SetActive(true);
    }

    public void HidePrompt()
    {
        interactionCanvas.SetActive(false);
    }
}