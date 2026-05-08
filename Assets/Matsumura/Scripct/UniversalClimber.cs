using UnityEngine;

public class UniversalClimber : MonoBehaviour
{
    public Player motor;            // 体の動き担当
    public PlayerStamina stamina;    // スタミナ管理担当
    private WindSystem wind;

    [Header("移動設定")]
    public float walkSpeed = 10f;
    
    [Header("ジャンプ設定")]
    public float jumpForce = 12f;
    public float coyoteTime = 0.3f;
    public float jumpBufferTime = 0.3f;
    private float coyoteTimer;
    private float jumpBufferTimer;

    [Header("スタミナ消費量")]
    public float jumpCost = 20f;
    public float grabCost = 10f; // 掴み（秒間）
    public float linkCost = 15f; // 連結・踏ん張り（秒間）

    void Start()
    {
        wind = FindObjectOfType<WindSystem>();
    }

    void Update()
    {
        // 1. 入力の受付（PC/コントローラー共通）
        UpdateJumpInput();

        // 2. ジャンプ判定
        if (jumpBufferTimer > 0 && coyoteTimer > 0)
        {
            // スタミナがあればジャンプ実行
            if (stamina != null && stamina.UseStamina(jumpCost))
            {
                motor.PerformJump(jumpForce);
                jumpBufferTimer = 0;
            }
        }

        // 3. アクション判定（掴み・連結）
        HandleStaminaActions();

        // 4. アニメーションや接地状態の更新をMotorに依頼
        motor.UpdatePhysicsState();
    }

    void FixedUpdate()
    {
        // 5. 移動の実行
        // Input.GetAxisはPCのWASDとコントローラーのスティック両方に反応します
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v);

        // スタミナがある時だけ通常移動
        if (stamina == null || stamina.currentStamina > 0)
        {
            motor.PerformMove(h, v);
            if (inputDir.magnitude > 0.1f) motor.Rotate(inputDir);
        }

        // 6. 風の影響
        if (wind != null && wind.IsBlowing)
        {
            motor.ApplyWind(wind.windDirection, wind.windPower);
        }
    }

    void UpdateJumpInput()
    {
        // PCのSpaceキーまたはコントローラーのAボタン(Joystick Button 0など)に反応
        if (Input.GetButtonDown("Jump")) jumpBufferTimer = jumpBufferTime;
        else jumpBufferTimer -= Time.deltaTime;

        if (motor.isGrounded) coyoteTimer = coyoteTime;
        else coyoteTimer -= Time.deltaTime;
    }

    void HandleStaminaActions()
    {
        // --- 掴み (Grab) ---
        if (Input.GetButton("Grab"))
        {
            if (stamina.ConsumeStaminaContinuous(grabCost))
            {
                // ここに掴み中の物理制限などを追加
            }
        }

        // --- 連結・踏ん張り (Link) ---
        if (Input.GetButton("Link"))
        {
            if (stamina.ConsumeStaminaContinuous(linkCost))
            {
                // ここに風の影響を減らすなどの処理を追加
            }
        }
    }
}