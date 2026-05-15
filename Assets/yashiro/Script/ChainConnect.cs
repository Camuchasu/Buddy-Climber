using UnityEngine;

public class ChainConnect : MonoBehaviour
{
    [SerializeField] private Rigidbody connectedPlayer;

    private SpringJoint joint;

    void Start()
    {
        joint = gameObject.AddComponent<SpringJoint>();

        joint.connectedBody = connectedPlayer;

        joint.autoConfigureConnectedAnchor = false;

        joint.maxDistance = 3f;
        joint.minDistance = 1f;

        joint.spring = 50f;
        joint.damper = 5f;
    }
}