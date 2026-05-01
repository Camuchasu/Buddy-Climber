using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandumScaffold : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject normalPrefab;
    [SerializeField] private GameObject breakablePrefab;

    [Header("範囲")]
    [SerializeField] private Vector3 minPosition;
    [SerializeField] private Vector3 maxPosition;

    [Header("生成設定")]
    [SerializeField] private int spawnCount = 10;
    [SerializeField] private float minDistance = 2.5f; // ←ここ少し大きめ推奨

    [Header("高さトリガー")]
    [SerializeField] private Transform player;
    [SerializeField] private float heightInterval = 10f;

    [Header("崩れる床の割合")]
    [Range(0f, 1f)]
    [SerializeField] private float breakableRate = 0.3f;

    // 👇 今までの全部を保持
    private List<Vector3> allPositions = new List<Vector3>();

    private float nextSpawnHeight = 0f;

    void Start()
    {
        nextSpawnHeight = heightInterval;
        SpawnObjects(player.position.y);
    }

    void Update()
    {
        if (player.position.y > nextSpawnHeight)
        {
            SpawnObjects(nextSpawnHeight);
            nextSpawnHeight += heightInterval;
        }
    }

    void SpawnObjects(float baseHeight)
    {
        int attempts = 0;
        int created = 0;

        while (created < spawnCount && attempts < 2000)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(minPosition.x, maxPosition.x),
                baseHeight + Random.Range(minPosition.y, maxPosition.y),
                Random.Range(minPosition.z, maxPosition.z)
            );

            if (IsFarEnoughFromAll(randomPosition))
            {
                GameObject prefabToSpawn = 
                    (Random.value < breakableRate) ? breakablePrefab : normalPrefab;

                GameObject Dummy = Instantiate(prefabToSpawn, randomPosition, Quaternion.identity);
                Dummy.transform.parent = transform;

                allPositions.Add(randomPosition);
                created++;
            }

            attempts++;
        }
    }

    bool IsFarEnoughFromAll(Vector3 position)
    {
        foreach (Vector3 pos in allPositions)
        {
            if (Vector3.Distance(pos, position) < minDistance)
            {
                return false;
            }
        }
        return true;
    }
}