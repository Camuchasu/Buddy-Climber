using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBase : MonoBehaviour
{
    [Header("プレイヤー本体")]
    public Player player;

    [Header("スタミナ")]
    public PlayerStamina2 stamina;

    [Header("Input")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;


    [Header("ジャンプ設定")]
    public float jumpStaminaCost = 20f;


    private WindSystem wind;



    void Awake()
    {
        if(player == null)
        {
            player = GetComponent<Player>();
        }
    }



    void Start()
    {
        wind = FindObjectOfType<WindSystem>();
    }



    void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }



    void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }




    void Update()
    {
        // ジャンプ入力
        if(jumpAction.action.WasPressedThisFrame())
        {
            TryJump();
        }


        player.UpdatePhysicsState();
    }




    void FixedUpdate()
    {
        // 移動入力
        Vector2 input =
            moveAction.action.ReadValue<Vector2>();


        float h = input.x;
        float v = input.y;



        // スタミナがある時だけ移動
        if(stamina == null ||
           stamina.currentStamina > 0)
        {
            player.PerformMove(h, v);
        }



        // 風
        if(wind != null &&
           wind.IsBlowing)
        {
            player.ApplyWind(
                wind.windDirection,
                wind.windPower
            );
        }
    }





    void TryJump()
    {
        if(player == null)
            return;


        if(!player.isGrounded)
            return;



        if(stamina == null)
        {
            player.PerformJump();
            return;
        }



        if(stamina.UseStamina(jumpStaminaCost))
        {
            player.PerformJump();
        }
    }
}