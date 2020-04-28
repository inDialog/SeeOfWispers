using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;




public class Multiplayer : MonoBehaviour
{
    // define public game object used to visualize other players
    public GameObject otherPlayerObject;
    public GameObject otherTextPrefab;
    
    public GameObject myPlayer;
    public GameObject crena;

    public string url;
    private Vector3 prevPosition;
    //public Dictionary<string, GameObject> otherPlayers = new Dictionary<string, GameObject>();
    public Dictionary<string, InfoPlayers> infoPl = new Dictionary<string, InfoPlayers>();
    public Dictionary<string, MessegeInfo> _messeges = new Dictionary<string, MessegeInfo>();
    public event Action<bool> AccountVerified;
    public event Action<bool> NewArtwork;


    public Color32 myColor;
    //WebSocket w = new WebSocket(new Uri("ws://www.in-dialog.com:3002/socket.io/?EIO=4&transport=websocket"));
    //"ws://localhost:8000"
    public AssetManager assetManager;
    public WebSocket w;
    public System.Guid myGUID;

    private void Start()
    {
        myColor = RandomColor();
        myColor.a = 225;
        crena.GetComponent<Renderer>().material.SetColor("_EmissionColor", myColor);
        myPlayer.GetComponentInChildren<SpriteRenderer>().color = myColor;
        //GeneralState.AceptAssets = true;
    }
    private void OnEnable()
    {
        w = new WebSocket(new Uri(url));
        myGUID = System.Guid.NewGuid();
        //print(myGUID.ToString());
        StartCoroutine("Multyplayer");
    }


    IEnumerator Multyplayer()
    {
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
                Debug.Log(message.ToString());
                if (message.ToString() == "Conceted")
                {
                    w.SendString(myGUID + "\t" + StringToCollor(myColor) + "\t" + "color");
                    //Debug.Log("color" + myGUID + "\t" + StringToCollor(myColor) + "\t" + "color");
                    continue;
                }
                else if (message.ToString().Contains("Deleted"))
                {
                    string otherGUID = message.ToString().Split('@')[1];
                    //Debug.Log(" Deleted id: " + otherGUID);
                    Destroy(infoPl[otherGUID].otherPlayer);
                    infoPl.Remove(otherGUID);
                    continue;
                }
                if (message.ToString().Contains("messageS"))
                {
                    TextMessages inMeseges = JsonUtility.FromJson<TextMessages>(message);
                    UpdateText(inMeseges);
                    //Debug.Log(" Mesege : " + message.ToString());
                    continue;
                }
                if (message.ToString().Contains("artWroks"))
                {
                    //Artworks listArtworks = JsonUtility.FromJson<Artworks>(message);

                    //assetManager.UpdateArtwork = listArtworks.artWroks;

                    if (GeneralState.AceptAssets)
                    {
                        Artworks listArtworks = JsonUtility.FromJson<Artworks>(message);
                        assetManager.UpdateArtwork = listArtworks.artWroks;
                    }
                    else
                        NewArtwork(true);
                    //Debug.Log(" Mesege : " + listArtworks.artWroks.Count);
                    Debug.Log(GeneralState.AceptAssets);
                    continue;
                }
                if (message.ToString().Contains("players"))
                {
                    Players data = JsonUtility.FromJson<Players>(message);
                    UpdateLocalData(data);

                }
               
                if (message.ToString().Contains("artKey"))
                {
                    string artKey = message.Split('@')[1];
                    artKey = artKey.Remove(artKey.Length - 1);
                    artKey=  artKey.Replace(" ", "");
                    bool temp = ExtensionMethods.CheckKey(artKey);
                    if (temp)
                    {
                        ArtistInfo.activ = true;
                        ArtistInfo.artistKey  = artKey;
                        Debug.Log(ArtistInfo.artistKey);
                        AccountVerified(true);
                    }
                }
                continue;
                //Debug.Log("PlayerCount: " + data.players.Count);
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
        float distance = Vector3.Distance(prevPosition, myPlayer.transform.position);
        if (distance > 0.05f)
        {
            // send update if position had changed
            w.SendString(FormatMessege(myPlayer.transform));
            prevPosition = myPlayer.transform.position;
        }
    }


    public static string StringToCollor(Color32 Color)
    {
        return
        Color.r + "\t" +
        Color.g + "\t" +
        Color.b;
    }
    public static Color32 RandomColor()
    {
        return new Color32(
        (byte)UnityEngine.Random.Range(0, 200),
        (byte)UnityEngine.Random.Range(0, 200),
        (byte)UnityEngine.Random.Range(0, 200),
        225);
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
        Debug.Log(GeneralState.AceptAssets);
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
