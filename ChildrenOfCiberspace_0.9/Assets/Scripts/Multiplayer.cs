using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using AsImpL;

public class Multiplayer : MonoBehaviour
{
    // define public game object used to visualize other players and their public text
    public GameObject otherPlayerObject;
    public GameObject otherTextPrefab;
    
    // The prefab of the player
    public GameObject crena;

    // The dictionary that keeps track of the artworks with the keys of artists
    public Dictionary<string, MessegeInfo> _messeges = new Dictionary<string, MessegeInfo>();

    // The action that tells the client that server has verified his key
    public event Action<bool> AccountVerified;

    // The color of the player
    public Color32 myColor;

    // URL array for selecting multiple rooms;
    public string[] Urls = new string [2];
    // The index that represents the rooms id;
    public int ind;
    // The url that is used for the websocket
    public string url;

    // The reference for the asset manager
    public AssetManager assetManager;
    // The websocket connection reference
    public WebSocket w;
    // The session id
    public System.Guid myGUID;

    /// <summary>
    /// In start, the color of the player its getting assigned
    /// </summary>
    private void Start()
    {
        myColor = ExtensionMethods.RandomColor();
        crena.GetComponent<Renderer>().material.SetColor("_EmissionColor", myColor);
    }
    /// <summary>
    /// Asssigning and Initializez the websocket and the session id 
    /// </summary>
    private void OnEnable()
    {
        url = Urls[ind];
        w = new WebSocket(new Uri(url));
        myGUID = System.Guid.NewGuid(); 
        //print(myGUID.ToString());
        StartCoroutine("Multyplayer");
    }
    /// <summary>
    /// Its connecting to the websocket and
    /// manages all the information that it is received from the server
    /// As well it send to server the possition of the player
    /// if there is an message error from the connection, the loop its being braked
    /// </summary>
    IEnumerator Multyplayer()
    {
        //if (Time.frameCount % 60 == 0)
        //{
        //    System.GC.Collect();
        //}
        // connect to server
        yield return StartCoroutine(w.Connect());
        Debug.Log("CONNECTED TO WEBSOCKETS");
        // generate random ID to have idea for each client (feels unsecure)
        // wait for messages
        while (true)
        {
            // read message
            string message = w.RecvString();
            // check if message is not empty
            if (message != null)
            {
                //Debug.Log(message.ToString());
                if (message.ToString() == "Conceted")
                {
                    w.SendString(myGUID + "\t" + ExtensionMethods.StringToCollor(myColor) + "\t" + "color");
                    //Debug.Log("color" + myGUID + "\t" + StringToCollor(myColor) + "\t" + "color");
                }
                else if (message.ToString().Contains("Deleted"))
                {
                    string otherGUID = message.ToString().Split('@')[1];
                    //Debug.Log(" Deleted id: " + otherGUID);
                    Destroy(NetworkPlayerManager.infoPl[otherGUID].otherPlayer);
                    NetworkPlayerManager.infoPl.Remove(otherGUID);
                }
                if (message.ToString().Contains("players"))
                {
                    Players playerInfo = JsonUtility.FromJson<Players>(message);
                    NetworkPlayerManager.UpdateLocalData(playerInfo, otherPlayerObject);
                }
                if (message.ToString().Contains("messageS"))
                {
                    TextMessages inMeseges = JsonUtility.FromJson<TextMessages>(message);
                    UpdateText(inMeseges);
                    //Debug.Log(" Mesege : " + message.ToString());
                }
                if (message.ToString().Contains("artWroks"))
                {
                    Debug.Log("Accept dowload: " + GeneralState.AceptAssets);

                    Artworks listArtworks = JsonUtility.FromJson<Artworks>(message);
                    assetManager.UpdateArtwork = listArtworks.artWroks;
                    Debug.Log("ArtworkOnTheServer: " + listArtworks.artWroks.Count);
                }
                if (message.ToString().Contains("DeleteArtWroks"))
                {
                    if (message.Split('@')[1]!=ArtistInfo.artistKey) 
                    assetManager.DeletArtWork(message.Split('@')[1]);
                }
                if (message.ToString().Contains("artKey"))
                {
                    string artKey = message.Split('@')[1];
                    artKey = artKey.Remove(artKey.Length - 1);
                    artKey=  artKey.Replace(" ", "");
                    bool temp = ExtensionMethods.CheckKey(artKey);
                    if (temp)
                    {
                        ArtistInfo.artistKey  = artKey;
                        Debug.Log(ArtistInfo.artistKey);
                        AccountVerified(true);
                    }
                }
                message = null;
                //Debug.Log("otherPlayers: " + otherPlayers.Count);
            }
            // if connection error, break the loop
            if (w.error != null)
            {
                Debug.Log("Error: " + w.error);
                break;
            }
            // Sending the position of the player to the server
            NetworkPlayerManager.SendPositions(ref w, FormatMessagePlayer);
            yield return 0;
        }
        // if error, close connection
        w.Close();
    }
    /// <summary>
    /// In onDisable the websocket its closed and the coroutine its stopped
    /// And at the same time all the other player and their messaages its getting deletted
    /// </summary>
    private void OnDisable()
    {
        w.Close();
        StopAllCoroutines();
        foreach (KeyValuePair<string, InfoPlayers> entry in NetworkPlayerManager.infoPl)
        {
            Destroy(entry.Value.otherPlayer);
        }
        foreach (KeyValuePair<string, MessegeInfo> entry in _messeges)
        {
            Destroy(entry.Value.textObject); 
        }
        NetworkPlayerManager.infoPl.Clear();
        _messeges.Clear();
        print("disconect3d");
    }
    /// <summary>
    /// It updates or creates new messages that the player wants to display publicly
    /// </summary>
    /// <param name="inMeseges"></param>
    void UpdateText(TextMessages inMeseges)
    {
        for (int i = 0; i < inMeseges.messageS.Count; i++)
        {
            if (!_messeges.ContainsKey(inMeseges.messageS[i].id))
            {
                if (myGUID.ToString() != inMeseges.messageS[i].id)
                {
                    string id = inMeseges.messageS[i].id;
                    _messeges.Add(id, inMeseges.messageS[i]);
                    _messeges[id].textObject = Instantiate(otherTextPrefab);
                    _messeges[id].textObject.transform.SetPositionAndRotation(_messeges[id].position, Quaternion.Euler(_messeges[id].rotation));
                    _messeges[id].textObject.GetComponent<TextMeshPro>().text = inMeseges.messageS[i].text;
                }
            }
            else
            {
                string id = inMeseges.messageS[i].id;
                _messeges[id].position = inMeseges.messageS[i].position;
                _messeges[id].rotation = inMeseges.messageS[i].rotation;

                _messeges[id].text = inMeseges.messageS[i].text;
                _messeges[id].textObject.transform.SetPositionAndRotation(_messeges[id].position, Quaternion.Euler(_messeges[id].rotation));
                _messeges[id].textObject.GetComponent<TextMeshPro>().text = _messeges[id].text;
            }

        }
    }
    /// <summary>
    /// For sending the player position 
    /// It takes the player position as a transforms and
    /// returns the string in the specific format in order to be send to the server
    /// </summary>
    /// <param name="_player"></param>
    /// <returns></returns>
    public string FormatMessagePlayer(Transform _player)
    {
        return myGUID + "\t" + _player.position.x + "\t" + _player.position.y + "\t" + _player.position.z
        +"\t" + 0 + "\t" + _player.rotation.eulerAngles.y + "\t" +0;
    }
    /// <summary>
    /// For sending the player public message
    /// It takes the player transform, the message and its type
    /// in order to returns the string in the specific format for the server to be understood
    /// </summary>
    /// <param name="_player"></param>
    /// <param name="msg"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public string FromatMessageText(Transform _player,string msg,string type)
    {
        return myGUID + "\t" + _player.position.x + "\t" + _player.position.y + "\t" + _player.position.z
        + "\t" + 0 + "\t" + _player.rotation.eulerAngles.y + "\t" + 0 + "\t" + msg + "\t" + type;
    }
    /// <summary>
    /// It sends to the server a request to get the ArtWorks informationn
    /// </summary>
    public void askForArtwork()
    {
        GeneralState.AceptAssets = true;
        Debug.Log("Send Request to server for new artwork");
        w.SendString("RequestArtwork");
    }

}


/// <summary>
/// A class that containts the list of all public text messages
/// </summary>
[Serializable]
public class TextMessages
{
    public List<MessegeInfo> messageS;
}

/// <summary>
/// A class that has all the information regarding the Public texts
/// </summary>
[Serializable]
public class MessegeInfo
{
    public Vector3 position;
    public Vector3 rotation;
    public string text;
    public string id;
    public GameObject textObject;
}
