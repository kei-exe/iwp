using UnityEngine;
using UnityEngine.SceneManagement;

public class WallsClosingIn : MonoBehaviour
{
    [SerializeField] private Transform leftWall;
    [SerializeField] private Transform rightWall;
    [SerializeField] private Transform frontWall;
    [SerializeField] private Transform backWall;
    [SerializeField] private GameObject hole;

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float targetDistance = 1f;

    [SerializeField] private PlayerController playerController;

    private bool startClosing = false;
    private bool hasTriggeredLose = false;

    private Vector3 leftStart, rightStart, frontStart, backStart;

    void Start()
    {
        leftStart = leftWall.position;
        rightStart = rightWall.position;
        frontStart = frontWall.position;
        backStart = backWall.position;
    }

    void Update()
    {
        if (startClosing)
        {
            leftWall.position = Vector3.MoveTowards(leftWall.position, leftStart + Vector3.right * targetDistance, moveSpeed * Time.deltaTime);
            rightWall.position = Vector3.MoveTowards(rightWall.position, rightStart + Vector3.left * targetDistance, moveSpeed * Time.deltaTime);
            frontWall.position = Vector3.MoveTowards(frontWall.position, frontStart + Vector3.back * targetDistance, moveSpeed * Time.deltaTime);
            backWall.position = Vector3.MoveTowards(backWall.position, backStart + Vector3.forward * targetDistance, moveSpeed * Time.deltaTime);

            float horizontalDist = Vector3.Distance(leftWall.position, rightWall.position);
            float verticalDist = Vector3.Distance(frontWall.position, backWall.position);

            if (!hasTriggeredLose && horizontalDist <= targetDistance && verticalDist <= targetDistance)
            {
                TriggerLose();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startClosing = true;
            hole.SetActive(true);
            GetComponent<Collider>().enabled = false;
        }
    }

    private void TriggerLose()
    {
        hasTriggeredLose = true;
        Debug.Log("Player got squashed!");
        playerController.TakeDamage(1);
    }

    public void ResetWalls()
    {
        leftWall.position = leftStart;
        rightWall.position = rightStart;
        frontWall.position = frontStart;
        backWall.position = backStart;

        startClosing = false;
        hasTriggeredLose = false;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        if (hole != null)
            hole.SetActive(false);
        Debug.Log("Walls reset!");
    }
}