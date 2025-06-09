using UnityEngine;

public class Torch : MonoBehaviour, IInteractable
{
    public int torchIndex; // index in sequence
    public bool isLit = false;

    private PuzzleManager manager;
    [SerializeField] private GameObject fireParticle;

    private void Start()
    {
        manager = FindObjectOfType<PuzzleManager>();
    }

    public void OnInteract()
    {
        if (!isLit)
        {
            isLit = true;
            // TODO: turn on fire effect
            fireParticle.SetActive(true);
            manager.RegisterTorch(this);

            Debug.Log("lit");
        }

        Debug.Log("lit broken");
    }

    public void ResetTorch()
    {
        isLit = false;
        // TODO: turn off fire effect
        fireParticle.SetActive(false);
        Debug.Log("unlit");
    }
}