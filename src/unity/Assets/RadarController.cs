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
    BoxCollider bx;
    AssetManager asset;
    bool colide;
    private void OnEnable()
    {
        bx = GetComponent<BoxCollider>();
        if (ArtistInfo.colderSize != Vector3.zero)
            bx.size = ArtistInfo.colderSize;
        asset = FindObjectOfType<AssetManager>();
        mainCH = GameObject.FindGameObjectWithTag("Player");
        ExtensionMethods.ConvertConvexObjects(asset.InfoArtwork, out isNotConvex);
        ShowBorder();
        if (InRangeOfArtwork != null)
            InRangeOfArtwork(true);
        GeneralState.InRangeOfArtWork = true;
        bool colide;
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

        if (this.transform.position.y > GeneralState.Y_axisMax & colide == true)
        {
            if (InRangeOfArtwork != null)
            {
                InRangeOfArtwork(false);
            }
        }
        else
        {
            if (InRangeOfArtwork != null)
            {
                InRangeOfArtwork(true);
            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Base")
            colide = true;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Base")
            colide = false;

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
        foreach (var item in asset.InfoArtwork)
        {
            if (item.Key != ArtistInfo.artistKey& item.Value.colideScale!=Vector3.zero)
                boxes.Add(FindObjectOfType<FittingRoom>().CreateColider(item.Value.@object.transform));
        }
    }


}
