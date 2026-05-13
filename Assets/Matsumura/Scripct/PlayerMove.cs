using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Model Settings")]
    public Transform modelTransform;
    public float rotationSpeed = 720f;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float stepHeight = 0.5f;
    public float stepSmooth = 2f;

    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float jumpStaminaCost = 20f;

    [Header("Layer")]
    public LayerMask groundLayer;

    [Header("Player Size")]
    public float playerHeight = 2.0f;

    [Header("Ground Check")]
    public float groundCheckRadius = 0.25f;
    public float groundCheckDistance = 0.1f;

    private bool isGrounded;

    private Rigidbody rb;
    private PlayerStamina2 stamina;

    private Vector3 moveInput;

    // 通常値保存用
    private float defaultJumpForce;
    private float defaultLinearDamping;
    private float defaultMass;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        stamina = GetComponent<PlayerStamina2>();

        rb.freezeRotation = true;

        // 初期値保存
        defaultJumpForce = jumpForce;
        defaultLinearDamping = rb.linearDamping;
        defaultMass = rb.mass;
    }

    void Update()
    {
        CheckGround();

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(h, 0, v).normalized;

        HandleModelRotation();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            TryJump();
        }
    }

    void FixedUpdate()
    {
        ApplyMovement();

        TryStepUp();
    }

    void ApplyMovement()
    {
        Vector3 velocity = moveInput * moveSpeed;

        velocity.y = rb.linearVelocity.y;

        rb.linearVelocity = velocity;
    }

    void HandleModelRotation()
    {
        if (moveInput != Vector3.zero && modelTransform != null)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveInput);

            modelTransform.rotation =
                Quaternion.RotateTowards(
                    modelTransform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
        }
    }

    void TryStepUp()
    {
        if (moveInput == Vector3.zero) return;

        Vector3 rayOrigin =
            transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(rayOrigin,
                            moveInput,
                            out RaycastHit hit,
                            0.7f,
                            groundLayer))
        {
            Vector3 stepUpperOrigin =
                transform.position + Vector3.up * stepHeight;

            if (!Physics.Raycast(stepUpperOrigin,
                                 moveInput,
                                 0.8f,
                                 groundLayer))
            {
                rb.linearVelocity =
                    new Vector3(
                        rb.linearVelocity.x,
                        stepSmooth,
                        rb.linearVelocity.z
                    );

                rb.position += moveInput * 0.02f;
            }
        }
    }

    void TryJump()
    {
        if (stamina != null &&
            stamina.UseStamina(jumpStaminaCost))
        {
            rb.AddForce(
                Vector3.up * jumpForce,
                ForceMode.Impulse
            );
        }
    }

    void CheckGround()
    {
        Vector3 origin =
            transform.position +
            Vector3.down *
            (playerHeight * 0.5f - groundCheckRadius);

        if (Physics.SphereCast(origin,
                               groundCheckRadius,
                               Vector3.down,
                               out RaycastHit hit,
                               groundCheckDistance,
                               groundLayer))
        {
            float angle =
                Vector3.Angle(hit.normal, Vector3.up);

            isGrounded = angle < 45f;
        }
        else
        {
            isGrounded = false;
        }
    }

    // 氷床用
    public void SetIceState(bool ice)
    {
        if (ice)
        {
            // ジャンプ弱化
            jumpForce = defaultJumpForce * 0.6f;

            // 滑る
            rb.linearDamping = 0f;

            // 少し軽くする
            rb.mass = 0.7f;
        }
        else
        {
            // 元に戻す
            jumpForce = defaultJumpForce;

            rb.linearDamping = defaultLinearDamping;

            rb.mass = defaultMass;
        }
    }
}