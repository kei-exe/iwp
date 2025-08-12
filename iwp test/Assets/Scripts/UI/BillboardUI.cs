using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        // look at camera
        transform.LookAt(transform.position + cam.forward);
    }
}