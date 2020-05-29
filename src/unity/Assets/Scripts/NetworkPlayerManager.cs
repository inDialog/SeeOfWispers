using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetworkPlayerManager
{
    public static Dictionary<string, InfoPlayers> infoPl = new Dictionary<string, InfoPlayers>();

    public static void UpdateLocalData(Players data, GameObject otherPlayerObject)
    {
        // if number of players is not enough, create new ones
        for (int i = 0; i < data.players.Count; i++)
        {
            string playerID = data.players[i].id.ToString();
            if (!infoPl.ContainsKey(playerID))
            {
                GameObject instance = GameObject.Instantiate(otherPlayerObject, data.players[i].position, Quaternion.identity) as GameObject;
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

    public static void SendPositions(ref WebSocket w, Func<Transform,string> FormatMessege)
    {
        // check if player moved
        if (Vector3.Distance(myPlayerInfo.prevPosition, myPlayerInfo.curPosition.position) > 0.1f)
        {
            w.SendString(FormatMessege(myPlayerInfo.curPosition));
            myPlayerInfo.prevPosition = myPlayerInfo.curPosition.position;
        }
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

public static class myPlayerInfo
{
    public static Vector3 prevPosition;
    public static Transform curPosition;
}