using UnityEngine;

public class PlayerMove : MonoBehaviour
{   
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

        // 地面にいる時だけジャンプ
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
        // 氷床中
        if (isOnIce)
        {
            rb.AddForce(
                moveInput * moveSpeed,
                ForceMode.Acceleration
            );
        }
        // 通常床
        else
        {
            Vector3 velocity = moveInput * moveSpeed;

            velocity.y = rb.linearVelocity.y;

            rb.linearVelocity = velocity;
        }
    }

    void HandleModelRotation()
    {
        // 入力がある時だけ、モデル（子）をその方向に向かせる
        if (moveInput.magnitude > 0.1f && modelTransform != null)
        {
            // 1. 入力方向に基づいたターゲットの回転を作成
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            
            // 2. モデルの向きのズレを補正（Y軸を90度などオフセット）
            targetRotation *= Quaternion.Euler(0, modelRotationOffset, 0);
            
            // 3. 現在の回転から目標の回転へスムーズに回す
            modelTransform.rotation = Quaternion.RotateTowards(
                modelTransform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // 壁よじ登り
    private void OnCollisionStay(Collision collision)
    {
        // ジャンプ押してないなら終了
        if (!Input.GetButton("Jump")) return;

        // 入力ないなら終了
        if (moveInput.magnitude < 0.1f) return;

        // GroundLayer判定
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // 接触位置の高さ
                float contactHeight =
                    contact.point.y - transform.position.y;

                // 足元〜stepHeightまで
                if (contactHeight > 0.0f &&
                    contactHeight < stepHeight)
                {
                    // 壁方向
                    Vector3 dirToContact =
                        (contact.point - transform.position)
                        .normalized;

                    dirToContact.y = 0;

                    // 壁方向へ入力してるか
                    if (Vector3.Dot(moveInput, dirToContact) > 0.2f)
                    {
                        rb.linearVelocity =
                            new Vector3(
                                rb.linearVelocity.x,
                                stepSmooth,
                                rb.linearVelocity.z
                            );

                        rb.position +=
                            Vector3.up * 0.05f +
                            moveInput * 0.02f;

                        break;
                    }
                }
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

        if (Physics.SphereCast(
                origin,
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

    // 氷床状態変更
    public void SetIceState(bool ice)
    {
        isOnIce = ice;

        if (ice)
        {
            // ジャンプ弱化
            jumpForce = defaultJumpForce * 0.6f;

            // 滑る
            rb.linearDamping = 0.05f;

            // 少し軽く
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