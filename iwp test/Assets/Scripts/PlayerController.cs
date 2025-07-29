using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class FootstepAudioSet
{
    public string surfaceTag;
    public AudioClip[] footstepClips;
}

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float checkDistance = 0.1f;
    [SerializeField] private float jumpCooldown = 0.2f;
    private float lastJumpTime = -999f;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchCameraOffset = -0.5f;
    [SerializeField] private float crouchSmooth = 6f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 1f;
    [SerializeField] private Transform cameraTransform;
    [HideInInspector] public bool isLookLocked = false;

    [Header("Stats")]
    public int maxHealth = 3;
    public int currentHealth;

    public float maxStamina = 100f;
    public float currentStamina;
    [SerializeField] private float staminaDrainRate = 20f;
    [SerializeField] private float staminaRegenRate = 15f;
    [SerializeField] private float staminaRegenDelay = 1.5f;

    private bool isSprintingBlocked = false;
    private float staminaRegenTimer;
    private bool isInvincible = false;
    [SerializeField] private float invisDelay = 2.0f;

    [Header("Plank Balancing")]
    private bool isOnPlank = false;
    [SerializeField] private float plankForwardSpeed = 3f;
    [SerializeField] private float plankBalanceSensitivity = 2f;
    [SerializeField] private float plankMaxTiltAngle = 30f;
    [SerializeField] private float plankFallThreshold = 25f;
    private float plankTilt = 0f;
    [SerializeField] private float autoTiltSpeed = 5f;
    [SerializeField] private float autoTiltDirectionChangeTime = 2f;
    private float autoTiltTimer = 0f;
    private int autoTiltDirection = 1;

    [Header("Plank Detection")]
    [SerializeField] private LayerMask plankLayer;
    [SerializeField] private float plankCheckDistance = 0.2f;

    [Header("L2 Respawn")]
    private Vector3 lastCheckpoint;
    [SerializeField] private float fallThresholdY = -10f;

    [Header("Interaction")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private LayerMask interactableLayer;
    private IInteractable lastInteractable;

    private InputAction move, look, jump, sprint, crouch, interact;

    [HideInInspector] public Rigidbody rb;
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

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;
    private AudioSource sfxSource;
    [SerializeField] private AudioClip[] sfxClips;

    [Header("Footstep Audio")]
    [SerializeField] private FootstepAudioSet[] footstepAudioSets;
    private float footstepTimer = 0f;
    [SerializeField] private float footstepInterval = 0.5f;

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
        lastCheckpoint = transform.position;

        move = inputActions["Move"];
        look = inputActions["Look"];
        jump = inputActions["Jump"];
        sprint = inputActions["Sprint"];
        crouch = inputActions["Crouch"];
        interact = inputActions["Interact"];

        sfxSource = GetComponent<AudioSource>();

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
        CheckIfOnPlank();

        if (isOnPlank)
        {
            HandlePlankMovement();
            return;
        }

        HandleLook();
        HandleJump();
        HandleCrouch();
        SmoothCrouch();
        HandleStamina();

        //footstep audio
        if (moveInput.magnitude > 0.1f && isGrounded && !isOnPlank)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                AudioClip clip = GetFootstepClip();
                if (clip != null)
                {
                    sfxSource.PlayOneShot(clip);
                }
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }

        CheckFallHeight();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleInteraction()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 3f, Color.cyan, 0.5f);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                if (interactable != lastInteractable)
                {
                    if (lastInteractable is Torch prevTorch)
                        prevTorch.HidePrompt();

                    lastInteractable = interactable;

                    if (interactable is Torch torch)
                        torch.ShowPrompt();
                }

                if (interact.WasPressedThisFrame())
                {
                    if (lastInteractable is ElevatorButton elevatorButton)
                    {
                        PlaySFX(1);
                    }
                    interactable.OnInteract();
                }
            }
            else
            {
                ClearPrompt();
            }
        }
        else
        {
            ClearPrompt();
        }
    }

    void ClearPrompt()
    {
        if (lastInteractable is Torch torch)
        {
            torch.HidePrompt();
            lastInteractable = null;
        }
    }

    void HandleMovement()
    {
        float currentSpeed = walkSpeed;

        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (!isOnPlank && sprint.IsPressed() && !isSprintingBlocked && currentStamina > 0f)
        {
            currentSpeed = sprintSpeed;
        }

        Vector3 move;
        if (isOnPlank)
        {
            move = transform.forward * moveInput.y;
        }
        else
        {
            move = transform.right * moveInput.x + transform.forward * moveInput.y;
        }

        Vector3 velocity = move.normalized * currentSpeed;
        velocity.y = rb.linearVelocity.y;
        if (!rb.isKinematic)
        {
            rb.linearVelocity = velocity;
        }

        // anim
        bool isWalking = moveInput.magnitude > 0.1f && isGrounded;
        animator.SetBool("IsWalking", isWalking);
    }

    public void SetMouseSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }

    void HandlePlankMovement()
    {
        float forwardInput = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
        float balanceInput = Input.GetKey(KeyCode.D) ? -1 : Input.GetKey(KeyCode.A) ? 1 : 0;

        // Move forward/back
        transform.position += transform.forward * forwardInput * plankForwardSpeed * Time.deltaTime;

        // Drift logic
        autoTiltTimer += Time.deltaTime;
        if (autoTiltTimer >= autoTiltDirectionChangeTime)
        {
            autoTiltDirection *= -1;
            autoTiltTimer = 0f;
        }

        // Apply auto-tilt drift
        plankTilt += autoTiltDirection * autoTiltSpeed * Time.deltaTime;

        // Apply player counterbalance
        plankTilt += balanceInput * plankBalanceSensitivity * Time.deltaTime;

        // Clamp tilt
        plankTilt = Mathf.Clamp(plankTilt, -plankMaxTiltAngle, plankMaxTiltAngle);

        // Rotate player based on tilt
        transform.localRotation = Quaternion.Euler(0f, transform.localEulerAngles.y, plankTilt);

        // Fall check
        if (Mathf.Abs(plankTilt) >= plankFallThreshold)
        {
            LoseLifeAndRespawn();
        }
    }

    void CheckIfOnPlank()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        Ray ray = new Ray(origin, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, plankCheckDistance))
        {
            Debug.Log("Raycast hit: " + hit.collider.name + ", Layer: " + hit.collider.gameObject.layer);


            bool isHitPlankLayer = ((1 << hit.collider.gameObject.layer) & plankLayer) != 0;
            Debug.Log("Is plank layer? " + isHitPlankLayer);


            if (isHitPlankLayer)
            {
                if (!isOnPlank)
                {
                    isOnPlank = true;

                    Vector3 plankForward = hit.transform.forward;
                    plankForward.y = 0;
                    plankForward.Normalize();

                    Vector3 playerForward = transform.forward;
                    playerForward.y = 0;
                    playerForward.Normalize();

                    if (Vector3.Dot(playerForward, plankForward) < 0)
                    {
                        plankForward = -plankForward;
                    }

                    if (plankForward != Vector3.zero)
                    {
                        Quaternion targetRot = Quaternion.LookRotation(plankForward);
                        transform.rotation = targetRot;
                    }
                }
            }
            else
            {
                isOnPlank = false;
            }
        }
        else
        {
            isOnPlank = false;
        }

        if (!isOnPlank)
        {
            plankTilt = 0f;
            transform.localRotation = Quaternion.Euler(0f, transform.localEulerAngles.y, 0f);
        }
    }

    public void UpdateCheckpoint(Vector3 checkpointPos)
    {
        lastCheckpoint = checkpointPos;
    }

    void CheckFallHeight()
    {
        if (transform.position.y < fallThresholdY)
        {
            LoseLifeAndRespawn();
        }
    }

    void LoseLifeAndRespawn()
    {
        currentHealth--;

        if (currentHealth > 0)
        {
            Debug.Log("You fell! Respawning to spawn point.");
            transform.position = spawnPosition;
            rb.linearVelocity = Vector3.zero;
            plankTilt = 0f;
            cameraTransform.localRotation = Quaternion.Euler(rotation, 0f, 0f);
            isOnPlank = false;
        }
        else
        {
            Debug.Log("No lives left!");
            Die();
        }
    }

    void HandleLook()
    {
        if (isLookLocked) return;

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        rotation -= mouseY;
        rotation = Mathf.Clamp(rotation, -90f, 90f);

        Quaternion current = cameraTransform.localRotation;
        cameraTransform.localRotation = Quaternion.Euler(rotation, 0f, current.eulerAngles.z); // preserve tilt

        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleJump()
    {
        if (jump.WasPressedThisFrame() && isGrounded && !isCrouching && Time.time >= lastJumpTime + jumpCooldown)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            lastJumpTime = Time.time;
            PlaySFX(0);
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
        if (!isOnPlank && sprint.IsPressed() && !isCrouching && moveInput != Vector2.zero && !isSprintingBlocked)
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
        Vector3 origin = transform.position + Vector3.up * 0.1f;
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

        WallsClosingIn walls = FindObjectOfType<WallsClosingIn>();
        if (walls != null)
            walls.ResetWalls();

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

    public void SetPlayerEnabled(bool enabled)
    {
        gameObject.SetActive(enabled);
    }

    public void PlaySFX(int index)
    {
        if (index >= 0 && index < sfxClips.Length && sfxClips[index] != null)
        {
            sfxSource.PlayOneShot(sfxClips[index]);
        }
    }

    private AudioClip GetFootstepClip()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            string tag = hit.collider.tag;
            foreach (var set in footstepAudioSets)
            {
                if (set.surfaceTag == tag && set.footstepClips.Length > 0)
                {
                    int index = Random.Range(0, set.footstepClips.Length);
                    return set.footstepClips[index];
                }
            }
        }
        return null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawLine(origin, origin + Vector3.down * plankCheckDistance);
    }
}