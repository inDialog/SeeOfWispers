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
    public Button SignIn;
    public GameObject Wanternty;
    public TMP_InputField[] IdText;

    public TMP_InputField TextLogIn;
    // Start is called before the first frame update
    void Start()
    {
        multiplayer = FindObjectOfType<Multiplayer>();
        GetKey.onClick.AddListener(GetKeyFromWeb);
        LogIn.onClick.AddListener(VerifyKey);
        SignIn.onClick.AddListener(SignInStart);
        FindObjectOfType<Multiplayer>().AccountVerified += FillID;

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
        {
            TextLogIn.image.color = Color.yellow;
            Debug.LogWarning("KeyWrong");
        }
    }
    void SignInStart()
    {
        Wanternty.SetActive(true);
    }
    void FillID(bool tr)
    {
        SetId = IdText;
    }

    public TMP_InputField [] SetId
    {
        set
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i].IsActive())
                    value[i].text = ArtistInfo.artistKey;
            }
        }
    } 

}
