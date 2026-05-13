using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Model Settings")]
    public Transform modelTransform;
    public float rotationSpeed = 720f;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float stepHeight = 0.8f; // よじ登り判定の高さ
    public float stepSmooth = 5f;  // よじ登る速度（力）

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

        // 地面にいる時だけ通常のジャンプ
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            TryJump();
        }
    }

    void FixedUpdate()
    {
        ApplyMovement();
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
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            modelTransform.rotation = Quaternion.RotateTowards(
                modelTransform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // --- 壁に当たっている間の処理 ---
    // --- 修正版：ボタンを押している間、物理を無視して持ち上げる ---
    private void OnCollisionStay(Collision collision)
    {
        // 1. スペースキー（Jump）が押されているか
        if (!Input.GetButton("Jump")) return;

        // 2. 移動入力があるか
        if (moveInput.magnitude < 0.1f) return;

        // 3. 相手のレイヤー確認
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // プレイヤーの足元からの高さを計算
                float contactHeight = contact.point.y - transform.position.y;

                // 4. 判定条件を少し広げました（0.0f 〜 stepHeight）
                if (contactHeight > 0.0f && contactHeight < stepHeight)
                {
                    // 衝突方向の計算
                    Vector3 dirToContact = (contact.point - transform.position).normalized;
                    dirToContact.y = 0;

                    // 5. 壁に向かって進んでいるなら
                    if (Vector3.Dot(moveInput, dirToContact) > 0.2f) 
                    {
                        // 【ここがポイント】重力を打ち消すように、直接速度を上書き
                        // stepSmoothは 7〜10 くらいに上げるとスムーズです
                        rb.linearVelocity = new Vector3(rb.linearVelocity.x, stepSmooth, rb.linearVelocity.z);
                        
                        // 壁にめり込まないよう、少しだけ浮かせて前に進める
                        rb.position += Vector3.up * 0.05f + moveInput * 0.02f;
                        break; 
                    }
                }
            }
        }
    }

    void TryJump()
    {
        if (stamina != null && stamina.UseStamina(jumpStaminaCost))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
        if (ice) { jumpForce = defaultJumpForce * 0.6f; rb.linearDamping = 0f; rb.mass = 0.7f; }
        else { jumpForce = defaultJumpForce; rb.linearDamping = defaultLinearDamping; rb.mass = defaultMass; }
    }
}