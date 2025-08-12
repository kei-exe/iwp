using System.Collections;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private Animator elevatorAnimator;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private float doorAnimTime = 2f;
    [SerializeField] private float moveAnimTime = 3f;
    private int currentFloor = 1;

    public void GoToFloor(int floor)
    {
        if (floor == currentFloor)
        {
            return;
        }

        StartCoroutine(HandleElevatorSequence(floor));
    }

    private IEnumerator HandleElevatorSequence(int floor)
    {
        doorAnimator.SetTrigger("CloseDoor");
        yield return new WaitForSeconds(doorAnimTime);

        string moveTrigger = $"GoToFloor{floor}";
        elevatorAnimator.SetTrigger(moveTrigger);
        yield return new WaitForSeconds(moveAnimTime);

        doorAnimator.SetTrigger("OpenDoor");

        currentFloor = floor;
    }
}