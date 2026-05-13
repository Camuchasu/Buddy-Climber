using UnityEngine;

public class Hook : MonoBehaviour
{
    [Header("設定")]
    public Camera playerCamera;
    public Rigidbody rb;

    // フック可能なレイヤー
    public LayerMask grappleLayer;

    // 最大距離
    public float maxDistance = 3f;

    // 引っ張る強さ
    public float pullForce = 20f;

    // フック中か
    private bool isGrappling = false;

    // フック地点
    private Vector3 grapplePoint;

    void Update()
    {
        // Eキーでフック
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryGrapple();
        }
    }

    void FixedUpdate()
    {
        // フック中なら引っ張る
        if (isGrappling)
        {
            PullPlayer();
        }
    }

    void TryGrapple()
    {
        RaycastHit hit;

        // カメラの正面にRayを飛ばす
        if (Physics.Raycast(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            out hit,
            maxDistance,
            grappleLayer))
        {
            // フック成功
            grapplePoint = hit.point;
            isGrappling = true;

            Debug.Log("フック成功");
        }
        else
        {
            // フック失敗
            Debug.Log("引っ掛かる場所がない");
        }
    }

    void PullPlayer()
    {
        // フック方向
        Vector3 direction =
            (grapplePoint - transform.position).normalized;

        // プレイヤーを引っ張る
        rb.AddForce(direction * pullForce, ForceMode.Acceleration);

        // 近づいたら終了
        float distance =
            Vector3.Distance(transform.position, grapplePoint);

        if (distance < 1f)
        {
            isGrappling = false;
        }
    }
}