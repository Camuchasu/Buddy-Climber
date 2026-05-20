using UnityEngine;

public class RandumScaffold : MonoBehaviour
{
    [Header("通常床")]
    [SerializeField] private GameObject normalFloorPrefab;

    [Header("崩れる床")]
    [SerializeField] private GameObject collapsingFloorPrefab;

    [Header("氷床")]
    [SerializeField] private GameObject iceFloorPrefab;

    [Header("雲床")]
    [SerializeField] private GameObject cloudFloorPrefab;

    [Header("参照")]
    [SerializeField] private Transform player;

    [SerializeField] private Transform wall;

    [Header("生成設定")]
    [SerializeField] private float xRange = 5f;

    [SerializeField] private int startCount = 30;

    [SerializeField] private float spawnOffsetY = 20f;

    [SerializeField] private float goalY = 200f;

    [Header("床間隔")]
    [SerializeField] private float minDistance = 3f;

    [Header("高さ範囲")]
    [SerializeField] private float collapsingHeight = 50f;

    [SerializeField] private float collapsingEndHeight = 100f;

    [SerializeField] private float iceHeight = 100f;

    [SerializeField] private float iceEndHeight = 150f;

    [SerializeField] private float cloudHeight = 150f;

    [SerializeField] private float cloudEndHeight = 200f;

    private float highestY;

    void Start()
    {
        if (!player)
        {
            GameObject dummy =
                GameObject.Find("Player");

            if (dummy)
            {
                player = dummy.transform;
            }
        }

        highestY = player.position.y;

        for (int i = 0; i < startCount; i++)
        {
            SpawnFloor(highestY);

            // 初期生成の高さもランダム
            highestY +=
                Random.Range(3f, 7f);
        }
    }

    void Update()
    {
        while (
            highestY <
            player.position.y +
            spawnOffsetY
            &&
            highestY < goalY
        )
        {
            SpawnFloor(highestY);

            // 毎回ランダムな高さ差
            highestY +=
                Random.Range(2f, 6f);
        }
    }

    void SpawnFloor(float y)
    {
        // 1段ごとの床数
        int floorCount =
            Random.Range(1, 8);

        Vector3[] spawnedPositions =
            new Vector3[floorCount];

        int spawned = 0;

        int maxTry = 100;

        for (
            int i = 0;
            i < maxTry
            &&
            spawned < floorCount;
            i++
        )
        {
            float randX =
                Random.Range(
                    -xRange,
                    xRange
                );

            // Y方向もランダム
            float randY =
                Random.Range(
                    -1.5f,
                    1.5f
                );

            Vector3 pos =
                new Vector3(
                    wall.position.x + randX,
                    y + randY,
                    wall.position.z
                );

            bool tooClose = false;

            // 同じ段の床だけ距離判定
            for (int j = 0; j < spawned; j++)
            {
                if (
                    Vector3.Distance(
                        spawnedPositions[j],
                        pos
                    ) < minDistance
                )
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose)
            {
                continue;
            }

            GameObject prefabToSpawn;

            // 高さで床変更
            if (
                y >= collapsingHeight
                &&
                y < collapsingEndHeight
            )
            {
                prefabToSpawn =
                    collapsingFloorPrefab;
            }
            else if (
                y >= iceHeight
                &&
                y < iceEndHeight
            )
            {
                prefabToSpawn =
                    iceFloorPrefab;
            }
            else if (
                y >= cloudHeight
                &&
                y < cloudEndHeight
            )
            {
                prefabToSpawn =
                    cloudFloorPrefab;
            }
            else
            {
                prefabToSpawn =
                    normalFloorPrefab;
            }

            Instantiate(
                prefabToSpawn,
                pos,
                Quaternion.identity,
                transform
            );

            spawnedPositions[spawned] = pos;

            spawned++;
        }
    }
}