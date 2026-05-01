using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    private Animator anim; 

    [Header("移動")]
    public float moveSpeed = 10f;

    [Header("ジャンプ")]
    public float jumpForce = 10f;
    public float coyoteTime = 0.30f;
    public float jumpBufferTime = 0.30f;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool isGrounded;
    private bool wasGrounded; // ★前フレームの接地状態を記録

    private WindSystem wind;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>(); 
        wind = FindObjectOfType<WindSystem>();
        wasGrounded = true;
    }

    void Update()
    {
        // ジャンプ入力受付
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

        UpdateAnimation();
        
        // ★着地の判定
        if (!wasGrounded && isGrounded)
        {
            OnLand();
        }
        wasGrounded = isGrounded;
    }

    void UpdateAnimation()
    {
        if (anim == null) return;

        // Grounded: 常に接地状態を同期（空中ならMidairへ行くための基本条件）
        anim.SetBool("Grounded", isGrounded);

        // MoveSpeed: 移動アニメーションのブレンド用
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        anim.SetFloat("MoveSpeed", horizontalVelocity.magnitude);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        isGrounded = false;
        coyoteTimer = 0;

        // ★ジャンプのアニメーションを即座に再生
        anim.SetTrigger("Jump"); 
    }

    void OnLand()
    {
        // ★着地のアニメーションを再生
        anim.SetTrigger("Land");
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.forward * v + transform.right * h;
        Vector3 velocity = move * moveSpeed;
        velocity.y = rb.linearVelocity.y;

        rb.linearVelocity = velocity;

        if (wind != null && wind.IsBlowing)
        {
            float multiplier = isGrounded ? 1f : airWindMultiplier;
            rb.AddForce(wind.windDirection.normalized * wind.windPower * multiplier, ForceMode.Force);
        }
    }

    [Header("風の影響")]
    public float airWindMultiplier = 1.5f;

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