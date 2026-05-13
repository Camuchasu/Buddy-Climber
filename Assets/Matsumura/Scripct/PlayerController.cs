using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;      // 移動速度
    private Animator anim;
    private Rigidbody rb;

    void Start()
    {
        // コンポーネントの取得
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Rigidbodyの回転を固定（倒れないようにする）
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        // キーボード入力の取得 (WASDや矢印キー)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 移動方向の計算
        Vector3 moveDir = new Vector3(h, 0, v).normalized;

        // 移動処理
        if (moveDir.magnitude >= 0.1f)
        {
            // 移動する
            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
            
            // 進行方向を向く
            transform.rotation = Quaternion.LookRotation(moveDir);
        }

        // --- Animatorへの値の受け渡し ---
        // 入力の強さをMoveSpeedに渡す（0なら待機、1なら走り）
        anim.SetFloat("MoveSpeed", moveDir.magnitude);

        // 常に地面にいる設定にする（ジャンプを実装するまで）
        anim.SetBool("Grounded", true);
    }
}
