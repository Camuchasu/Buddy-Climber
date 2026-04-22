using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] private Transform m_rockPos;
    [SerializeField] private Transform m_playerPos;

    private float m_dist;

    void Update()
    {
        m_dist = Vector3.Distance(m_rockPos.position, m_playerPos.position);

        if (m_dist >= 5.0f)
        {
            Destroy(gameObject);
        }
    }
}
