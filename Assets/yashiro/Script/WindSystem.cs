using System.Collections;
using UnityEngine;

public class WindSystem : MonoBehaviour
{
    [Header("風エフェクト")]
    [SerializeField] private ParticleSystem windEffect;

    [Header("風の時間")]
    [SerializeField] private float minInterval = 3f;
    [SerializeField] private float maxInterval = 8f;
    [SerializeField] private float windDuration = 2f;

    [Header("風向き")]
    [SerializeField]
    public Vector3 windDirection =
        Vector3.left;

    [Header("風の強さ")]
    [SerializeField]
    public float windPower = 5f;

    [Header("風範囲")]
    [SerializeField]
    private Vector3 boxSize =
        new Vector3(30f, 30f, 30f);

    [Header("追従対象")]
    [SerializeField]
    private Transform player;

    private bool isBlowing = false;

    public bool IsBlowing => isBlowing;

    public bool CanBlow = false;

    void Start()
    {
        StartCoroutine(WindRoutine());
    }

    void Update()
    {
        // プレイヤーに追従
        if (player != null)
        {
            transform.position = player.position;
        }

        // 風エフェクト
        if (isBlowing)
        {
            if (!windEffect.isPlaying)
            {
                windEffect.Play();
            }
        }
        else
        {
            if (windEffect.isPlaying)
            {
                windEffect.Stop();
            }
        }

        
    }

    IEnumerator WindRoutine()
    {
    while (true)
    {
        if (!CanBlow)
        {
            isBlowing = false;
            yield return null;
            continue;
        }

        yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

        isBlowing = true;

        yield return new WaitForSeconds(windDuration);

        isBlowing = false;
    }
}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireCube(
            transform.position,
            boxSize
        );
    }
}