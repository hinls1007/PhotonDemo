using UnityEngine;
using System.Collections;

public class BallTest : MonoBehaviour
{
    public bool isSimulate = false;
    public Rigidbody referenceBody;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isSimulate)
        {
            Debug.Log("Ball, velocity" + rb.velocity);
            rb.velocity = referenceBody.velocity;
            rb.angularVelocity = referenceBody.angularVelocity;
        } else
        {
            Debug.Log("Ball, velocity" + rb.velocity);
            Debug.Log("Ball angularVelocity" + rb.angularVelocity);
        }
    }
}
