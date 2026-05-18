using UnityEngine;

public class WindFollow : MonoBehaviour
{
    [SerializeField] private Transform player;

    void Update()
    {
        transform.position = player.position;
    }
}