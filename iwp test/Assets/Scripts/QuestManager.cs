using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public string questName = "Light the torches";
    public string description = "Solve the forest puzzle by lighting the torches in the correct order.";
    public QuestState state = QuestState.NotStarted;

    public enum QuestState { NotStarted, InProgress, Completed }

    public UIManager uiManager;

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
            uiManager.UpdateQuest(questName, "Completed! Return to the bedroom.");
        }
    }
}
