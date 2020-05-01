using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UXPlayer : MonoBehaviour
{
    GameObject mainCH;

    public event Action<bool> InRangeOfArtwork;

    private void Start()
    {
    mainCH = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        this.transform.position = mainCH.transform.position;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Base")
        {
            InRangeOfArtwork(true);
            GeneralState.InRangeOfArtWork = true;
            print("+++");
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Base")
        {
            InRangeOfArtwork(false);
            GeneralState.InRangeOfArtWork = false;
            print("---");
        }

    }

}
