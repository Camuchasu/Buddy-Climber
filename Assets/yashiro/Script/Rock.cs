using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] private Transform m_rockPos;
    [SerializeField] private Transform m_playerPos;

    private float m_dist;

    void Start()
{
    Rigidbody rb = GetComponent<Rigidbody>();

    if (rb != null)
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}

    void Update()
    {
        m_dist = Vector3.Distance(m_rockPos.position, m_playerPos.position);

        if (m_dist >= 5.0f)
        {
            //Destroy(gameObject);
        }
    }

private void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;

            Vector3 dir = new Vector3(1f, 0.3f, 0f);

            rb.AddForce(dir * 30f, ForceMode.Impulse);
        }
    }
}
}
