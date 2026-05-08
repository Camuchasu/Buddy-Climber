using UnityEngine;

public class StageFollow : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] private float borderY = 3f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void LateUpdate()
    {
        float offset = player.position.y - borderY;

        if (offset < 0)
        {
            offset = 0;
        }

        transform.position =
            startPos + Vector3.down * offset;
    }
}