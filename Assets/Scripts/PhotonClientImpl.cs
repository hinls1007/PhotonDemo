using UnityEngine;
using System.Linq;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon.StructWrapping;
using MultiPlayer;
public class PhotonClientImpl: MonoBehaviourPunCallbacks, MultiPlayClient
{
    private MultiPlayClientCallback clientCallback;

    private const string KEY_action = "KEY_action";
    private const string KEY_playerMoveData = "KEY_playerMoveData";
    private const string KEY_objectMoveData = "KEY_objectMoveData";
    private const string KEY_playerLocation = "KEY_playerLocation";

    private const string ACTION_playerMove = "ACTION_playerMove";
    private const string ACTION_objectMove = "ACTION_objectMove";

    public void init(MultiPlayClientCallback clientCallback)
    {
        this.clientCallback = clientCallback;
    }

    public string getUserID()
    {
        return PhotonNetwork.LocalPlayer.UserId;
    }

    public List<PlayerInfo> getPlayerList()
    {
        return PhotonNetwork.PlayerList.Select(player =>
        {
            var property = player.CustomProperties;
            var location = new Vector3();
            property.TryGetValue(KEY_playerLocation, out location);

            return new PlayerInfo(userID: player.UserId, location: location);
        }
            
        ).ToList();
    }

    public void playerMove(string userID, Vector3 location = default, Vector3 velocity = default, Vector3 angularVelocity = default)
    {
        var customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        customProperties.TryAdd(KEY_action, ACTION_playerMove);
        customProperties.TryAdd(KEY_playerMoveData, new PlayerMove(userID, velocity, angularVelocity));
        customProperties.TryAdd(KEY_playerLocation, location);

        Debug.Log("playerMove " + customProperties);
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    public void objectMove(string triggerByID, string targetObjectID, Vector3 velocity = default, Vector3 angularVelocity = default)
    {
        var customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        customProperties.TryAdd(KEY_action, ACTION_playerMove);
        customProperties.TryAdd(KEY_playerMoveData, new ObjectMove(triggerByID, targetObjectID, velocity, angularVelocity));

        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    public void connectServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void createRoom(string roomName, string password)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 5;
        options.PublishUserId = true;
        options.CleanupCacheOnLeave = true;
        PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
    }

    public void joinRoom(string roomName, string password)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 5;
        options.PublishUserId = true;
        options.CleanupCacheOnLeave = true;
        PhotonNetwork.JoinRoom(roomName);
    }

    public void createOrJoinRoom(string roomName, string password)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 5;
        options.PublishUserId = true;
        options.CleanupCacheOnLeave = true;
        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        clientCallback.onConnectedServer();
        Debug.Log("onConnectedServer");
    }

    public override void OnConnected()
    {
        base.OnConnected();
        Debug.Log("OnConnected");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        clientCallback.onRoomCreated();
        Debug.Log("OnCreatedRoom");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        clientCallback.onRoomJoined();
        Debug.Log("OnJoinedRoom");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("OnCreateRoomFailed : " + message);
        clientCallback.onRoomCreatError();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("OnJoinRoomFailed : " + message);
        clientCallback.onRoomJoinFail();
    }

    public override void OnErrorInfo(ErrorInfo errorInfo)
    {
        base.OnErrorInfo(errorInfo);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        clientCallback.onPlayerJoinedRoom(player: new PlayerInfo(userID: newPlayer.UserId, location: new Vector3()));
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        clientCallback.onPlayerLeftRoom(otherPlayer.UserId);
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        Debug.Log("OnPlayerPropertiesUpdate : UserId" + targetPlayer.UserId);

        if (targetPlayer.UserId != getUserID() && changedProps.ContainsKey(KEY_action))
        {
            var action = changedProps[KEY_action];
            Debug.Log("OnPlayerPropertiesUpdate : actiactionon" + action);

            switch (action)
            {
                case ACTION_playerMove:
                    PlayerMove playerData = new PlayerMove();
                    changedProps.TryGetValue(KEY_playerMoveData, out playerData);
                    Debug.Log("OnPlayerPropertiesUpdate : playerData" + playerData);
                    if (playerData.userId != null && playerData.userId != "")
                    {
                        clientCallback.onOtherPlayerMove(
                            userID: playerData.userId,
                            velocity: playerData.velocity,
                            angularVelocity: playerData.angularVelocity
                        );
                    }
                    break;
                case ACTION_objectMove:
                    ObjectMove objectData = new ObjectMove();

                    changedProps.TryGetValue(KEY_objectMoveData, out objectData);
                    if (objectData.targetObjectID != null && objectData.targetObjectID != "")
                    {
                        clientCallback.onOtherObjectMove(
                            triggerByID: objectData.triggerByID,
                            targetObjectID: objectData.targetObjectID,
                            velocity: objectData.velocity,
                            angularVelocity: objectData.angularVelocity
                        );
                    }
                    break;
                default:
                    break;
            }

        }
    }

    struct PlayerMove
    {
        public string userId;
        public Vector3 velocity;
        public Vector3 angularVelocity;

        public PlayerMove(string userId, Vector3 velocity, Vector3 angularVelocity)
        {
            this.userId = userId;
            this.velocity = velocity;
            this.angularVelocity = angularVelocity;
        }
    }

    struct ObjectMove
    {
        public string triggerByID;
        public string targetObjectID;
        public Vector3 velocity;
        public Vector3 angularVelocity;

        public ObjectMove(string triggerByID, string targetObjectID, Vector3 velocity, Vector3 angularVelocity)
        {
            this.triggerByID = triggerByID;
            this.targetObjectID = targetObjectID;
            this.velocity = velocity;
            this.angularVelocity = angularVelocity;
        }
    }
}
