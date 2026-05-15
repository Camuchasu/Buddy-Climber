using UnityEngine;

public class ChainVisual : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        line.SetPosition(0, player1.position);
        line.SetPosition(1, player2.position);
    }
}