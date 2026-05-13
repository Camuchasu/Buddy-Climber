using UnityEngine;
//プレイヤーを操作するためのスクリプト。移動やジャンプ、風の影響などを処理します。
public class Player : MonoBehaviour
{
    public Rigidbody rb;
    private Animator anim;

    [Header("移動・重力設定")]
    public float moveSpeed = 10f;
    public float airWindMultiplier = 1.5f;

    public bool isGrounded { get; private set; }
    private bool wasGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        wasGrounded = true;
    }

    // 脳(Controller)から呼ばれる移動命令
    public void PerformMove(float h, float v)
    {
        Vector3 move = transform.forward * v + transform.right * h;
        Vector3 velocity = move * moveSpeed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }

    // 風の影響を適用
    public void ApplyWind(Vector3 direction, float power)
    {
        float multiplier = isGrounded ? 1f : airWindMultiplier;
        rb.AddForce(direction.normalized * power * multiplier, ForceMode.Force);
    }

    // ジャンプの物理実行
    public void PerformJump(float force)
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        anim.SetTrigger("Jump");
        isGrounded = false;
    }

    public void Rotate(Vector3 direction)
{
    // 入力（スティックの傾きなど）がほとんどない時は回転させない
    if (direction.sqrMagnitude > 0.01f)
    {
        // 進みたい方向（direction）を向く回転を作る
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        
        // 瞬間的に向きを変える場合
        transform.rotation = targetRotation;
        
        // もし「滑らかに」回転させたい場合はこちら（お好みで）
        // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }
}

    // 接地判定の更新とアニメーション同期
    public void UpdatePhysicsState()
    {
        if (anim == null) return;
        anim.SetBool("Grounded", isGrounded);

        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        anim.SetFloat("MoveSpeed", horizontalVelocity.magnitude);

        if (!wasGrounded && isGrounded)
        {
            anim.SetTrigger("Land");
        }
        wasGrounded = isGrounded;
    }

    // --- 接地判定ロジック ---
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
    void OnCollisionExit(Collision collision) => isGrounded = false;
}