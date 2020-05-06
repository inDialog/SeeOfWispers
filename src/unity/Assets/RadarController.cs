using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RadarController : MonoBehaviour
{
    GameObject mainCH;
    BoxCollider bc;
    public event Action<bool> InRangeOfArtwork;
    public List<string> isNotConvex = new List<string>();
    public List<GameObject> boxes = new List<GameObject>();

    AssetManager asset;
    private void OnEnable()
    {
        asset = FindObjectOfType<AssetManager>();

        mainCH = GameObject.FindGameObjectWithTag("Player");
        bc = FindObjectOfType<BoxCollider>();

        //bool[] options = ExtensionMethods.ConcertToBool(ArtistInfo.uploadOptionsA);
        //if (options[0])
        //{
        //    bc.size = ArtistInfo.colderSize;
        //}
        //else
        //{
        //    bc.size = Vector3.one;

        //}

        foreach (var item in asset.infoArwork)
        {
            if (!ExtensionMethods.ConcertToBool(item.Value.uploadOptions)[4])
            {
                isNotConvex.Add(item.Value.@object.transform.root.name);

            }
            MeshCollider[] meshFilters = item.Value.@object.transform.GetChild(1).GetComponentsInChildren<MeshCollider>();
            foreach (var item2 in meshFilters)
            {
                item2.convex = true;
            }
        }
        ShowBorder();
    }
    private void OnDisable()
    {
        AssetManager _as = FindObjectOfType<AssetManager>();
        foreach (var item in isNotConvex)
        {
            if (asset.infoArwork.ContainsKey(item))
            {
                if (_as.infoArwork[item].@object)
                {
                    MeshCollider[] meshFilters = _as.infoArwork[item].@object.transform.GetChild(1).GetComponentsInChildren<MeshCollider>();
                    foreach (var item2 in meshFilters)
                    {
                        item2.convex = false;
                    }
                }
            }
        }
        Clear();
    }
    private void LateUpdate()
    {
        this.transform.position = mainCH.transform.position;
        if (!ArtistInfo.hasArt)
        {
            if (boxes.Count != asset.infoArwork.Count)
            {
                Clear();
                ShowBorder();
            }
        }
        else
        {
            if (boxes.Count != asset.infoArwork.Count - 1)
            {
                Clear();
                ShowBorder();
            }
        }
         Debug.Log(ArtistInfo.hasArt.ToString() + "   a"+ asset.infoArwork.Count + "   b" + boxes.Count);

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Base")
        {
            InRangeOfArtwork(true);
            GeneralState.InRangeOfArtWork = true;
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Base")
        {
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
                if(item.Key!=ArtistInfo.artistKey)
                boxes.Add(FindObjectOfType<FittingRoom>().CreateColider(item.Value.@object.transform));
            
        }
    }


}
