using UnityEngine;
using UnityEngine.InputSystem;

public class Plank : MonoBehaviour
{
    private Rigidbody heldRigidbody;

    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask plankLayer;

    [Header("Follow Settings")]
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float minFollowDistance = 1f;
    [SerializeField] private float followDistance = 3f;
    [SerializeField] private float moveSmoothness = 10f;

    [Header("Resize Settings")]
    [SerializeField] private float scrollSpeed = 2f;
    [SerializeField] private float minLength = 0.5f;
    [SerializeField] private float maxLength = 5f;

    private Transform heldPlank = null;
    private bool isHolding = false;

    void Awake()
    {
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && !isHolding)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, plankLayer))
            {
                heldPlank = hit.collider.transform;
                isHolding = true;

                // Cache Rigidbody and freeze physics
                heldRigidbody = heldPlank.GetComponent<Rigidbody>();
                if (heldRigidbody != null)
                {
                    heldRigidbody.isKinematic = true;
                    heldRigidbody.useGravity = false;
                }
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            // Restore physics
            if (heldRigidbody != null)
            {
                heldRigidbody.isKinematic = false;
                heldRigidbody.useGravity = true;
                heldRigidbody = null;
            }

            heldPlank = null;
            isHolding = false;
        }

        if (isHolding && heldPlank != null)
        {
            Vector3 forward = cam.transform.forward;
            Vector3 flatForward = new Vector3(forward.x, 0, forward.z).normalized;
            float pitchInfluence = Mathf.Clamp(-forward.y, 0f, 1f); // More downward = more pitch

            // Base distance
            Vector3 basePos = cam.transform.position + flatForward * followDistance;
            // Add vertical offset when looking down
            Vector3 adjustedPos = basePos - Vector3.up * pitchInfluence * 1.5f; // tweak 1.5f for how much drop

            heldPlank.position = Vector3.Lerp(heldPlank.position, adjustedPos, Time.deltaTime * moveSmoothness);

            float scroll = Mouse.current.scroll.ReadValue().y;

            if (Mouse.current.rightButton.isPressed)
            {
                // Adjust plank length (z scale)
                Vector3 scale = heldPlank.localScale;
                float newLength = Mathf.Clamp(scale.z + scroll * scrollSpeed * Time.deltaTime, minLength, maxLength);
                heldPlank.localScale = new Vector3(scale.x, scale.y, newLength);
            }
            else
            {
                // Move plank closer/farther
                followDistance = Mathf.Clamp(followDistance + scroll * scrollSpeed * Time.deltaTime, minFollowDistance, maxDistance);
            }
        }
    }
}