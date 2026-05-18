using UnityEngine;

public class PlayerMove : MonoBehaviour
{   
    [Header("Player ID")]
    [Range(1, 2)] public int playerID = 1;

    [Header("Model Settings")]
    public Transform modelTransform;
    public float rotationSpeed = 720f;
    public float modelRotationOffset = 90f;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float stepHeight = 0.8f;
    public float stepSmooth = 5f;

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
    private bool isOnIce;

    private Rigidbody rb;
    private PlayerStamina2 stamina;

    private Vector3 moveInput;

    // 通常値保存用
    private float defaultJumpForce;
    private float defaultLinearDamping;
    private float defaultMass;

    // インプット名を保持する変数
    private string horizontalAxis;
    private string verticalAxis;
    private string jumpButton;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (playerID == 1)
        {
            horizontalAxis = "Horizontal";
            verticalAxis = "Vertical";
            jumpButton = "Jump";
        }
        else
        {
            horizontalAxis = "Horizontal_P2";
            verticalAxis = "Vertical_P2";
            jumpButton = "Jump_P2";
        }
    }

    void Start()
    {
        stamina = GetComponent<PlayerStamina2>();

        // 💡 インスペクターで設定した「本物の数値」をここで安全に記憶します
        defaultJumpForce = jumpForce;
        defaultLinearDamping = rb.linearDamping;
        defaultMass = rb.mass;
    }

    void Update()
    {
        CheckGround();

        float h = Input.GetAxisRaw(horizontalAxis);
        float v = Input.GetAxisRaw(verticalAxis);

        moveInput = new Vector3(h, 0, v).normalized;

        HandleModelRotation();

        // --- 👇 2Pジャンプ・アニメーション同期処理 ---
        Player playerScript = GetComponent<Player>();
        bool g = (playerScript != null) ? playerScript.isGrounded : isGrounded;

        if (Input.GetButtonDown(jumpButton) && g)
        {
            if (playerID == 1)
            {
                if (stamina != null) stamina.UseStamina(jumpStaminaCost);
            }
            else if (playerID == 2 && playerScript != null)
            {
                if (stamina == null || stamina.UseStamina(jumpStaminaCost))
                {
                    playerScript.rb.linearVelocity = new Vector3(playerScript.rb.linearVelocity.x, 0f, playerScript.rb.linearVelocity.z);
                    playerScript.rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    
                    Animator anim = GetComponent<Animator>();
                    if (anim != null) anim.SetTrigger("Jump");
                }
            }
        }

        if (playerScript != null)
        {
            playerScript.UpdatePhysicsState();
        }
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    void ApplyMovement()
    {
        if (isOnIce)
        {
            rb.AddForce(moveInput * moveSpeed, ForceMode.Acceleration);
        }
        else
        {
            Vector3 velocity = moveInput * moveSpeed;
            velocity.y = rb.linearVelocity.y;
            rb.linearVelocity = velocity;
        }
    }

    void HandleModelRotation()
    {
        if (moveInput.magnitude > 0.1f && modelTransform != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            targetRotation *= Quaternion.Euler(0, modelRotationOffset, 0);
            
            modelTransform.rotation = Quaternion.RotateTowards(
                modelTransform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!Input.GetButton(jumpButton)) return;
        if (moveInput.magnitude < 0.1f) return;

        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                float contactHeight = contact.point.y - transform.position.y;

                if (contactHeight > 0.0f && contactHeight < stepHeight)
                {
                    Vector3 dirToContact = (contact.point - transform.position).normalized;
                    dirToContact.y = 0;

                    if (Vector3.Dot(moveInput, dirToContact) > 0.2f)
                    {
                        rb.linearVelocity = new Vector3(rb.linearVelocity.x, stepSmooth, rb.linearVelocity.z);
                        rb.position += Vector3.up * 0.05f + moveInput * 0.02f;
                        break;
                    }
                }
            }
        }
    }

    void CheckGround()
    {
        Vector3 origin = transform.position + Vector3.down * (playerHeight * 0.5f - groundCheckRadius);

        if (Physics.SphereCast(origin, groundCheckRadius, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            isGrounded = angle < 45f;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void SetIceState(bool ice)
    {
        isOnIce = ice;

        if (ice)
        {
            jumpForce = defaultJumpForce * 0.6f;
            rb.linearDamping = 0.05f;
            rb.mass = 0.7f;
        }
        else
        {
            // 💡 これでStartで保存された「インスペクター通りの正しい数値」が戻るようになります！
            jumpForce = defaultJumpForce;
            rb.linearDamping = defaultLinearDamping;
            rb.mass = defaultMass;
        }
    }
}