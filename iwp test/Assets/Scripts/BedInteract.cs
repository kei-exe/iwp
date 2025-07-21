using UnityEngine;
using UnityEngine.SceneManagement;

public class BedInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private string targetSceneName;
    [SerializeField] private Timeline timeline;

    public void OnInteract()
    {
        if (gameObject.CompareTag("B2"))
        {
            timeline.PlayTimelineManually();
        }
        else
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        timeline.PlayTimelineManually();
    }
}