using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float jumpStaminaCost = 20f; // ジャンプ1回で消費する量

    [Header("Layer")]
    public LayerMask groundLayer;

    [Header("Player Size")]
    public float playerHeight = 2.0f;

    [Header("Ground Check")]
    public float groundCheckRadius = 0.25f;
    public float groundCheckDistance = 0.1f;

    [Header("Ceiling Check")]
    public float ceilingCheckRadius = 0.25f;
    public float ceilingCheckDistance = 0.1f;

    private bool isGrounded;
    private bool isCeiling;
    private Rigidbody rb;
    private PlayerStamina2 stamina; // スタミナスクリプトとの連携用

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        stamina = GetComponent<PlayerStamina2>(); // 同じオブジェクトに付いている想定
    }

    void Update()
    {
        CheckGround();
        CheckCeiling();

        // ジャンプ入力（スペースキー）の検知
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            TryJump();
        }
    }

    void TryJump()
    {
        // スタミナがあるか確認してからジャンプ
        if (stamina != null)
        {
            if (stamina.UseStamina(jumpStaminaCost))
            {
                // 上方向に力を加える
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                Debug.Log("ジャンプ成功！");
            }
            else
            {
                Debug.Log("スタミナが足りません！");
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

    void CheckCeiling()
    {
        Vector3 origin = transform.position + Vector3.up * (playerHeight * 0.5f - ceilingCheckRadius);
        isCeiling = Physics.SphereCast(origin, ceilingCheckRadius, Vector3.up, out RaycastHit hit, ceilingCheckDistance, groundLayer);
    }
}