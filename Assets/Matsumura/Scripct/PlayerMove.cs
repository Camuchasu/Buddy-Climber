using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Layer")]
    public LayerMask groundLayer;

    [Header("Player Size")]
    public float playerHeight = 2.0f;

    [Header("Ground Check")]
    public float groundCheckRadius = 0.25f;
    public float groundCheckDistance = 0.1f;

    [Header("Ceiling Check")]
    public float ceilingCheckRadius = 0.25f;
    public float ceilingCheckDistance = 0.1f;

    private bool isGrounded;
    private bool isCeiling;

    void Update()
    {
        CheckGround();
        CheckCeiling();

        // if (isGrounded)
        // {
        //     Debug.Log("接地中");
        // }

        // if (isCeiling)
        // {
        //     Debug.Log("頭上ヒット");
        // }
    }

    void CheckGround()
{
    Vector3 origin =
        transform.position +
        Vector3.down * (playerHeight * 0.5f - groundCheckRadius);

    if (Physics.SphereCast(
        origin,
        groundCheckRadius,
        Vector3.down,
        out RaycastHit hit,
        groundCheckDistance,
        groundLayer))
    {
        // 面の向きを確認
        float angle = Vector3.Angle(hit.normal, Vector3.up);

        // 角度が小さいなら地面
        isGrounded = angle < 45f;
    }
    else
    {
        isGrounded = false;
    }
}
    void CheckCeiling()
    {
        Vector3 origin =
            transform.position +
            Vector3.up * (playerHeight * 0.5f - ceilingCheckRadius);

        isCeiling = Physics.SphereCast(
            origin,
            ceilingCheckRadius,
            Vector3.up,
            out RaycastHit hit,
            ceilingCheckDistance,
            groundLayer
        );

        Debug.DrawRay(
            origin,
            Vector3.up * ceilingCheckDistance,
            Color.blue
        );
    }
}