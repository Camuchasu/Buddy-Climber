using UnityEngine;

public class RockSpowner : MonoBehaviour
{
    public GameObject prefab;
    public float zPosition = 0f; 
    public float yOffset = 5f;

    void Start()
    {
        InvokeRepeating("Spawn", 1f, 1f);
    }

 void Spawn()
{
    float spawnY = Camera.main.transform.position.y + yOffset;

    // カメラからその高さまでの距離
    float distance = spawnY - Camera.main.transform.position.y;

    // 画面の左下と右上をワールド座標に変換
    Vector3 left = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, distance));
    Vector3 right = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, distance));

    float randomX = Random.Range(left.x, right.x);

    Vector3 spawnPos = new Vector3(randomX, spawnY, zPosition);

    Instantiate(prefab, spawnPos, Quaternion.identity);
}
}