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
            GameObject dummy = GameObject.Find("Player");
            if (dummy) player = dummy.transform;
        }

        highestY = player.position.y;

        for (int i = 0; i < startCount; i++)
        {
            SpawnFloor(highestY);
            highestY += 2f;
        }
    }

    void Update()
    {
        while (highestY < player.position.y + spawnOffsetY
               && highestY < goalY)
        {
            SpawnFloor(highestY);
            highestY += 3f;
        }
    }

    void SpawnFloor(float y)
    {
        int maxTry = 50;

        for (int i = 0; i < maxTry; i++)
        {
            float randX = Random.Range(-xRange, xRange);

            Vector3 pos = new Vector3(
                wall.position.x + randX,
                y,
                wall.position.z
            );

            GameObject prefabToSpawn;

            // 高さで種類決定
            if (y >= collapsingHeight && y < collapsingEndHeight)
            {
                prefabToSpawn = collapsingFloorPrefab;
            }
            else if (y >= iceHeight && y < iceEndHeight)
            {
                prefabToSpawn = iceFloorPrefab;
            }
            else if (y >= cloudHeight && y < cloudEndHeight)
            {
                prefabToSpawn = cloudFloorPrefab;
            }
            else
            {
                prefabToSpawn = normalFloorPrefab;
            }

            // 完全ランダムなので軽い重なりチェックだけ
            if (Random.value > 0.1f)
            {
                Instantiate(
                    prefabToSpawn,
                    pos,
                    Quaternion.identity,
                    transform
                );

                return;
            }
        }
    }
}