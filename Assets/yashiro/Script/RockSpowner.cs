using UnityEngine;

public class RockSpowner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;   // 落とすオブジェクト
    [SerializeField] private float spawnInterval = 1.0f; // 出現間隔
    [SerializeField] private Vector3 minPos; // 最小のランダム範囲
    [SerializeField] private Vector3 maxPos; // 最大のランダム範囲
    void Start()
    {
        InvokeRepeating("SpawnObject", 1f, spawnInterval);
    }

    void SpawnObject()
    {
        Vector3 randomPosition = new Vector3
            (Random.Range(minPos.x, maxPos.x),
                Random.Range(minPos.y, maxPos.y),
                Random.Range(minPos.z, maxPos.z)
            );

        Instantiate(prefab, randomPosition, Quaternion.identity);
    }
}
