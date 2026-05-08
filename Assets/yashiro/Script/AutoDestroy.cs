using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    private Transform player;

    [SerializeField] private float destroyDistance = 50f;

    void Start()
    {
        GameObject dummy = GameObject.Find("Player");

        if (dummy)
        {
            player = dummy.transform;
        }
    }

    void Update()
    {
        if (!player) return;

        if (transform.position.y
            < player.position.y - destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}
