using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using AsImpL;

[Serializable]
public class Artworks
{
    public List<InfoArtwork> artWroks;
}
[Serializable]
public class InfoArtwork
{
    public Vector3 position;
    public Vector3 artworkScale;
    public Vector3 platform;
    public Vector3 rotation;
    public Vector3 colideScale;
    public string url;
    public string description;
    public string uploadOptions;
    public string id;
    /// </summary>
    public GameObject @object;
    public string SpawnState;
}


public class AssetManager : MonoBehaviour
{
    public Dictionary<string, InfoArtwork> infoArwork = new Dictionary<string, InfoArtwork>();
    public event Action<bool> NewArtwork;
    public GameObject prefabBase;
    MultiObjectImporter moImporter;

    private void Start()
    {
        moImporter = GetComponent<MultiObjectImporter>();
    }

    public List<InfoArtwork> UpdateArtwork
    {
        set
        {
            for (int i = 0; i < value.Count; i++)
            {
                string _id = value[i].id;

                if (!infoArwork.ContainsKey(_id))
                {
                    SpawnArtWork(value, _id, i);
                }
                else
                {
                    if (infoArwork[_id].url == value[i].url & infoArwork[_id].SpawnState == "FullySpawn" & infoArwork[_id].uploadOptions==value[i].uploadOptions)
                    {
                        infoArwork[_id].@object.transform.position = value[i].platform;
                        if (infoArwork[_id].@object.transform.childCount > 1)
                        {
                            GameObject artwork = infoArwork[_id].@object.transform.GetChild(1).gameObject;
                            artwork.transform.localPosition = value[i].position;
                            artwork.transform.localEulerAngles = value[i].rotation;
                            artwork.transform.localScale = value[i].artworkScale;
                        }
                    }
                    else
                    {
                        Destroy(infoArwork[_id].@object);
                        infoArwork.Remove(_id);
                        SpawnArtWork(value, _id, i);
                    }

                }
                if (_id == ArtistInfo.artistKey)
                {
                    ArtistInfo.hasArt = true;
                }

                if (i == value.Count - 1) GeneralState.AceptAssets = false;
            }

        }
    }
    void SpawnArtWork(List<InfoArtwork> value, string _id, int i)
    {
        infoArwork.Add(_id, value[i]);
        infoArwork[_id].@object = Instantiate(prefabBase);
        infoArwork[_id].@object.name = _id;
        infoArwork[_id].@object.tag = "Base";
        infoArwork[_id].@object.transform.position = value[i].platform;
        infoArwork[_id].uploadOptions = value[i].uploadOptions;

        bool[] importOption = ExtensionMethods.ConcertToBool(value[i].uploadOptions);
        if(importOption[0])
            infoArwork[_id].@object.GetComponent<BoxCollider>().size = value[i].colideScale;
        else
            infoArwork[_id].@object.GetComponent<BoxCollider>().size = Vector3.one;
        Debug.Log(value[i].colideScale);
        if (GeneralState.AceptAssets)
        {
            //infoArwork[_id].Name = value[i].description.Split('\t')[i];
            ImportOptions optionsIm = new ImportOptions();
            optionsIm.localScale = value[i].artworkScale;
            optionsIm.localPosition = value[i].position;
            optionsIm.localEulerAngles = value[i].rotation;
            optionsIm.use32bitIndices = true;
            optionsIm.buildColliders = importOption[1];
            optionsIm.tag = "Base";
            //optionsIm.colliderTrigger = true;
            optionsIm.convertToDoubleSided = importOption[2];
            optionsIm.zUp = importOption[3];
            optionsIm.colliderConvex = importOption[4];

            optionsIm.inheritLayer = true;

            moImporter.ImportModelAsync(value[i].id, value[i].url, infoArwork[_id].@object.transform, optionsIm);
            infoArwork[_id].SpawnState = "FullySpawn";
        }
        else
        {
            NewArtwork(true);
            infoArwork[_id].SpawnState = "OnlyPlatform";
        }
    }
   public void DestroyObject(string _key)
    {
        Destroy(infoArwork[_key].@object);
        infoArwork.Remove(_key);
        if (_key == ArtistInfo.artistKey)
            ArtistInfo.hasArt = false;
        FindObjectOfType<Multiplayer>().w.SendString(_key+ "DeleteArtwork");
    }
}
