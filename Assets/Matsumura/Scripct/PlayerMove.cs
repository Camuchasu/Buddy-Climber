using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Model Settings")]
    public Transform modelTransform;
    public float rotationSpeed = 720f;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float stepHeight = 0.6f; // 少し高めに設定すると登りやすいです
    public float stepSmooth = 5f;  // 持ち上げる力（2fだと弱いので少し上げました）

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
        // FixedUpdate内でのTryStepUp(Ray判定)は空にします（Collision判定に移行するため）
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

   // --- 左右の移動でも反応する物理よじ登り処理 ---
    private void OnCollisionStay(Collision collision)
    {
        // 1. 移動入力がまったくない場合は登らない（静止中の浮き上がり防止）
        if (moveInput.magnitude < 0.1f) return;

        // 2. ぶつかっている相手のレイヤーを確認
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // 3. 接触点の高さを計算（プレイヤーの足元からの相対高さ）
                float contactHeight = contact.point.y - transform.position.y;

                // 4. 判定：足元(0.1f)から設定高さ(stepHeight)の間でぶつかっているか
                if (contactHeight > 0.1f && contactHeight < stepHeight)
                {
                    // 5. 衝突方向の確認（壁が「進もうとしている方向」にあるか）
                    // 衝突点への方向ベクトル
                    Vector3 dirToContact = (contact.point - transform.position).normalized;
                    dirToContact.y = 0; // 高さ方向は無視して水平方向だけで判定

                    // 入力方向と衝突方向が近い（＝壁に向かって歩いている）場合のみ登る
                    if (Vector3.Dot(moveInput, dirToContact) > 0.3f)
                    {
                        // 上方向にスライド
                        rb.linearVelocity = new Vector3(rb.linearVelocity.x, stepSmooth, rb.linearVelocity.z);
                        
                        // 角に乗り上げやすくするための前進補正（入力方向へ押し出す）
                        rb.position += moveInput * 0.03f;
                        break; 
                    }
                }
            }
        }
    }

    // 以前のRaycast方式は空のメソッドとして残すか、削除してください
    void TryStepUp() { }

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
        if (ice)
        {
            jumpForce = defaultJumpForce * 0.6f;
            rb.linearDamping = 0f;
            rb.mass = 0.7f;
        }
        else
        {
            jumpForce = defaultJumpForce;
            rb.linearDamping = defaultLinearDamping;
            rb.mass = defaultMass;
        }
    }
}