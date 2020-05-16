using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using AsImpL;




public class Multiplayer : MonoBehaviour
{
    // define public game object used to visualize other players
    public GameObject otherPlayerObject;
    public GameObject otherTextPrefab;
    
    public GameObject myPlayer;
    public GameObject crena;

    private Vector3 prevPosition;
    //public Dictionary<string, GameObject> otherPlayers = new Dictionary<string, GameObject>();
    public Dictionary<string, InfoPlayers> infoPl = new Dictionary<string, InfoPlayers>();
    public Dictionary<string, MessegeInfo> _messeges = new Dictionary<string, MessegeInfo>();
    public event Action<bool> AccountVerified;


    public Color32 myColor;
    //WebSocket w = new WebSocket(new Uri("ws://www.in-dialog.com:3002/socket.io/?EIO=4&transport=websocket"));
    //"ws://localhost:8000"
    /// <summary>
    /// URL manager todo UI for selecting multiple rooms;
    /// </summary>
    public string[] Urls = new string [2];
    public int ind;
    public string url;

    public AssetManager assetManager;
    public WebSocket w;
    public System.Guid myGUID;

    private void Start()
    {
        myColor = ExtensionMethods.RandomColor();
        myColor.a = 225;
        crena.GetComponent<Renderer>().material.SetColor("_EmissionColor", myColor);
        myPlayer.GetComponentInChildren<SpriteRenderer>().color = myColor;
        //GeneralState.AceptAssets = true;
    }
    private void OnEnable()
    {
        url = Urls[ind];
        w = new WebSocket(new Uri(url));
        myGUID = System.Guid.NewGuid();
        //print(myGUID.ToString());
        StartCoroutine("Multyplayer");
    }


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
                    message = null;
                    continue;
                }
                else if (message.ToString().Contains("Deleted"))
                {
                    string otherGUID = message.ToString().Split('@')[1];
                    //Debug.Log(" Deleted id: " + otherGUID);
                    Destroy(infoPl[otherGUID].otherPlayer);
                    infoPl.Remove(otherGUID);
                    message = null;
                    continue;
                }
                if (message.ToString().Contains("players"))
                {
                    Players data = JsonUtility.FromJson<Players>(message);
                    UpdateLocalData(data);
                    message = null;
                    continue;

                }
                if (message.ToString().Contains("messageS"))
                {
                    TextMessages inMeseges = JsonUtility.FromJson<TextMessages>(message);
                    UpdateText(inMeseges);
                    //Debug.Log(" Mesege : " + message.ToString());
                    message = null;
                    continue;
                }
                if (message.ToString().Contains("artWroks"))
                {
                    Debug.Log("Accept dowload: " + GeneralState.AceptAssets);

                    Artworks listArtworks = JsonUtility.FromJson<Artworks>(message);
                    assetManager.UpdateArtwork = listArtworks.artWroks;
                    Debug.Log("ArtworkOnTheServer: " + listArtworks.artWroks.Count);

                    message = null;
                    continue;
                }
                if (message.ToString().Contains("DeleteArtWroks"))
                {
                    if (message.Split('@')[1]!=ArtistInfo.artistKey) 
                    assetManager.DeletArtWork(message.Split('@')[1]);
                    message = null;
                    continue;
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
                    message = null;
                    continue;
                }
                //Debug.Log("otherPlayers: " + otherPlayers.Count);
            }

            // if connection error, break the loop
            if (w.error != null)
            {
                Debug.Log("Error: " + w.error);
                break;
            }
            SendPositions();
            yield return 0;
        }

        // if error, close connection
        w.Close();
    }
    private void OnDisable()
    {
        w.Close();
        StopAllCoroutines();
        foreach (KeyValuePair<string, InfoPlayers> entry in infoPl)
        {
            Destroy(entry.Value.otherPlayer);
        }
        foreach (KeyValuePair<string, MessegeInfo> entry in _messeges)
        {
            Destroy(entry.Value.textObject);
        }
        infoPl.Clear();
        _messeges.Clear();
        print("disconect3d");
    }
    void UpdateText(TextMessages inMeseges)
    {
        for (int i = 0; i < inMeseges.messageS.Count; i++)
        {
            //Debug.Log(" xxxxx : " + inMeseges.messageS[i].position);/
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

    void UpdateLocalData(Players data)
    {
        // if number of players is not enough, create new ones
        for (int i = 0; i < data.players.Count; i++)
        {
            string playerID = data.players[i].id.ToString();
            //Debug.Log("data id " + data.players[i].rotation);
            //Debug.Log(i + "data id " + playerID);
            if (!infoPl.ContainsKey(playerID))
            {
                GameObject instance = Instantiate(otherPlayerObject, data.players[i].position, Quaternion.identity);
                instance.name = playerID;
                instance.GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", data.players[i].color);
                data.players[i].color.a = 225;
                instance.GetComponentInChildren<SpriteRenderer>().color = data.players[i].color;
                infoPl.Add(playerID, data.players[i]);
                infoPl[playerID].otherPlayer = instance;
            }
            else
            {
                if (infoPl.ContainsKey(playerID))
                {
                    infoPl[playerID].position = data.players[i].position;
                    infoPl[playerID].rotation = data.players[i].rotation;
                }
            }
        }
    }
    private void SendPositions()
    {
        // check if player moved
        if (Vector3.Distance(prevPosition , myPlayer.transform.position)>0.1f)
        {
            // send update if position had changed
            w.SendString(FormatMessege(myPlayer.transform));
            prevPosition = myPlayer.transform.position;
        }
    }
   
    public string FormatMessege (Transform _player)
    {
        return myGUID + "\t" + _player.position.x + "\t" + _player.position.y + "\t" + _player.position.z
        +"\t" + 0 + "\t" + _player.rotation.eulerAngles.y + "\t" +0;
    }
    public string FormatMessege(Transform _player,string msg,string type)
    {
        return myGUID + "\t" + _player.position.x + "\t" + _player.position.y + "\t" + _player.position.z
        + "\t" + 0 + "\t" + _player.rotation.eulerAngles.y + "\t" + 0 + "\t" + msg + "\t" + type;
    }
    public void askForArtwork()
    {
        GeneralState.AceptAssets = true;
        Debug.Log("Send Request to server for new artwork");
        w.SendString("RequestArtwork");
    }

}
// define classed needed to deserialize recieved data
[Serializable]
public class Players
{
    public List<InfoPlayers> players;
}
[Serializable]
public class InfoPlayers
{
    public Vector3 position;
    public Vector3 rotation;
    public Color32 color;
    public int timestamp;
    public string id;
    public GameObject otherPlayer;

}

[Serializable]
public class TextMessages
{
    public List<MessegeInfo> messageS;
}
[Serializable]
public class MessegeInfo
{
    public Vector3 position;
    public Vector3 rotation;
    public string text;
    public string id;
    public GameObject textObject;
}
