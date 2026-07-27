using UnityEngine;

public class Hook : MonoBehaviour
{
    [Header("プレイヤー")]
    public Rigidbody rb;

    [Header("プレイヤーモデル")]
    public Transform modelTransform;

    [Header("フック発射位置")]
    public Transform hookOrigin;

    [Header("ロープ表示")]
    public LineRenderer lineRenderer;

    [Header("フック可能レイヤー")]
    public LayerMask grappleLayer;

    [Header("最大距離")]
    public float maxDistance = 20f;

    [Header("エイム回転速度")]
    public float rotateSpeed = 60f;

    [Header("エイム角度範囲")]
    public float angleRange = 60f;

    [Header("ロープの強さ")]
    public float springPower = 20f;

    [Header("ロープの減衰")]
    public float damperPower = 5f;

    [Header("巻き取り速度")]
    public float reelSpeed = 5f;

    // フック中
    private bool isGrappling = false;

    // エイム中
    private bool isCharging = false;

    // 現在角度
    private float currentAngle = 0f;

    // 往復用
    private bool angleForward = true;

    // フック地点
    private Vector3 grapplePoint;

    // Joint
    private SpringJoint joint;

    void Start()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    void Update()
    {
        HandleInput();

        if (isCharging)
        {
            RotateAim();
            DrawAimRay();
        }

        if (isGrappling)
        {
            DrawGrappleLine();

            // 左Shiftを押している間ロープを巻き取る
            if (Input.GetKey(KeyCode.LeftShift) && joint != null)
            {
                joint.maxDistance -= reelSpeed * Time.deltaTime;

                // 最低1mまでは縮められる
                joint.maxDistance = Mathf.Max(joint.maxDistance, 1f);
            }
        }

        if (!isCharging && !isGrappling)
        {
            if (lineRenderer != null)
                lineRenderer.enabled = false;
        }
    }

    void HandleInput()
    {
        // フック中はE無効
        if (!isGrappling)
        {
            // E押した瞬間
            if (Input.GetKeyDown(KeyCode.E))
            {
                isCharging = true;

                currentAngle = 0f;

                angleForward = true;
            }

            // E離した瞬間
            if (Input.GetKeyUp(KeyCode.E))
            {
                isCharging = false;

                TryGrapple();
            }
        }

        // Spaceで解除
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopGrapple();
        }
    }

    void RotateAim()
    {
        if (angleForward)
        {
            currentAngle += rotateSpeed * Time.deltaTime;

            if (currentAngle >= angleRange)
            {
                currentAngle = angleRange;

                angleForward = false;
            }
        }
        else
        {
            currentAngle -= rotateSpeed * Time.deltaTime;

            if (currentAngle <= 0f)
            {
                currentAngle = 0f;

                angleForward = true;
            }
        }
    }

    void DrawAimRay()
    {
        if (lineRenderer == null || modelTransform == null)
            return;

        Vector3 direction =
            Quaternion.AngleAxis(
                -currentAngle,
                modelTransform.right
            ) * modelTransform.forward;

        Vector3 endPoint =
            hookOrigin.position + direction * maxDistance;

        lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, hookOrigin.position);
        lineRenderer.SetPosition(1, endPoint);

        Debug.DrawRay(
            hookOrigin.position,
            direction * maxDistance,
            Color.red
        );
    }

    void TryGrapple()
    {
        // 古いJoint削除
        if (joint != null)
        {
            Destroy(joint);
        }

        if (modelTransform == null)
            return;

        Vector3 direction =
            Quaternion.AngleAxis(
                -currentAngle,
                modelTransform.right
            ) * modelTransform.forward;

        RaycastHit hit;

        // レイキャスト
        if (Physics.Raycast(
            hookOrigin.position,
            direction,
            out hit,
            maxDistance,
            grappleLayer))
        {
            grapplePoint = hit.point;

            isGrappling = true;

            // Joint追加
            joint = rb.gameObject.AddComponent<SpringJoint>();

            // ワールド座標使用
            joint.connectedBody = null;

            // 自動設定OFF
            joint.autoConfigureConnectedAnchor = false;

            // プレイヤー中心
            joint.anchor = Vector3.zero;

            // フック地点
            joint.connectedAnchor = grapplePoint;

            // Rigidbody基準距離
            float distance =
                Vector3.Distance(
                    rb.position,
                    grapplePoint
                );

            // ロープ長を短くして引っ張る
            joint.maxDistance = distance * 0.5f;
            joint.minDistance = 0f;

            // 引っ張る力
            joint.spring = springPower;
            joint.damper = damperPower;
            joint.massScale = 1f;

            Debug.Log("フック成功");
        }
        else
        {
            Debug.Log("引っ掛かる場所がない");
        }
    }

    void DrawGrappleLine()
    {
        if (lineRenderer == null)
            return;

        lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, hookOrigin.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }

    void StopGrapple()
    {
        isGrappling = false;

        // Joint削除
        if (joint != null)
        {
            Destroy(joint);
        }

        // ロープ非表示
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    public bool IsGrappling()
    {
        return isGrappling;
    }
}