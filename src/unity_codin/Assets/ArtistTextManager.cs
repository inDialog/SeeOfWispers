using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
public class ArtistTextManager : MonoBehaviour
{
    
    private string keyCollision;
    public Action<string> Colided;


    // Start is called before the first frame update
    void Start()
    {
        keyCollision = "";

    }
    private void OnTriggerEnter(Collider other)
    {
        if (keyCollision != other.gameObject.transform.root.name && other.tag == "Base")
        {
            keyCollision = other.gameObject.transform.root.name;
            Colided(keyCollision);
        }
    }
}
