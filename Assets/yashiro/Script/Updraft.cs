using System.Collections;
using UnityEngine;

public class Updraft : MonoBehaviour
{
    [Header("上昇力")]
    [SerializeField] private float upForce = 20f;

    [Header("横移動")]
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private float moveRange = 10f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float x =
            Mathf.Sin(Time.time * moveSpeed)
            * moveRange;

        transform.position = new Vector3(
            startPos.x + x,
            transform.position.y,
            transform.position.z
        );
    }

   private void OnTriggerStay(Collider other)
{
    if (other.CompareTag("Player"))
    {
        Rigidbody rb =
            other.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity =
    new Vector3(
        rb.linearVelocity.x,
        upForce,
        rb.linearVelocity.z
    );
        }
    }
}
}