using UnityEngine;
using System.Collections;

using MultiPlayer;

public class BallTest : MonoBehaviour, MultiPlayCallback
{
    public string itemID;

    public Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (MultiPlayManager.Instance.isHostPlayer())
        {
            var item = new RoomItem(
                itemID: itemID,
                itemTypeID: "ball",
                location: rb.position,
                rotation: rb.rotation,
                currentVelocity: rb.velocity,
                currentAngularVelocity: rb.angularVelocity
                );
            MultiPlayManager.Instance.objectMove(triggerByID: "", item: item);
        }
    }

    public void setCurrentState(Vector3 velocity, Vector3 angularVelocity)
    {
        rb.velocity = velocity;
        rb.angularVelocity = angularVelocity;
    }

    public void onOtherObjectMove(
        string triggerByID,
        RoomItem item)
    {
        if (item.itemID == itemID)
        {
            setCurrentState(velocity: item.currentVelocity, angularVelocity: item.currentAngularVelocity);
        }
    }
}
