using UnityEngine;
using UnityEngine.SceneManagement;

public class BedInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private string targetSceneName;
    private QuestManager questManager;

    public void OnInteract()
    {
        //if (questManager.state != QuestManager.QuestState.Completed) return;
        SceneManager.LoadScene(targetSceneName);
    }
}