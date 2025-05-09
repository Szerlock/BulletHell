using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [SerializeField] private CharacterController controller;
    [SerializeField] public Transform cameraTransform;
    [SerializeField] private Transform minigun;

    [Header("PushBack")]
    [SerializeField] private float pushForce;
    [SerializeField] private float duration;
    [SerializeField] private float upwardForce;

    private Vector3 velocity;
    public bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    
    public bool isHovering = false;

    [Header("Dashing Variable")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;

    public bool isAiming = false;

    [Header("Rotate Character")]
    [SerializeField] private Vector3 offset;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        if (Cursor.lockState == CursorLockMode.Locked && !Cursor.visible)
        {
            HandleDashInput();

            if (!isDashing)
                HandleMovement();
        }
        ApplyGravity();

        controller.Move(velocity * Time.deltaTime);
        RotateTowardsCamera();
    }

    private void HandleMovement()
    {
        if (isHovering)
        {
            Debug.Log("Hovering returning");
            return;
        }

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * moveZ + camRight * moveX;

        float effectiveSpeed = moveSpeed;
        if (!isGrounded && !isHovering)
        {
            effectiveSpeed *= 0.3f;
        }

        controller.Move(moveDir * effectiveSpeed * Time.deltaTime);

        CheckJump();
    }
    
    private void CheckJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f && !isDashing)
        {
            Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            if (inputDir.magnitude < 0.1f)
                inputDir = Vector3.forward;

            dashDirection = (cameraTransform.forward * inputDir.z + cameraTransform.right * inputDir.x).normalized;
            dashDirection.y = 0f;

            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;
        }

        if (isDashing)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }
    }

    public bool IsMoving()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
            
        return Mathf.Abs(moveX) > 0 || Mathf.Abs(moveZ) > 0;
    }

    private void ApplyGravity()
    {
        if (!isGrounded && !isHovering)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (isHovering)
        {
            velocity.y = 0f;
        }

        if (isGrounded && !isHovering && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void RotateTowardsCamera()
    {
        Vector3 lookDirection = cameraTransform.forward;
        lookDirection.y = 0f;

        if (lookDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

            Quaternion offsetRotation = Quaternion.Euler(offset);

            targetRotation *= offsetRotation;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            if (!isGrounded)
            {
                float pitch = cameraTransform.localEulerAngles.x;

                Quaternion characterRotation = Quaternion.Euler(pitch, transform.eulerAngles.y, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, characterRotation, Time.deltaTime * 10f);
            }
        }
    }

    public IEnumerator ApplyPushBackwards()
    {
        float timer = 0f;
        Vector3 pushDir = (-transform.forward * pushForce + Vector3.up * upwardForce);

        float originalVelocityY = velocity.y;

        velocity.y = 0f;

        while (timer < duration)
        {
            controller.Move(pushDir * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        velocity.y = originalVelocityY;

    }
}
