using UnityEngine;

public class CloudLoop : MonoBehaviour
{
    public float resetX = 12f;
    public float endX = -12f;

    void Update()
    {
        transform.position += Vector3.left * 0.3f * Time.deltaTime;

        if (transform.position.x < endX)
        {
            Vector3 pos = transform.position;
            pos.x = resetX;
            transform.position = pos;
        }
    }
}