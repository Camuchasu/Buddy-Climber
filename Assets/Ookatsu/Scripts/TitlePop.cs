using UnityEngine;
using System.Collections;

public class TitlePop : MonoBehaviour
{
    public Transform logo;
    public float duration = 0.5f;

    void Start()
    {
        StartCoroutine(PopAnimation());
    }

    IEnumerator PopAnimation()
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // イージング（ドーン感）
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f) * 4.0f;

            logo.localScale = Vector3.one * scale;
            yield return null;
        }

    }
}