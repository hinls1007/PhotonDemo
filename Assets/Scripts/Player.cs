using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MultiPlayer;

public class Player : MonoBehaviour, MultiPlayCallback
{
    public string playerID;
    public Rigidbody rigidbody;

    private void Start()
    {
        MultiPlayManager.Instance.registerCallback(this);
    }

    private void FixedUpdate()
    {
        if (MultiPlayManager.Instance.isLocalPlayer(userID: playerID))
        {
            MultiPlayManager.Instance.playerMove(
                userID: playerID,
                location: transform.position,
                velocity: rigidbody.velocity,
                angularVelocity: rigidbody.angularVelocity
            );
        }
    }

    public void onOtherPlayerMove(string userID, Vector3 velocity, Vector3 angularVelocity) {
        if (!MultiPlayManager.Instance.isLocalPlayer(userID: userID)
            && userID == playerID)
        {
            rigidbody.velocity = velocity;
            rigidbody.angularVelocity = angularVelocity;
        }
    }
}
