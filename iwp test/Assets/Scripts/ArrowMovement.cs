using UnityEngine;

public class ArrowBobAndSpin : MonoBehaviour
{
    public float bobSpeed = 2f;         // Speed of the up/down motion
    public float bobHeight = 0.5f;      // Max height offset from start
    public float spinSpeed = 50f;       // Degrees per second for rotation

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        // Bobbing motion
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);

        // Slow spin
        transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime, Space.Self);
    }
}