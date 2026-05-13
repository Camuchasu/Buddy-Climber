using UnityEngine;
using System.Collections;

public class CollapsingFloor : MonoBehaviour
{
    [Header("設定")]
    public float breakDelay = 1f;
    public float respawnDelay = 3f;

    [Header("Prefab")]
    public GameObject floorPrefab;

    [Header("発動高さ")]
    public float activeHeight = 20f;

    [Header("プレイヤー")]
    public Transform player;

    [Header("揺れ")]
    public bool shakeBeforeBreak = true;
    public float shakePower = 0.05f;

    private bool isTriggered = false;
    private bool isActive = false;

    void Update()
    {
        if (!player)
        {
            GameObject dummy = GameObject.Find("Player");
            if (dummy) player = dummy.transform;
        }
        else
        {
            if (!isActive && player.position.y >= activeHeight)
                isActive = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isActive || isTriggered) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            isTriggered = true;
            StartCoroutine(BreakAndRespawn());
        }
    }

    IEnumerator BreakAndRespawn()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        float timer = 0f;

        // 揺れ
        while (timer < breakDelay)
        {
            if (shakeBeforeBreak)
            {
                transform.position = pos + Random.insideUnitSphere * shakePower;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = pos;

        // ここで床を消す
        Destroy(gameObject);

        // 復活待ち
        yield return new WaitForSeconds(respawnDelay);

        // Prefabで再生成
        Debug.Log("復活処理開始");
        Instantiate(floorPrefab, pos, rot);
    }
}