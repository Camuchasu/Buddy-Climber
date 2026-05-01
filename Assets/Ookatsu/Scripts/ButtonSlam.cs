using UnityEngine;
using System.Collections;

public class ButtonSlam : MonoBehaviour
{
    public RectTransform target;
    public Vector2 endPos;

    void Start()
    {
        StartCoroutine(StickAnimation());
    }

    IEnumerator StickAnimation()
    {
        Vector2 startPos = target.anchoredPosition;

        float time = 0;
        float duration = 1.0f;

        // ① 高速で移動
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float ease = 1 - Mathf.Pow(1 - t, 4);
            target.anchoredPosition = Vector2.Lerp(startPos, endPos, ease);

            // ② 潰れる（超重要）
            float squash = 1 + (1 - t) * 0.4f;
            target.localScale = new Vector3(squash, 1 / squash, 1);

            yield return null;
        }
    }
}