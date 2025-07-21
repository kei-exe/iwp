using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private string questName = "";
    [SerializeField] private string description = "";
    public QuestState state = QuestState.NotStarted;

    public enum QuestState { NotStarted, InProgress, Completed }
    
    public UIManager uiManager;

    [SerializeField] private GameObject trigger;
    [SerializeField] private GameObject trigger2;

    private void Start()
    {
        StartQuest();
    }

    public void StartQuest()
    {
        if (state == QuestState.NotStarted)
        {
            state = QuestState.InProgress;
            uiManager.UpdateQuest(questName, description);
        }
    }

    public void CompleteQuest()
    {
        if (state == QuestState.InProgress)
        {
            state = QuestState.Completed;
            uiManager.UpdateQuest(questName, "Completed! Nice job.");

            if (trigger != null)
                trigger.SetActive(true);

            if (trigger2 != null)
                trigger2.SetActive(false);
        }
    }
}
