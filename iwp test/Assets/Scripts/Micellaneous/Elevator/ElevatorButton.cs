using UnityEngine;

public class ElevatorButton : MonoBehaviour, IInteractable
{
    [SerializeField] private ElevatorController elevator;
    [SerializeField] private int targetFloor = 2;

    public void OnInteract()
    {
        elevator.GoToFloor(targetFloor);
    }
}