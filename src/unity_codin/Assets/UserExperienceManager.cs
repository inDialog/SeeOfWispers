using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Things to do :
///     Make the hovering and artist info in the same funtion
///     The camera 
/// </summary>
public class UserExperienceManager : MonoBehaviour
{
    public GameObject MainCharacter;

    public Action<string> Teleport;
    private AssetManager assetManager;

    // Start is called before the first frame update
    void Start()
    {
        assetManager = FindObjectOfType<AssetManager>();
        UXTools.UXTols(this, MainCharacter, assetManager);

        //uxTools =  UXTools(this, MainCharacter, assetManager);
    }
    private void OnGUI()
    {
        Event e = Event.current;
        UXTools.MouseEvents(e);

    }

}
