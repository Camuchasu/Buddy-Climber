using UnityEngine;

public class GimmickManager : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("発動高さ")]
    [SerializeField] private float fallingFloorHeight = 20f;
    [SerializeField] private float windHeight = 40f;
    [SerializeField] private float rockHeight = 60f;

    [Header("ギミック")]
    [SerializeField] private GameObject fallingFloorArea;
    [SerializeField] private GameObject windSystem;
    [SerializeField] private GameObject rockSpawner;

    private bool fallingActivated;
    private bool windActivated;
    private bool rockActivated;

    void Update()
    {
        float y = player.position.y;

        if (!fallingActivated && y >= fallingFloorHeight)
        {
            fallingFloorArea.SetActive(true);

            fallingActivated = true;
        }

        if (!windActivated && y >= windHeight)
        {
            windSystem.SetActive(true);

            windActivated = true;
        }

        if (!rockActivated && y >= rockHeight)
        {
            rockSpawner.SetActive(true);

            rockActivated = true;
        }
    }
}
