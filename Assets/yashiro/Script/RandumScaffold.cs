using System.Collections.Generic;
using UnityEngine;

public class RandumScaffold : MonoBehaviour
{
    [SerializeField] private GameObject normalFloorPrefab;

    [SerializeField] private Transform player;

    [SerializeField] private Transform wall;

    [SerializeField] private float xRange = 5f;

    [SerializeField] private float zPos = 0f;

    [SerializeField] private int startCount = 30;

    [SerializeField] private float minDistance = 3f;

    [SerializeField] private float spawnOffsetY = 20f;

    [SerializeField] private float goalY = 200f;


    private List<Vector3> positions = new List<Vector3>();

    private float highestY;

    void Start()
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
        highestY = player.position.y;

        for (int i = 0; i < startCount; i++)
        {
            SpawnFloor(highestY);

            highestY += 2f;
        }
        }
    }

    void Update()
    {
        
        while (highestY < player.position.y + spawnOffsetY
               && highestY < goalY)
        {
            SpawnFloor(highestY);

            highestY += 2f;
        }
        
    }

    void SpawnFloor(float y)
    {
        for (int i = 0; i < 100; i++)
        {
      Vector3 randomPos = new Vector3(
    Random.Range(
        wall.position.x - xRange,
        wall.position.x + xRange
    ),
    y,
    wall.position.z
);
            bool tooClose = false;

            foreach (Vector3 pos in positions)
            {
                if (Vector3.Distance(pos, randomPos) < minDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                GameObject floor =
                    Instantiate(normalFloorPrefab,
                                randomPos,
                                Quaternion.identity,
                                transform);

                positions.Add(randomPos);

                return;
            }
        }
    }
}