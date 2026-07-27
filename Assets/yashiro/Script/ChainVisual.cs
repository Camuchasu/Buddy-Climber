using UnityEngine;

public class ChainVisual : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    public float pullForce = 10f;
    public float maxDistance = 3f;

    private LineRenderer line;

    private Rigidbody rb1;
    private Rigidbody rb2;

    [Header("ロープ巻き取り速度")]
    public float reelSpeed = 2f;


    void Start()
    {
        line = GetComponent<LineRenderer>();

        rb1 = player1.GetComponent<Rigidbody>();
        rb2 = player2.GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        // 距離計算
        Vector3 direction = player2.position - player1.position;
        float distance = direction.magnitude;


        // 線表示
        line.SetPosition(0, player1.position);
        line.SetPosition(1, player2.position);


        // 一定距離以上離れたら引っ張る
        if (distance > maxDistance)
        {
            Vector3 pullDir = direction.normalized;

            float stretch = distance - maxDistance;
            float force = stretch * pullForce;

            // Player1が巻き取り
            if (Input.GetKey(KeyCode.V))
            {
                rb1.AddForce(pullDir * force, ForceMode.Force);
            }

            // Player2が巻き取り
            if (Input.GetKey(KeyCode.Keypad0))
            {
                rb2.AddForce(-pullDir * force, ForceMode.Force);
            }
        }
    }
}