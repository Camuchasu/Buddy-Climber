using UnityEngine;

public class Player : MonoBehaviour
{

    public Rigidbody rb;    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = (transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * 500) +
                            (transform.right * Input.GetAxis("Horizontal") * Time.deltaTime * 500) + 
                            (transform.up * rb.velocity.y);
        //transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * 5);
    }

}
