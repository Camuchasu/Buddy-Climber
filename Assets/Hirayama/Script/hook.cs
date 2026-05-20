using UnityEngine;

public class Hook : MonoBehaviour
{
   // [Header("�v���C���[�ݒ�")]
    public Rigidbody rb;

   // [Header("�����ڃ��f��")]
    public Transform modelTransform;

    //[Header("�t�b�N���ˈʒu")]
    public Transform hookOrigin;

   // [Header("���C���\��")]
    public LineRenderer lineRenderer;

   // [Header("�t�b�N�\���C���[")]
    public LayerMask grappleLayer;

   // [Header("�ő勗��")]
    public float maxDistance = 20f;

   // [Header("�������鑬�x")]
    public float pullForce = 25f;

   // [Header("��~����")]
    public float stopDistance = 2f;

   // [Header("�p�x�ύX���x")]
    public float rotateSpeed = 30f;

   // [Header("�ő�p�x")]
    public float angleRange = 60f;

    // �t�b�N��
    private bool isGrappling = false;

    // �_����
    private bool isCharging = false;

    // ���݊p�x
    private float currentAngle = 0f;

    // �p�x�̐i�s����
    private bool angleForward = true;

    // �t�b�N�n�_
    private Vector3 grapplePoint;

    void Start()
    {
        // �ŏ��͎擾���Ă��Ȃ����
        //enabled = false;

        // ���C����\��
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    void Update()
    {
        HandleInput();

        // �_����
        if (isCharging)
        {
            RotateAim();
            DrawAimRay();
        }

        // �t�b�N��
        if (isGrappling)
        {
            DrawGrappleLine();
        }

        // ��\������
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
        // E�������u��
        if (Input.GetKeyDown(KeyCode.E))
        {
            isCharging = true;

            // �p�x���Z�b�g
            currentAngle = 0f;

            // �ŏ��͏����
            angleForward = true;
        }

        // E�������u��
        if (Input.GetKeyUp(KeyCode.E))
        {
            isCharging = false;

            TryGrapple();
        }
    }

    void RotateAim()
    {
        // �����
        if (angleForward)
        {
            currentAngle += rotateSpeed * Time.deltaTime;

            // �ő�p�x
            if (currentAngle >= angleRange)
            {
                currentAngle = angleRange;

                angleForward = false;
            }
        }
        // ������
        else
        {
            currentAngle -= rotateSpeed * Time.deltaTime;

            // 0�܂Ŗ߂���
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

        // �v���C���[��ŏ������]
        Vector3 direction =
            Quaternion.AngleAxis(
                -currentAngle,
                modelTransform.right
            ) * modelTransform.forward;

        Vector3 endPoint =
        hookOrigin.position + direction * maxDistance;

        // ���C���\��
        lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, hookOrigin.position);
        lineRenderer.SetPosition(1, endPoint);

        // Scene�r���[�p
        Debug.DrawRay(
        hookOrigin.position,
            direction * maxDistance,
            Color.red
        );
    }

    void TryGrapple()
    {
        if (modelTransform == null)
            return;

        // �v���C���[��ŏ������]
        Vector3 direction =
            Quaternion.AngleAxis(
                -currentAngle,
                modelTransform.right
            ) * modelTransform.forward;

        RaycastHit hit;

        if (Physics.Raycast(
            hookOrigin.position,
            direction,
            out hit,
            maxDistance,
            grappleLayer))
        {
            grapplePoint = hit.point;

            isGrappling = true;

           // Debug.Log("�t�b�N����");
        }
        else
        {
           // Debug.Log("�����|����ꏊ���Ȃ�");
        }
    }

    void PullPlayer()
    {
        Vector3 direction =
            (grapplePoint - transform.position).normalized;

        rb.AddForce(direction * pullForce, ForceMode.Acceleration);

        float distance =
            Vector3.Distance(transform.position, grapplePoint);

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

        lineRenderer.SetPosition(0, hookOrigin.position);
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