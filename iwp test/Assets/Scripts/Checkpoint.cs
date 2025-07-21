using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject arrow;               // The arrow above this checkpoint
    public Checkpoint nextCheckpoint;      // The next checkpoint in the course

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Set this as the latest checkpoint
                player.UpdateCheckpoint(transform.position);

                // Turn off this arrow
                if (arrow != null)
                    arrow.SetActive(false);

                // Turn on next arrow
                if (nextCheckpoint != null && nextCheckpoint.arrow != null)
                    nextCheckpoint.arrow.SetActive(true);

                triggered = true;
            }
        }
    }
}