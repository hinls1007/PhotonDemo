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
            MultiPlayManager.Instance.playerMove(playerInfo: new PlayerInfo(
                userID: playerID,
                location: transform.position,
                rotation: rigidbody.rotation,
                currentVelocity: rigidbody.velocity,
                currentAngularVelocity: rigidbody.angularVelocity
                )
            );
        }
    }

    public void onOtherPlayerMove(PlayerInfo playerInfo) {
        if (!MultiPlayManager.Instance.isLocalPlayer(userID: playerInfo.userID)
            && playerInfo.userID == playerID)
        {
            transform.position = playerInfo.location;
            rigidbody.rotation = playerInfo.rotation;
            rigidbody.velocity = playerInfo.currentVelocity;
            rigidbody.angularVelocity = playerInfo.currentAngularVelocity;
        }
    }
}
