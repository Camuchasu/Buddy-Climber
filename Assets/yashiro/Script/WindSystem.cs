using System.Collections;
using UnityEngine;

public class WindSystem : MonoBehaviour
{
    public ParticleSystem windEffect;

    public float minInterval = 3f;
    public float maxInterval = 8f;
    public float windDuration = 2f;

    [Header("風")]
    public Vector3 windDirection = new Vector3(1, 0, 0);
    public float windPower = 10f;

    public bool IsBlowing => isBlowing; // 外から読めるように

    private bool isBlowing = false;

    void Start()
    {
        StartCoroutine(WindRoutine());
    }

    void Update()
    {
        if (isBlowing)
        {
            if (!windEffect.isPlaying) windEffect.Play();
        }
        else
        {
            if (windEffect.isPlaying) windEffect.Stop();
        }
    }

    IEnumerator WindRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            isBlowing = true;
            yield return new WaitForSeconds(windDuration);
            isBlowing = false;
        }
    }
}