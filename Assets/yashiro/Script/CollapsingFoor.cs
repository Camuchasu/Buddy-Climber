using System.Collections;
using UnityEngine;

public class CollapsingFoor : MonoBehaviour
{
     [Header("設定")]
    public float breakDelay = 1.0f;   // 乗ってから崩れるまでの時間
    public float destroyDelay = 2.0f; // 落ちてから消えるまで

    [Header("演出")]
    public bool shakeBeforeBreak = true;
    public float shakePower = 0.05f;

    private bool isTriggered = false;
    private Rigidbody rb;
    private Vector3 startPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;

        // 最初は固定しておく
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isTriggered) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            isTriggered = true;
            StartCoroutine(BreakRoutine());
        }
    }

    IEnumerator BreakRoutine()
    {
        float timer = 0f;

        // 揺れる演出（予兆）
        while (timer < breakDelay)
        {
            if (shakeBeforeBreak)
            {
                transform.position = startPos + Random.insideUnitSphere * shakePower;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = startPos;

        // 落下開始
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // 一定時間後に削除
        Destroy(gameObject, destroyDelay);
    }
}
