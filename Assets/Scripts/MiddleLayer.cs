using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MiddleLayer : MonoBehaviour
{
    private delegate void SuccessCallback(string name);
    private delegate void FailCallback(int errorCode, string errorMessage);
    public delegate void DisconnectedCallback(string message);
    public delegate void PlayerCallback(string player);

    private GameObject localPlayer;

}
