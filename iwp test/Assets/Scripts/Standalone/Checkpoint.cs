using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject arrow;
    public Checkpoint nextCheckpoint;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.UpdateCheckpoint(transform.position);

                if (arrow != null)
                    arrow.SetActive(false);

                if (nextCheckpoint != null && nextCheckpoint.arrow != null)
                    nextCheckpoint.arrow.SetActive(true);

                triggered = true;
            }
        }
    }
}