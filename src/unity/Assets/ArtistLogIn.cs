using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ArtistLogIn : MonoBehaviour
{
    Multiplayer multiplayer;
    public Button GetKey;
    public Button LogIn;

    public TMP_InputField TextLogIn;
    // Start is called before the first frame update
    void Start()
    {
        multiplayer = FindObjectOfType<Multiplayer>();
        GetKey.onClick.AddListener(GetKeyFromWeb);
        LogIn.onClick.AddListener(VerifyKey);
    }
    void GetKeyFromWeb()
    {
        multiplayer.w.SendString("GetKey");
        Debug.Log("send");
    }
    void VerifyKey()
    {
        string key = TextLogIn.text;
        if (ExtensionMethods.CheckKey(key))
        {
            multiplayer.w.SendString(key + '\t' + "CheckKey");
            Debug.Log(key + '\t' + "CheckKey");
        }
        else
            Debug.LogWarning("KeyWrong");
    }

}
