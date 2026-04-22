using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandumScaffold : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    [SerializeField] private Vector3 minPosition;
    [SerializeField] private Vector3 maxPosition;

    [SerializeField] private int spawnCount = 10;

    [SerializeField] private float minDistance = 0.01f; // Å© ç≈í·ä‘äu

    private List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        int attempts = 0;

        while (spawnedPositions.Count < spawnCount && attempts < 1000)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(minPosition.x, maxPosition.x),
                Random.Range(minPosition.y, maxPosition.y),
                Random.Range(minPosition.z, maxPosition.z)
            );

            if (IsFarEnough(randomPosition))
            {
                Instantiate(prefab, randomPosition, Quaternion.identity);
                spawnedPositions.Add(randomPosition);
            }

            attempts++;
        }
    }

    bool IsFarEnough(Vector3 position)
    {
        foreach (Vector3 pos in spawnedPositions)
        {
            if (Vector3.Distance(pos, position) < minDistance)
            {
                return false;
            }
        }
        return true;
    }
}
