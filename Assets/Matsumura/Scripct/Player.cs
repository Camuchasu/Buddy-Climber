using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 10f;

    [Header("ジャンプ設定")]
    public float jumpForce = 10f;

    [Header("風設定")]
    public float airWindMultiplier = 1.5f;

    [Header("氷設定")]
    public float iceMoveMultiplier = 0.6f;
    public float iceDamping = 0.05f;

    private float defaultMoveSpeed;
    private float defaultDamping;

    private Rigidbody rb;
    private Animator anim;

    public bool isGrounded { get; private set; }

    private bool wasGrounded;
    private bool isIce;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        rb.freezeRotation = true;

        defaultMoveSpeed = moveSpeed;
        defaultDamping = rb.linearDamping;

        wasGrounded = true;
    }


    // =========================
    // 移動
    // =========================
    public void PerformMove(float h, float v)
    {
        Hook hook = GetComponentInChildren<Hook>();

        if (hook != null && hook.IsGrappling())
        {
            return;
        }


        Vector3 move =
            transform.forward * v +
            transform.right * h;


        float speed = moveSpeed;

        if (isIce)
        {
            speed *= iceMoveMultiplier;
        }


        Vector3 velocity = move * speed;

        velocity.y = rb.linearVelocity.y;

        rb.linearVelocity = velocity;


        if (move.sqrMagnitude > 0.01f)
        {
            Rotate(move);
        }
    }



    // =========================
    // ジャンプ
    // =========================
    public void PerformJump()
    {
        if (!isGrounded)
            return;


        rb.linearVelocity =
            new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );


        rb.AddForce(
            Vector3.up * jumpForce,
            ForceMode.Impulse
        );


        if(anim != null)
        {
            anim.SetTrigger("Jump");
        }


        isGrounded = false;
    }



    // =========================
    // 回転
    // =========================
    void Rotate(Vector3 direction)
    {
        Quaternion target =
            Quaternion.LookRotation(direction);


        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                target,
                Time.deltaTime * 10f
            );
    }



    // =========================
    // 風
    // =========================
    public void ApplyWind(Vector3 direction, float power)
    {
        float multiplier =
            isGrounded ? 1f : airWindMultiplier;


        rb.AddForce(
            direction.normalized *
            power *
            multiplier,
            ForceMode.Force
        );
    }



    // =========================
    // 氷
    // =========================
    public void SetIceState(bool value)
    {
        isIce = value;


        if(value)
        {
            rb.linearDamping = iceDamping;
        }
        else
        {
            rb.linearDamping = defaultDamping;
        }
    }



    // =========================
    // Animator更新
    // =========================
    public void UpdatePhysicsState()
    {
        if(anim == null)
            return;


        anim.SetBool(
            "Grounded",
            isGrounded
        );


        Vector3 velocity =
            rb.linearVelocity;


        velocity.y = 0;


        anim.SetFloat(
            "MoveSpeed",
            velocity.magnitude
        );


        if(!wasGrounded && isGrounded)
        {
            anim.SetTrigger("Land");
        }


        wasGrounded = isGrounded;
    }



    // =========================
    // 接地判定
    // =========================
    void OnCollisionStay(Collision collision)
    {
        foreach(ContactPoint contact in collision.contacts)
        {
            if(contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }


        isGrounded = false;
    }


    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}