using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    [Header("Brzine kretanja")]
    public bool canMove = true;
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float sprintSpeed = 9f;
    public float crouchSpeed = 1.5f;
    public float sideSpeed = 2.5f;
    public float jumpForce = 5f;

    [Header("Stamina Sistem")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float runDrainRate = 5f;
    public float sprintDrainRate = 20f;
    public float rechargeRate = 15f;
    public Slider staminaBar;

    [Header("Double Tap Postavke")]
    public float doubleTapTime = 0.3f;
    private float lastShiftTime = 0f;
    private bool isSprinting = false;

    [Header("Physics & Grounding")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float jumpDelay = 1f;
    private float nextJumpTime = 0f;

    private Rigidbody rb;
    private bool isGrounded;
    private float currentSpeed;
    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponent<Animator>();
        currentStamina = maxStamina;
        if (staminaBar != null) staminaBar.maxValue = maxStamina;
    }

    void Update()
    {
        if (!canMove) { StopMovement(); return; }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        HandleSprintInput();
        HandleStamina();
        HandleSpeedLogic();
        HandleMovement();
        HandleJump();
        HandleAnimations();
        UpdateUI();
    }

    void HandleSprintInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (Time.time - lastShiftTime <= doubleTapTime && currentStamina > 10f)
                isSprinting = true;
            lastShiftTime = Time.time;
        }
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            isSprinting = false;
    }

    void HandleStamina()
    {
        bool isMovingForward = Input.GetAxisRaw("Vertical") > 0.1f;
        if (isSprinting && isMovingForward)
            currentStamina -= sprintDrainRate * Time.deltaTime;
        else if (Input.GetKey(KeyCode.LeftShift) && isMovingForward && !Input.GetKey(KeyCode.LeftControl))
            currentStamina -= runDrainRate * Time.deltaTime;
        else
            currentStamina += rechargeRate * Time.deltaTime;

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        if (currentStamina <= 0) isSprinting = false;
    }

    void HandleSpeedLogic()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.LeftControl)) currentSpeed = crouchSpeed;
        else if ((isSprinting || Input.GetKey(KeyCode.LeftShift)) && vertical > 0.1f && currentStamina > 0)
            currentSpeed = isSprinting ? sprintSpeed : runSpeed;
        else if (Mathf.Abs(horizontal) > 0.1f && Mathf.Abs(vertical) < 0.1f)
            currentSpeed = sideSpeed;
        else currentSpeed = walkSpeed;
    }

    void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 move = (transform.right * x + transform.forward * z);
        if (move.magnitude > 1f) move.Normalize();
        rb.linearVelocity = new Vector3(move.x * currentSpeed, rb.linearVelocity.y, move.z * currentSpeed);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && Time.time >= nextJumpTime)
        {
            animator.SetBool("IsJump", true);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            nextJumpTime = Time.time + jumpDelay;
        }
    }

    void HandleAnimations()
    {
        if (animator == null) return;
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);
        bool shouldRunAnim = (isSprinting || Input.GetKey(KeyCode.LeftShift)) && v > 0.1f && currentStamina > 0 && !isCrouching;

        animator.SetBool("IsJumpIdle", !isGrounded);
        if (isGrounded) animator.SetBool("IsJump", false);
        animator.SetBool("IsCrouch", isCrouching && (v == 0 && h == 0));
        animator.SetBool("IsCrouchWalk", isCrouching && (v != 0 || h != 0));
        animator.SetBool("IsRunning", shouldRunAnim);
        animator.SetBool("IsNormalWalking", (v != 0) && !isCrouching && !shouldRunAnim);
        animator.SetBool("IsNormalWalkingLeft", h < -0.1f && !isCrouching && !shouldRunAnim);
        animator.SetBool("IsNormalWalkingRight", h > 0.1f && !isCrouching && !shouldRunAnim);
    }

    void UpdateUI() { if (staminaBar != null) staminaBar.value = currentStamina; }

    void StopMovement()
    {
        rb.linearVelocity = Vector3.zero;
        if (animator != null)
        {
            animator.SetBool("IsNormalWalking", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsCrouch", false);
        }
    }
}
