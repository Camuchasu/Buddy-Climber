using System.Collections;
using UnityEngine;

public class WindSystem : MonoBehaviour
{
    
    public ParticleSystem windEffect;

    public float minInterval = 3f;
    public float maxInterval = 8f;
    public float windDuration = 2f;

    private bool isBlowing = false;

    void Start()
    {
        StartCoroutine(WindRoutine());
    }

   void Update()
    {
        if (isBlowing)
        {
            if (!windEffect.isPlaying)
                windEffect.Play();
        }
        else
        {
            if (windEffect.isPlaying)
                windEffect.Stop();
        }
    }

    IEnumerator WindRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            isBlowing = true;
            yield return new WaitForSeconds(windDuration);
            isBlowing = false;
        }
    }
}