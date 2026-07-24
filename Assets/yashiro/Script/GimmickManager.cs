using UnityEngine;

public class GimmickManager : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("風")]
    [SerializeField] private float windStart = 40f;
    [SerializeField] private float windEnd = 60f;
    [SerializeField] private WindSystem wind;

    [Header("岩")]
    [SerializeField] private float rockStart = 60f;
    [SerializeField] private float rockEnd = 80f;

    [Header("ギミック")]
    [SerializeField] private GameObject windSystem;
    [SerializeField] private GameObject windEffect;
    [SerializeField] private GameObject rockSpawner;

    void Update()
    {
        float y = player.position.y;

        // 風
 if (y >= windStart && y < windEnd)
{
    wind.CanBlow = true;
}
else
{
    wind.CanBlow = false;
}

        // 岩
        if (y >= rockStart && y < rockEnd)
        {
            rockSpawner.SetActive(true);
        }
        else
        {
            rockSpawner.SetActive(false);
        }
    }
}
