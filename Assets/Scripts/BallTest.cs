using UnityEngine;
using System.Collections;

using MultiPlayer;

public class BallTest : MonoBehaviour, MultiPlayCallback
{
    public string itemID;

    public Rigidbody rb;
    public Collider collider;

    private RoomItem currentItem;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        MultiPlayManager.Instance.registerCallback(this);
        if (!MultiPlayManager.Instance.isHostPlayer())
        {
            Destroy(rb);
        }
    }

    private void FixedUpdate()
    {
        Debug.Log("Item isHostPlayer " + MultiPlayManager.Instance.isHostPlayer());
        if (MultiPlayManager.Instance.isHostPlayer())
        {
            var item = new RoomItem(
                itemID: itemID,
                itemTypeID: "ball",
                location: transform.position,
                rotation: transform.rotation,
                currentVelocity: rb.velocity,
                currentAngularVelocity: rb.angularVelocity
                );
            MultiPlayManager.Instance.objectMove(triggerByID: "", item: item);
        } else
        {
            if (currentItem != null)
            {
                var lerp = Time.deltaTime * 5;
                Debug.Log("Lerp : " + lerp);
                transform.position = Vector3.Lerp(transform.position, currentItem.location, lerp);
                transform.rotation = Quaternion.Lerp(transform.rotation, currentItem.rotation, lerp);
            }
        }
    }

    public void setCurrentState(RoomItem item)
    {
        Debug.Log("Item currentVelocity " + item.currentVelocity);
        transform.position = item.location;
        transform.rotation = item.rotation;
    }

    public void onOtherObjectMove(
        string triggerByID, RoomItem item)
    {
        Debug.Log("Item itemID " + item.itemID);
        if (!MultiPlayManager.Instance.isHostPlayer())
        {
            if (item.itemID == itemID)
            {
                currentItem = item;
            }
        }
    }
}
