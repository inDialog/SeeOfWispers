using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class RadarController : MonoBehaviour
{
    GameObject mainCH;
    public event Action<bool> InRangeOfArtwork;
    List<string> isNotConvex = new List<string>();
    /// <summary>
    /// todo move the innot convex to extension method to be sheard with colider check
    /// as they beysicly do the same function at the beging and end
    /// </summary>
    List<GameObject> boxes = new List<GameObject>();

    AssetManager asset;

    private void OnEnable()
    {
        asset = FindObjectOfType<AssetManager>();
        mainCH = GameObject.FindGameObjectWithTag("Player");
        ExtensionMethods.ConvertConvexObjects(asset.infoArwork, out isNotConvex);
        ShowBorder();
    }
    private void OnDisable()
    {
        ExtensionMethods.RestitureConvertedObjects(isNotConvex);
        isNotConvex.Clear();
        Clear();
    }

    private void FixedUpdate()

    {
        this.transform.position = mainCH.transform.position;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Base")
        {
            if (InRangeOfArtwork != null)
                InRangeOfArtwork(true);
            GeneralState.InRangeOfArtWork = true;
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Base")
        {
            if (InRangeOfArtwork != null)
                InRangeOfArtwork(false);
            GeneralState.InRangeOfArtWork = false;
            //print("---");
        }

    }
    void Clear()
    {
        foreach (var item in boxes)
        {
            Destroy(item);
        }
        boxes.Clear();
    }
    void ShowBorder()
    {
        foreach (var item in asset.infoArwork)
        {
            if (item.Key != ArtistInfo.artistKey)
                boxes.Add(FindObjectOfType<FittingRoom>().CreateColider(item.Value.@object.transform));

        }
    }


}
