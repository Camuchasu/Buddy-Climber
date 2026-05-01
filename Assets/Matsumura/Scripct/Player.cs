using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rb;

    [Header("移動")]
    public float moveSpeed = 10f;

    [Header("ジャンプ")]
    public float jumpForce = 5f;
    public float coyoteTime = 0.15f;     // 地面から離れても少しジャンプ可能
    public float jumpBufferTime = 0.15f; // 入力先行受付

    private float coyoteTimer;
    private float jumpBufferTimer;

    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 入力受付（ジャンプバッファ）
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        // コヨーテタイム更新
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        // ジャンプ実行
        if (jumpBufferTimer > 0 && coyoteTimer > 0)
        {
            Jump();
            jumpBufferTimer = 0;
        }
    }

    void FixedUpdate()
    {
        // 移動処理（Yは保持）
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move =
            transform.forward * v +
            transform.right * h;

        Vector3 velocity = move * moveSpeed;
        velocity.y = rb.linearVelocity.y;

        rb.linearVelocity = velocity;
    }

    void Jump()
    {
        // Y速度リセットしてからジャンプ（安定）
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        isGrounded = false;
        coyoteTimer = 0;
    }

    // タグ不要の接地判定（法線チェック）
    void OnCollisionStay(Collision collision)
    {
        isGrounded = false;

        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}