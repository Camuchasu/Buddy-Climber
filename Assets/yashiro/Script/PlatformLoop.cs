using UnityEngine;

public class PlatformLoop : MonoBehaviour
{
    [Header("ステージ範囲")]
    [SerializeField] private float leftLimit = -20f;

    [SerializeField] private float rightLimit = 20f;

    void Update()
    {
        Vector3 pos = transform.position;

        // 左に出たら右へ
        if (pos.x < leftLimit)
        {
            pos.x = rightLimit;
        }

        // 右に出たら左へ
        if (pos.x > rightLimit)
        {
            pos.x = leftLimit;
        }

        transform.position = pos;
    }
}