using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float jumpForce = 5f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float crouchCameraOffset = -0.5f;
    public float crouchSmooth = 6f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 1f;
    public Transform cameraTransform;

    [Header("Stats")]
    public int maxHealth = 3;
    public int currentHealth;

    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrainRate = 20f;
    public float staminaRegenRate = 15f;
    public float staminaRegenDelay = 1.5f;

    private bool isSprintingBlocked = false;
    private float staminaRegenTimer;

    private bool isInvincible = false;
    public float invisDelay = 2.0f;

    [Header("Interaction")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputActionAsset inputActions;
    public float interactionDistance = 2f;
    public LayerMask interactableLayer;

    private InputAction move, look, jump, sprint, crouch, interact;

    public Rigidbody rb;
    private CapsuleCollider col;
    private float originalHeight;
    private Vector3 originalCamPos;
    private float targetHeight;
    private Vector3 targetCamPos;
    private Vector3 spawnPosition;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isGrounded = false;
    private bool isCrouching = false;
    private float rotation = 0f;

    [Header("UI")]
    public GameObject restartPanel;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        originalHeight = col.height;
        targetHeight = originalHeight;

        originalCamPos = cameraTransform.localPosition;
        targetCamPos = originalCamPos;

        spawnPosition = transform.position;

        // Reference actions
        move = inputActions["Move"];
        look = inputActions["Look"];
        jump = inputActions["Jump"];
        sprint = inputActions["Sprint"];
        crouch = inputActions["Crouch"];
        interact = inputActions["Interact"];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    void Update()
    {
        CheckGrounded();

        moveInput = move.ReadValue<Vector2>();
        lookInput = look.ReadValue<Vector2>();

        HandleInteraction();

        HandleLook();
        HandleJump();
        HandleCrouch();
        SmoothCrouch();

        HandleStamina();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleInteraction()
    {
        if (interact.WasPressedThisFrame())
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 3f, Color.cyan, 0.5f);

            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer))
            {
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

                if (interactable != null)
                {
                    interactable.OnInteract();
                }
            }
        }
    }

    void HandleMovement()
    {
        float currentSpeed = walkSpeed;

        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (sprint.IsPressed() && !isSprintingBlocked && currentStamina > 0f)
        {
            currentSpeed = sprintSpeed;
        }

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 velocity = move.normalized * currentSpeed;
        velocity.y = rb.linearVelocity.y;
        if (!rb.isKinematic)
        {
            rb.linearVelocity = velocity;
        }
    }

    void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        rotation -= mouseY;
        rotation = Mathf.Clamp(rotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(rotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleJump()
    {
        if (jump.WasPressedThisFrame() && isGrounded && !isCrouching)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleCrouch()
    {
        bool crouchPressed = crouch.IsPressed();

        if (crouchPressed != isCrouching)
        {
            isCrouching = crouchPressed;
            targetHeight = isCrouching ? crouchHeight : originalHeight;
            targetCamPos = isCrouching
                ? originalCamPos + new Vector3(0, crouchCameraOffset, 0)
                : originalCamPos;
        }
    }

    void HandleStamina()
    {
        if (sprint.IsPressed() && !isCrouching && moveInput != Vector2.zero && !isSprintingBlocked)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isSprintingBlocked = true;
            }

            staminaRegenTimer = 0f;
        }
        else
        {
            if (currentStamina < maxStamina)
            {
                staminaRegenTimer += Time.deltaTime;
                if (staminaRegenTimer >= staminaRegenDelay)
                {
                    currentStamina += staminaRegenRate * Time.deltaTime;
                    if (currentStamina >= maxStamina)
                    {
                        currentStamina = maxStamina;
                        isSprintingBlocked = false;
                    }
                }
            }
        }
    }

    void SmoothCrouch()
    {
        col.height = Mathf.Lerp(col.height, targetHeight, Time.deltaTime * crouchSmooth);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetCamPos, Time.deltaTime * crouchSmooth);
    }
    void CheckGrounded()
    {
        float checkDistance = 0.1f;
        Vector3 origin = transform.position + Vector3.up * 0.1f; // small offset above feet
        isGrounded = Physics.Raycast(origin, Vector3.down, col.bounds.extents.y + checkDistance);
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;
        currentHealth -= amount;

        if (currentHealth > 0)
            StartCoroutine(RespawnAfterDelay(0.5f)); // shorter delay
        else
            Die();

    }

    void Die()
    {
        Debug.Log("Player died � all health lost.");
        StartCoroutine(RestartLevel());
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        isInvincible = true;

        rb.isKinematic = true;
        yield return new WaitForSeconds(delay);
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;

        transform.position = spawnPosition;

        rb.isKinematic = false;
        Debug.Log("Player respawned.");
        yield return new WaitForSeconds(invisDelay);
        isInvincible = false;
        Debug.Log("RUN");
    }

    private IEnumerator RestartLevel()
    {
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        

        yield return new WaitForSeconds(2f);

        Debug.Log("RESTARTING");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("RESTARTINGGGG");
        
        Time.timeScale = 0f;
        if (restartPanel != null)
        {
            Debug.Log("RESTARTINGS");
            restartPanel.SetActive(true);
        }
    }
}