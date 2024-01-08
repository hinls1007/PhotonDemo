using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryData : MonoBehaviour
{
    //[SerializeField] public string objectName;
    //[SerializeField] public PlayerProperties[] playerDict;

    public Dictionary<int, GameObject>[] playerData;


}

[System.Serializable]
public class PlayerProperties
{
    public int playerID;
    public GameObject playerActor;
    public Vector3 location;
    public Quaternion rotation;

}


