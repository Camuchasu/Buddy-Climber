using System.Collections;
using UnityEngine;

public class CollapsingFloor : MonoBehaviour
{
    [SerializeField] private float breakDelay = 1f;
    [SerializeField] private float respawnDelay = 3f;

    [SerializeField] private GameObject floorPrefab;

    private bool isBroken = false;

    private Rigidbody rb;

    // ローカル位置保存
    private Vector3 startLocalPos;
    private Quaternion startLocalRot;

    // 親保存
    private Transform originalParent;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // 親保存
        originalParent = transform.parent;

        // ローカル位置保存
        startLocalPos = transform.localPosition;
        startLocalRot = transform.localRotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isBroken) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            isBroken = true;

            StartCoroutine(BreakRoutine());
        }
    }

    IEnumerator BreakRoutine()
    {
        yield return new WaitForSeconds(breakDelay);

        // 落下開始
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        yield return new WaitForSeconds(respawnDelay);

        // 生成
        GameObject newFloor =
            Instantiate(floorPrefab);

        // 親設定
        newFloor.transform.SetParent(originalParent);

        // ローカル座標復元
        newFloor.transform.localPosition = startLocalPos;
        newFloor.transform.localRotation = startLocalRot;

        // Rigidbody初期化
        Rigidbody newRb =
            newFloor.GetComponent<Rigidbody>();

        if (newRb != null)
        {
            newRb.isKinematic = true;

            newRb.linearVelocity = Vector3.zero;
            newRb.angularVelocity = Vector3.zero;
        }

        Destroy(gameObject);
    }
}