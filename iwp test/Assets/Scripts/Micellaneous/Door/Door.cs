using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Credits: Tutorial (https://www.youtube.com/watch?v=cPltQK5LlGE)

    public bool isOpen = false;
    [SerializeField] private bool isRotatingDoor = true;
    [SerializeField] private float speed = 1f; // how quickly door open and close
    [Header("Rotation Configs")]
    [SerializeField] private float rotationAmount = 90f; // how much door rotates to open
    [SerializeField] private float forwardDir = 0; // compare dot product to

    private Vector3 StartRotation;
    private Vector3 forward;

    private Coroutine animationCoroutine;

    private void Awake()
    {
        StartRotation = transform.rotation.eulerAngles;
        // since "forward" is into door frame, choose dir to think as "forward"
        forward = transform.right;
    }

    public void Open(Vector3 userPos)
    {
        if (!isOpen)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            if (isRotatingDoor)
            {
                float dot = Vector3.Dot(forward, (userPos - transform.position).normalized);
                animationCoroutine = StartCoroutine(DoRotationOpen(dot));
            }
        }
    }

    private IEnumerator DoRotationOpen(float forwardAmt)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if (forwardAmt >= forwardDir)
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y - rotationAmount, 0));
        }
        else
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y + rotationAmount, 0));
        }

        isOpen = true;

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            if (isRotatingDoor)
            {
                animationCoroutine = StartCoroutine(DoRotationClose());
            }
        }
    }

    private IEnumerator DoRotationClose()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(StartRotation);

        isOpen = false;

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }
}
