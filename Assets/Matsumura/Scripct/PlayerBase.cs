using System.Collections.Generic;
using UnityEngine;
//プレイヤーの基本的な機能をまとめたクラス。移動、ジャンプ、スタミナ管理、風の影響などを統括します。
public class PlayerBase : CharaBase
{
 public Player motor;
    public PlayerStamina2 stamina; // 先ほど作成したスタミナスクリプト
    private WindSystem wind;

    [Header("ジャンプ設定")]
    public float jumpForce = 10f;
    public float coyoteTime = 0.30f;
    public float jumpBufferTime = 0.30f;

    private float coyoteTimer;
    private float jumpBufferTimer;

    [Header("ジャンプキー")]
    public KeyCode jumpKey = KeyCode.Space;

    void Start()
    {
        wind = FindObjectOfType<WindSystem>();
    }

    void Update()
    {
        // 1. 入力受付
        UpdateTimers();

        // 2. ジャンプ判断（スタミナ消費と連動）
        if (jumpBufferTimer > 0 && coyoteTimer > 0)
        {
            // ジャンプ時にスタミナを20消費する例
            if (stamina != null && stamina.UseStamina(20f))
            {
                motor.PerformJump(jumpForce);
                jumpBufferTimer = 0;
            }
        }

        motor.UpdatePhysicsState();
    }

    void FixedUpdate()
    {
        // 3. 移動命令
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // スタミナがある時だけ動ける（あるいはスタミナ切れで鈍足にするなど）
        if (stamina == null || stamina.currentStamina > 0)
        {
            motor.PerformMove(h, v);
        }

        // 4. 風の影響
        if (wind != null && wind.IsBlowing)
        {
            motor.ApplyWind(wind.windDirection, wind.windPower);
        }
    }

    void UpdateTimers()
    {
        if (Input.GetKeyDown(jumpKey))
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;

        if (motor.isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;
}
}

