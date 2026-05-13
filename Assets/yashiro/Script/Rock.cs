using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] private Transform m_rockPos;

    private float m_dist;

    void Start()
{
    Rigidbody rb = GetComponent<Rigidbody>();

    if (rb != null)
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

      Destroy(gameObject, 2f);
}

    void Update()
    {

      
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
