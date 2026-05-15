using UnityEngine;

public class TemporaryPlatform : MonoBehaviour
{
    [Header("Á‚¦‚é‹——£")]
    public float destroyDistance = 5f;

    // ƒvƒŒƒCƒ„[
    private Transform player;

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
        {
            player = obj.transform;
        }
    }

    void Update()
    {
        if (player == null)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        // —£‚ê‚½‚çÁ‚¦‚é
        if (distance > destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}