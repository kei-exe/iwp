using UnityEngine;

public class WallsClosingIn : MonoBehaviour
{
    public Transform leftWall;
    public Transform rightWall;
    public Transform frontWall;
    public Transform backWall;
    public Transform ceiling;
    public float moveSpeed = 1f;
    public float targetDistance = 1f;

    private bool startClosing = false;

    private Vector3 leftStart, rightStart, frontStart, backStart, ceilingStart;

    void Start()
    {
        leftStart = leftWall.position;
        rightStart = rightWall.position;
        frontStart = frontWall.position;
        backStart = backWall.position;
        ceilingStart = ceiling.position;
    }

    void Update()
    {
        if (startClosing)
        {
            leftWall.position = Vector3.MoveTowards(leftWall.position, leftStart + Vector3.right * targetDistance, moveSpeed * Time.deltaTime);
            rightWall.position = Vector3.MoveTowards(rightWall.position, rightStart + Vector3.left * targetDistance, moveSpeed * Time.deltaTime);
            frontWall.position = Vector3.MoveTowards(frontWall.position, frontStart + Vector3.back * targetDistance, moveSpeed * Time.deltaTime);
            backWall.position = Vector3.MoveTowards(backWall.position, backStart + Vector3.forward * targetDistance, moveSpeed * Time.deltaTime);
            ceiling.position = Vector3.MoveTowards(ceiling.position, ceilingStart + Vector3.down * (targetDistance - 1), moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startClosing = true;
            Destroy(GetComponent<Collider>()); // disable re-entry
        }
    }
}
