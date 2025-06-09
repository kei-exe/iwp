using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public List<int> correctSequence = new List<int> { 1, 3, 2 }; // example order
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
        Debug.Log("Wrong order! Resetting puzzle.");
        foreach (Torch t in currentSequence)
        {
            t.ResetTorch();
        }
        currentSequence.Clear();
    }

    void PuzzleComplete()
    {
        Debug.Log("Puzzle completed!");
        if (linkedQuest != null)
        {
            linkedQuest.CompleteQuest();
        }
    }
}
