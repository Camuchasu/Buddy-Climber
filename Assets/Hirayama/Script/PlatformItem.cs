using UnityEngine;

public class PlatformItem : MonoBehaviour
{
    [Header("生成する足場")]
    public GameObject platformPrefab;

    [Header("生成位置オフセット")]
    public float spawnOffsetY = -2f;

    void Update()
    {
        // Qキー使用
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnPlatform();
        }
    }

    void SpawnPlatform()
    {
        // プレイヤー真下
        Vector3 spawnPos =
            transform.position
            + Vector3.up * spawnOffsetY;

        Instantiate(
            platformPrefab,
            spawnPos,
            Quaternion.identity
        );
    }
}