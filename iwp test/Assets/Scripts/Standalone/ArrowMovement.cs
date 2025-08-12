using UnityEngine;

public class ArrowBobAndSpin : MonoBehaviour
{
    public float bobSpeed = 2f;
    public float bobHeight = 0.5f;
    public float spinSpeed = 50f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);

        transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime, Space.Self);
    }
}