using UnityEngine;

public class Hook : MonoBehaviour
{
    [Header("プレイヤー設定")]
    public Rigidbody rb;

    [Header("見た目モデル")]
    public Transform modelTransform;

    [Header("ライン表示")]
    public LineRenderer lineRenderer;

    [Header("フック可能レイヤー")]
    public LayerMask grappleLayer;

    [Header("最大距離")]
    public float maxDistance = 20f;

    [Header("引っ張る速度")]
    public float pullForce = 25f;

    [Header("停止距離")]
    public float stopDistance = 2f;

    [Header("角度変更速度")]
    public float rotateSpeed = 30f;

    [Header("最大角度")]
    public float angleRange = 60f;

    // フック中
    private bool isGrappling = false;

    // 狙い中
    private bool isCharging = false;

    // 現在角度
    private float currentAngle = 0f;

    // 角度の進行方向
    private bool angleForward = true;

    // フック地点
    private Vector3 grapplePoint;

    void Start()
    {
        // 最初は取得していない状態
        enabled = false;

        // ライン非表示
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    void Update()
    {
        HandleInput();

        // 狙い中
        if (isCharging)
        {
            RotateAim();
            DrawAimRay();
        }

        // フック中
        if (isGrappling)
        {
            DrawGrappleLine();
        }

        // 非表示処理
        if (!isCharging && !isGrappling)
        {
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (isGrappling)
        {
            PullPlayer();
        }
    }

    void HandleInput()
    {
        // E押した瞬間
        if (Input.GetKeyDown(KeyCode.E))
        {
            isCharging = true;

            // 角度リセット
            currentAngle = 0f;

            // 最初は上方向
            angleForward = true;
        }

        // E離した瞬間
        if (Input.GetKeyUp(KeyCode.E))
        {
            isCharging = false;

            TryGrapple();
        }
    }

    void RotateAim()
    {
        // 上方向
        if (angleForward)
        {
            currentAngle += rotateSpeed * Time.deltaTime;

            // 最大角度
            if (currentAngle >= angleRange)
            {
                currentAngle = angleRange;

                angleForward = false;
            }
        }
        // 下方向
        else
        {
            currentAngle -= rotateSpeed * Time.deltaTime;

            // 0まで戻った
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

        // プレイヤー基準で上方向回転
        Vector3 direction =
            Quaternion.AngleAxis(
                -currentAngle,
                modelTransform.right
            ) * modelTransform.forward;

        Vector3 endPoint =
            transform.position + direction * maxDistance;

        // ライン表示
        lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);

        // Sceneビュー用
        Debug.DrawRay(
            transform.position,
            direction * maxDistance,
            Color.red
        );
    }

    void TryGrapple()
    {
        if (modelTransform == null)
            return;

        // プレイヤー基準で上方向回転
        Vector3 direction =
            Quaternion.AngleAxis(
                -currentAngle,
                modelTransform.right
            ) * modelTransform.forward;

        RaycastHit hit;

        if (Physics.Raycast(
            transform.position,
            direction,
            out hit,
            maxDistance,
            grappleLayer))
        {
            grapplePoint = hit.point;

            isGrappling = true;

            Debug.Log("フック成功");
        }
        else
        {
            Debug.Log("引っ掛かる場所がない");
        }
    }

    void PullPlayer()
    {
        Vector3 direction =
            (grapplePoint - transform.position).normalized;

        // プレイヤー移動
        rb.linearVelocity = direction * pullForce;

        float distance =
            Vector3.Distance(transform.position, grapplePoint);

        // 到達したら終了
        if (distance < stopDistance)
        {
            StopGrapple();
        }
    }

    void DrawGrappleLine()
    {
        if (lineRenderer == null)
            return;

        lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }

    void StopGrapple()
    {
        isGrappling = false;

        rb.linearVelocity = Vector3.zero;

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }
}