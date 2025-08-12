using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public List<int> correctSequence = new List<int> { 1, 3, 2 };
    private List<Torch> currentSequence = new List<Torch>();

    public QuestManager linkedQuest;

    public void RegisterTorch(Torch torch)
    {
        currentSequence.Add(torch);

        int i = currentSequence.Count - 1;
        if (torch.torchIndex != correctSequence[i])
        {
            ResetPuzzle();
            return;
        }

        if (currentSequence.Count == correctSequence.Count)
        {
            PuzzleComplete();
        }
    }

    void ResetPuzzle()
    {
        foreach (Torch t in currentSequence)
        {
            t.ResetTorch();
        }
        currentSequence.Clear();
    }

    void PuzzleComplete()
    {
        if (linkedQuest != null)
        {
            linkedQuest.CompleteQuest();
        }
    }
}
