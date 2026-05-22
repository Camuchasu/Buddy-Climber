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

    private SpringJoint joint;

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
    }

    void HandleInput()
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

        // Spaceで解除
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopGrapple();
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

        if (Physics.Raycast(
            hookOrigin.position,
            direction,
            out hit,
            maxDistance,
            grappleLayer))
        {
            grapplePoint = hit.point;

            isGrappling = true;

            // 少し前へ勢いをつける
            rb.AddForce(
                direction * 5f,
                ForceMode.VelocityChange
            );

            joint = rb.gameObject.AddComponent<SpringJoint>();

            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distance =
                Vector3.Distance(
                    transform.position,
                    grapplePoint
                );

            joint.maxDistance = distance;
            joint.minDistance = distance * 0.8f;

            joint.spring = 0.5f;
            joint.damper = 0.1f;
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

        if (joint != null)
        {
            Destroy(joint);
        }

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