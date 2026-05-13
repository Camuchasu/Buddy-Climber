using System.Collections;
using UnityEngine;

public class CollapsingFoor : MonoBehaviour
{
    [Header("設定")]
    public float breakDelay = 1f;
    public float destroyDelay = 2f;

    [Header("発動高さ")]
    public float activeHeight = 20f;

    [Header("プレイヤー")]
    public Transform player;

    [Header("揺れ")]
    public bool shakeBeforeBreak = true;
    public float shakePower = 0.05f;

    private bool isTriggered = false;
    private bool isActive = false;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
    }

    void Update()
    {
        if(!player)
        {
            GameObject dummy = GameObject.Find("Player");
            if(dummy)
            {
                player = dummy.transform;
            }
        }
        else
        {
            if (!isActive && player.position.y >= activeHeight)
            {
                isActive = true;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isActive) return;

        if (isTriggered) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            isTriggered = true;

            StartCoroutine(BreakRoutine());
        }
    }

    IEnumerator BreakRoutine()
    {
        Vector3 originalPos = transform.position;

        float timer = 0f;

        while (timer < breakDelay)
        {
            if (shakeBeforeBreak)
            {
                transform.position =
                    originalPos +
                    Random.insideUnitSphere * shakePower;
            }

            timer += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPos;

        rb.isKinematic = false;

        Destroy(gameObject, destroyDelay);
    }
}
