using UnityEngine;

public class WindArea : MonoBehaviour
{
    [Header("風システム")]
    [SerializeField] private WindSystem windSystem;

    [Header("追従対象")]
    [SerializeField] private Transform player;

    [Header("風範囲")]
    [SerializeField]
    private Vector3 boxSize =
        new Vector3(30f, 30f, 30f);

    [Header("風設定")]
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private Vector3 windDirection =
        Vector3.left;

    void Update()
    {
        // プレイヤーに追従
        transform.position = player.position;

        Collider[] hits = Physics.OverlapBox(
            transform.position,
            boxSize / 2
        );

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Floor"))
            {
                Rigidbody rb =
                    hit.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.sleepThreshold = 0;

                    Vector3 velocity =
                        rb.linearVelocity;

                    if (windSystem.IsBlowing)
                    {
                        rb.WakeUp();

                        velocity.x =
                            windDirection.normalized.x
                            * moveSpeed;
                    }
                    else
                    {
                        velocity.x = 0f;
                    }

                    rb.linearVelocity = velocity;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireCube(
            transform.position,
            boxSize
        );
    }
}