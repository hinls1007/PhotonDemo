using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using MultiPlayer;

public class PhotonClientImpl: MonoBehaviourPunCallbacks, MultiPlayClient
{
    private MultiPlayClientCallback clientCallback;

    private const string KEY_action = "KEY_action";
    private const string KEY_playerMoveData = "KEY_playerMoveData";
    private const string KEY_objectMoveData = "KEY_objectMoveData";

    private const string ACTION_playerMove = "ACTION_playerMove";
    private const string ACTION_objectMove = "ACTION_objectMove";

    public void init(MultiPlayClientCallback clientCallback)
    {
        this.clientCallback = clientCallback;

        PhotonPeer.RegisterType(typeof(PlayerMove), (byte)'A', PhotonSerialize.PlayerSerializeMethod, PhotonSerialize.PlayerDeserializeMethod);
        PhotonPeer.RegisterType(typeof(ObjectMove), (byte)'B', ObjectMove.SerializeMethod, ObjectMove.DeserializeMethod);
        var registered = PhotonPeer.RegisterType(typeof(RoomItem), (byte)'C', PhotonSerialize.RoomItemSerializeMethod, PhotonSerialize.RoomItemDeserializeMethod);
        Debug.Log("registered: " + registered);
    }

    public string getUserID()
    {
        return PhotonNetwork.LocalPlayer.UserId;
    }

    public bool isHostPlayer()
    {
        return PhotonNetwork.MasterClient.UserId == getUserID();
    }

    public List<PlayerInfo> getPlayerList()
    {
        return PhotonNetwork.PlayerList.Select(player =>
        {
            var property = player.CustomProperties;
            var moveData = new PlayerMove();
            property.TryGetValue(KEY_playerMoveData, out moveData);

            return new PlayerInfo(
                userID: player.UserId,
                location: moveData.location,
                rotation: moveData.rotation,
                currentVelocity: moveData.velocity,
                currentAngularVelocity: moveData.angularVelocity
                );
        }
            
        ).ToList();
    }

    public void playerMove(PlayerInfo playerInfo)
    {
        if (playerInfo.currentVelocity == Vector3.zero && playerInfo.currentAngularVelocity == Vector3.zero)
        {
            return;
        }
        var customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        customProperties[KEY_action] = ACTION_playerMove;
        customProperties[KEY_playerMoveData] = new PlayerMove(
            userId: playerInfo.userID,
            location: playerInfo.location,
            rotation: playerInfo.rotation,
            velocity: playerInfo.currentVelocity,
            angularVelocity: playerInfo.currentAngularVelocity);

        Debug.Log("playerMove " + customProperties);
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    public void objectMove(string triggerByID, RoomItem item)
    {
        var properties = PhotonNetwork.CurrentRoom.CustomProperties;
        properties[item.itemID] = item;

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
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
        Debug.Log("createOrJoinRoom");
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 5;
        options.PublishUserId = true;
        options.CleanupCacheOnLeave = true;
        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
    }

    public void initRoomInfo(MultiPlayer.RoomInfo roomInfo)
    {
        var properties = PhotonNetwork.CurrentRoom.CustomProperties;
        foreach (var room in roomInfo.roomItemList)
        {
            properties[room.itemID] = room;
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
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

        var properties = PhotonNetwork.CurrentRoom.CustomProperties;

        List<RoomItem> itemList = new List<RoomItem>();
        foreach (var value in properties.Values)
        {
            var item = value as RoomItem;

            if (item != null)
            {
                itemList.Add(item);
            }
        }

        MultiPlayer.RoomInfo roomInfo = null;
        if (itemList.Count > 0)
        {
            roomInfo = new MultiPlayer.RoomInfo(roomItemList: itemList);
        }
        clientCallback.onRoomJoined(roomInfo: roomInfo);
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
        clientCallback.onPlayerJoinedRoom(player: new PlayerInfo(
            userID: newPlayer.UserId,
            location: new Vector3(),
            rotation: new Quaternion(),
            currentVelocity: new Vector3(),
            currentAngularVelocity: new Vector3()
            )
            );
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
                    Debug.Log("OnPlayerPropertiesUpdate : playerData" + playerData.velocity);
                    if (playerData.userId != null && playerData.userId != "")
                    {
                        clientCallback.onOtherPlayerMove(
                            playerInfo: new PlayerInfo(
                                userID: playerData.userId,
                                location: playerData.location,
                                rotation: playerData.rotation,
                                currentVelocity: playerData.velocity,
                                currentAngularVelocity: playerData.angularVelocity
                                )
                        );
                    }
                    break;
                //case ACTION_objectMove:
                //    ObjectMove objectData = new ObjectMove();

                //    changedProps.TryGetValue(KEY_objectMoveData, out objectData);
                //    if (objectData.targetObjectID != null && objectData.targetObjectID != "")
                //    {
                //        clientCallback.onOtherObjectMove(
                //            triggerByID: objectData.triggerByID,
                //            targetObjectID: objectData.targetObjectID,
                //            velocity: objectData.velocity,
                //            angularVelocity: objectData.angularVelocity
                //        );
                //    }
                //    break;
                default:
                    break;
            }

        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        foreach(var value in propertiesThatChanged.Values)
        {
            var item = value as RoomItem;

            if (item != null)
            {
                clientCallback.onOtherObjectMove("", item);
            }
        }
    }

    public class PlayerMove
    {
        public string userId;
        public Vector3 location;
        public Quaternion rotation;
        public Vector3 velocity;
        public Vector3 angularVelocity;

        public PlayerMove()
        {
            this.userId = "";
            this.location = new Vector3();
            this.velocity = new Vector3();
            this.angularVelocity = new Vector3();
        }

        public PlayerMove(string userId, Vector3 location, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
        {
            this.userId = userId;
            this.location = location;
            this.rotation = rotation;
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

        public static byte[] SerializeMethod(object customObject)
        {
            var json = JsonUtility.ToJson((ObjectMove)customObject);
            var jsonByte = Encoding.UTF8.GetBytes(json);
            return jsonByte;
        }
        public static object DeserializeMethod(byte[] serializedCustomObject)
        {
            var json = Encoding.ASCII.GetString(serializedCustomObject);
            
            ObjectMove obj = JsonUtility.FromJson<ObjectMove>(json);
            return obj;
        }
    }

    class PhotonSerialize
    {
        
        public static byte[] RoomItemSerializeMethod(object customObject)
        {
            var json = JsonUtility.ToJson((RoomItem)customObject);
            var jsonByte = Encoding.ASCII.GetBytes(json);
            return jsonByte;
        }
        public static object RoomItemDeserializeMethod(byte[] serializedCustomObject)
        {
            var json = Encoding.ASCII.GetString(serializedCustomObject);
            RoomItem obj = JsonUtility.FromJson<RoomItem>(json);
            return obj;
        }


        static readonly object PlayerSerializeLocker = new object();
        static readonly object PlayerDeserializeLocker = new object();
        public static byte[] PlayerSerializeMethod(object customObject)
        {
            lock(PlayerSerializeLocker)
            {
                var json = JsonUtility.ToJson((PlayerMove)customObject);
                var jsonByte = Encoding.ASCII.GetBytes(json);
                return jsonByte;
            }
        }
        public static object PlayerDeserializeMethod(byte[] serializedCustomObject)
        {
            lock(PlayerDeserializeLocker)
            {
                var json = Encoding.UTF8.GetString(serializedCustomObject);
                Debug.Log("Deser json :" + json);
                PlayerMove obj = JsonUtility.FromJson<PlayerMove>(json);
                Debug.Log("Deser obj id: " + obj.userId + ", v:" + obj.velocity + ", angularVelocity:" + obj.angularVelocity);
                return obj;
            }
        }
    }
}
