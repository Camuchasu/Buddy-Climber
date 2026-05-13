using UnityEngine;

public class IceFloor : MonoBehaviour
{
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMove player =
                collision.gameObject.GetComponent<PlayerMove>();

            if (player != null)
            {
                player.SetIceState(true);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMove player =
                collision.gameObject.GetComponent<PlayerMove>();

            if (player != null)
            {
                player.SetIceState(false);
            }
        }
    }
}