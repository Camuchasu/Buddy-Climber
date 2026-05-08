using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("スタミナ")]
    public float maxStamina = 100f;
    public float currentStamina;

    public float staminaDrain = 20f;
    public float staminaRecovery = 15f;

    [Header("息切れ設定")]
    public float recoveryDelay = 2f;

    private bool isExhausted = false;
    private bool canSprint = true;

    private float recoveryTimer;

    [Header("移動速度")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;

    public float currentSpeed;

    [Header("UI")]
    public Image warningImage;

    [Header("SE")]
    public AudioSource recoverySE;

    void Start()
    {
        currentStamina = maxStamina;
        currentSpeed = walkSpeed;

        // 赤警告を透明に
        if (warningImage != null)
        {
            Color c = warningImage.color;
            c.a = 0f;
            warningImage.color = c;
        }
    }

    void Update()
    {
        HandleSprint();
        HandleRecovery();
        HandleWarningUI();

        // スタミナ制限
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    void HandleSprint()
    {
        // ダッシュ条件
        if (Input.GetKey(KeyCode.LeftShift) &&
            currentStamina > 0 &&
            canSprint)
        {
            currentSpeed = sprintSpeed;

            // スタミナ減少
            currentStamina -= staminaDrain * Time.deltaTime;

            // スタミナ切れ
            if (currentStamina <= 0)
            {
                currentStamina = 0;

                isExhausted = true;
                canSprint = false;

                recoveryTimer = recoveryDelay;

                currentSpeed = walkSpeed;
            }
        }
        else
        {
            currentSpeed = walkSpeed;
        }
    }

    void HandleRecovery()
    {
        // 息切れ中
        if (isExhausted)
        {
            recoveryTimer -= Time.deltaTime;

            // 回復開始
            if (recoveryTimer <= 0)
            {
                isExhausted = false;

                // 回復開始SE
                if (recoverySE != null)
                {
                    recoverySE.Play();
                }
            }
        }
        else
        {
            // スタミナ回復
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRecovery * Time.deltaTime;
            }

            // 完全回復で再びダッシュ可能
            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
                canSprint = true;
            }
        }
    }

    void HandleWarningUI()
    {
        if (warningImage == null)
            return;

        Color c = warningImage.color;

        // 息切れ中は赤点滅
        if (isExhausted)
        {
            c.a = Mathf.PingPong(Time.time * 2f, 0.7f);
        }
        else
        {
            c.a = 0f;
        }

        warningImage.color = c;
    }
}