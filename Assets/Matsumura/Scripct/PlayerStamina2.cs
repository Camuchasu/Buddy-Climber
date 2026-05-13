using UnityEngine;
using UnityEngine.UI; // UIを使うために必要

public class PlayerStamina2 : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float regenRate = 10f; // 1秒間に回復する量
    public float regenDelay = 2f; // 消費後、回復が始まるまでの待ち時間

    [Header("UI Reference")]
    public Slider staminaSlider; // インスペクターでUIのSliderをアタッチ

    private float regenTimer;

    void Awake()
    {
        currentStamina = maxStamina;
        UpdateUI();
    }
    void Update()
    {
        // 自動回復の処理
        if (regenTimer > 0)
        {
            regenTimer -= Time.deltaTime;
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += regenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
            UpdateUI();
        }
    }
    // --- 外部（移動スクリプトなど）から呼ぶ関数 ---

    // 1. 一時的な消費（ジャンプなど）
    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            regenTimer = regenDelay; // 回復を一時停止
            UpdateUI();
            return true; // スタミナが足りた
        }
        return false; // スタミナ不足
    }

    // 2. 持続的な消費（踏ん張る、走るなど）
    public bool ConsumeStaminaContinuous(float amountPerSecond)
    {
        if (currentStamina > 0)
        {
            currentStamina -= amountPerSecond * Time.deltaTime;
            regenTimer = regenDelay; 
            UpdateUI();
            return true;
        }
        return false;
    }

    void UpdateUI()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina / maxStamina;
        }
    }
}