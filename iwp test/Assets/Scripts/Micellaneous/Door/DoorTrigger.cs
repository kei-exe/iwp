using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private Door door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            if (!door.isOpen)
            {
                door.Open(other.transform.position);
                Debug.Log("door triggered");
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            if (door.isOpen)
            {
                door.Close();
                Debug.Log("door left");
            }
        }
    }
}
