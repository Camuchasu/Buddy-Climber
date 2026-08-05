using UnityEngine;
using UnityEngine.InputSystem;

public class ChainVisual : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    [Header("鎖設定")]
    public float maxDistance = 3f;
    public float minDistance = 1.5f;
    public float pullForce = 10f;

    [Header("巻き取り")]
    public float reelSpeed = 2f;

    private LineRenderer line;

    [SerializeField]
    private InputActionReference player1ReelAction;

    [SerializeField]
    private InputActionReference player2ReelAction;

    private Rigidbody rb1;
    private Rigidbody rb2;


    void Start()
    {
        line = GetComponent<LineRenderer>();

        rb1 = player1.GetComponent<Rigidbody>();
        rb2 = player2.GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        player1ReelAction.action.Enable();
        player2ReelAction.action.Enable();
    }

    void OnDisable()
    {
        player1ReelAction.action.Disable();
        player2ReelAction.action.Disable();
    }


    void Update()
    {
        // 鎖表示
        line.SetPosition(0, player1.position);
        line.SetPosition(1, player2.position);
    }


    void FixedUpdate()
    {
        Vector3 direction = player2.position - player1.position;
        float distance = direction.magnitude;


        // Player1 巻き取り
        if (player1ReelAction.action.IsPressed())
        {
            maxDistance -= reelSpeed * Time.fixedDeltaTime;
        }


        // Player2 巻き取り
       if (player2ReelAction.action.IsPressed())
        {
            maxDistance -= reelSpeed * Time.fixedDeltaTime;
        }


        // 鎖の長さ制限
        maxDistance = Mathf.Clamp(maxDistance, minDistance, 5f);


        // 一定距離以上なら引っ張る
        if (distance > maxDistance)
        {
            Vector3 pullDir = direction.normalized;

            float stretch = distance - maxDistance;

            float force = stretch * stretch * pullForce;


            // 両方を引き寄せる
            rb1.AddForce(pullDir * force * 0.5f, ForceMode.Force);
            rb2.AddForce(-pullDir * force * 0.5f, ForceMode.Force);
        }
    }
}