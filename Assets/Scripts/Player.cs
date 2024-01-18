using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MultiPlayer;

public class Player : MonoBehaviour, MultiPlayCallback
{
    public string playerID;
    public Rigidbody rigidbody;

    private PlayerInfo previousInfo;
    private PlayerInfo currentInfo;
    private float elapsedFrames = 0;

    private void Start()
    {
        
        MultiPlayManager.Instance.registerCallback(this);
        if (!MultiPlayManager.Instance.isHostPlayer())
        {
            Destroy(rigidbody);
        }
    }

    private void Update()
    {


        
    }

    private void FixedUpdate()
    {
        if (MultiPlayManager.Instance.isLocalPlayer(userID: playerID))
        {
            float x = Input.GetAxis("Horizontal") * 2f;
            float z = Input.GetAxis("Vertical") * 2f;
            var newLocation = new Vector3(x, 0, z);
            if (MultiPlayManager.Instance.isHostPlayer())
            {
                rigidbody.AddForce(newLocation);
            } else
            {
                MultiPlayManager.Instance.playerMove(playerInfo: new PlayerInfo(
                    userID: playerID,
                    force: newLocation
                    )
                );
            }
        } else
        if (MultiPlayManager.Instance.isHostPlayer())
        {
            MultiPlayManager.Instance.playerMove(playerInfo: new PlayerInfo(
                userID: playerID,
                location: transform.position,
                rotation: transform.rotation,
                force: Vector3.zero,
                currentVelocity: rigidbody.velocity,
                currentAngularVelocity: rigidbody.angularVelocity
                )
            );
        }
        if (!MultiPlayManager.Instance.isHostPlayer())
        {
            if (previousInfo != null && currentInfo != null)
            {
                var lerp = Time.deltaTime * 5;
                Debug.Log("Lerp : " + lerp);

                transform.position = Vector3.Lerp(transform.position, currentInfo.location, lerp);
                transform.rotation = Quaternion.Lerp(transform.rotation, currentInfo.rotation, lerp);
            }
        }
    }

    public void Init(PlayerInfo playerInfo)
    {
        playerID = playerInfo.userID;

        transform.position = playerInfo.location;
        transform.rotation = playerInfo.rotation;
        currentInfo = playerInfo;
    }

    public void onOtherPlayerMove(PlayerInfo playerInfo) {
        if (playerInfo.userID == playerID)
        {
            if (!MultiPlayManager.Instance.isHostPlayer())
            {
                if (playerInfo.force == Vector3.zero)
                {
                    previousInfo = currentInfo;
                    currentInfo = playerInfo;
                    elapsedFrames = 0;
                }
            }
            else
            {
                rigidbody.AddForce(playerInfo.force);
            }
        }
            
    }
}
