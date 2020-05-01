using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using AsImpL;
using UnityEngine.UI;



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
    public string url;
    public string description;
    public string uploadOptions;

    public string id;
    public GameObject @object;
    public string Name;
    public string SpawnState;



}


public class AssetManager : MonoBehaviour
{
    public Dictionary<string, InfoArtwork> infoArwork = new Dictionary<string, InfoArtwork>();
    public event Action<bool> NewArtwork;

    MultiObjectImporter moImporter;
    Multiplayer multiplayer;
    public GameObject prefabBase;
    public Slider mainSlider;
    public GameObject UploadForm;

    private void Start()
    {
        moImporter = GetComponent<MultiObjectImporter>();
        multiplayer = FindObjectOfType<Multiplayer>();
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
                    if (infoArwork[_id].url == value[i].url & infoArwork[_id].SpawnState == "FullySpawn")
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
                if(_id==ArtistInfo.artistKey)
                ArtistInfo.hasArt = true;

                if (i == value.Count - 1) GeneralState.AceptAssets = false;
            }

        }
    }
    void SpawnArtWork(List<InfoArtwork> value, string _id, int i)
    {
        infoArwork.Add(_id, value[i]);
        infoArwork[_id].@object = Instantiate(prefabBase);
        infoArwork[_id].@object.name = _id;
        infoArwork[_id].@object.transform.position = value[i].platform;
        if (GeneralState.AceptAssets)
        {
            //infoArwork[_id].Name = value[i].description.Split('\t')[i];
            ImportOptions optionsIm = new ImportOptions();
            optionsIm.localScale = value[i].artworkScale;
            optionsIm.localPosition = value[i].position;
            optionsIm.localEulerAngles = value[i].rotation;
            optionsIm.use32bitIndices = true;
            optionsIm.buildColliders = true;
            optionsIm.convertToDoubleSided = true;
            optionsIm.zUp = false;
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

    public void SendArtwork()
    {
        string toSend = "";
        if (!GatherString()) return;
        if (!infoArwork.ContainsKey(ArtistInfo.artistKey)) /// if  artist has no artwork then compose string  
                toSend = FormatMessege(this.transform, multiplayer.crena.transform.position - multiplayer.crena.transform.forward*(-1), ArtistInfo.urlArt, ArtistInfo.description, "ArtWork");
        else
        {
            if (ArtistInfo.keepInPlace)
            {
                Transform artwork = infoArwork[ArtistInfo.artistKey].@object.transform.GetChild(1).transform;
                toSend = FormatMessege(artwork, infoArwork[ArtistInfo.artistKey].@object.transform.position, ArtistInfo.urlArt, ArtistInfo.description, "ArtWork");
            }

            else
                toSend = FormatMessege(this.transform, multiplayer.crena.transform.position, ArtistInfo.urlArt, ArtistInfo.description, "ArtWork");
            /// if user changed artwork url 
            if (ArtistInfo.urlArt != infoArwork[ArtistInfo.artistKey].url)
            {
                infoArwork[ArtistInfo.artistKey].url = ArtistInfo.urlArt;
                //// destroy or keep old position
                DestroyObject();
            }
        }
        GeneralState.AceptAssets = true;
        if (GeneralState.InRangeOfArtWork | ArtistInfo.keepInPlace)
            multiplayer.w.SendString(toSend);
    }

    public bool GatherString()
    {
        TMP_InputField[] inputFields = UploadForm.GetComponentsInChildren<TMP_InputField>();
        if (CheckURl(inputFields[5].text))
        {
            ArtistInfo.urlArt = inputFields[5].text;
            ArtistInfo.description = ExtensionMethods.ComposeString(inputFields);
            return true;
        }
        else
            return false;
    }
    public void SetInputField()
    {
        TMP_InputField[] inputFields = UploadForm.GetComponentsInChildren<TMP_InputField>();
        string[] st = infoArwork[ArtistInfo.artistKey].description.Split('\n');
        for (int i = 0; i < st.Length; i++)
        {
            inputFields[i].text = st[i];
        }
        inputFields[5].text = infoArwork[ArtistInfo.artistKey].url;
    }


    public void UpdateExistingArtwork()
    {
        if (infoArwork[ArtistInfo.artistKey].@object.transform.childCount > 1)
        {
            GatherString();
            string toSend = FormatMessege(infoArwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject.transform,
                infoArwork[ArtistInfo.artistKey].@object.transform.position,
                ArtistInfo.urlArt,
                ArtistInfo.description,
                "ArtWork");
            multiplayer.w.SendString(toSend);

        }
    }

    void DestroyObject()
    {
        Destroy(infoArwork[ArtistInfo.artistKey].@object);
        infoArwork.Remove(ArtistInfo.artistKey);
        //StopAllCoroutines();
    }

    ///TODO add more condition  to check if actual mesh data exist before uploading 
    bool CheckURl(string url)
    {
        if (url == "")
            return false;
        if (url.Substring(url.Length - 3, 3) != "obj")
            return false;
        if (!url.Contains("http"))
            return false;
        Uri uriResult;
        return Uri.TryCreate(url, UriKind.Absolute, out uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

    }
    public string FormatMessege(Transform artWork, Vector3 platform, string url, string description, string type)
    {
        return ArtistInfo.artistKey
            + "\t" + artWork.localPosition.x + "\t" + artWork.localPosition.y + "\t" + artWork.localPosition.z
            + "\t" + artWork.localScale.x + "\t" + artWork.localScale.y + "\t" + artWork.localScale.z
            + "\t" + platform.x + "\t" + platform.y + "\t" + platform.z
            + "\t" + artWork.rotation.eulerAngles.x + "\t" + artWork.rotation.eulerAngles.y + "\t" + artWork.rotation.eulerAngles.z
            + "\t" + url + "\t" + description + "\t" + type + "\t";
    }

    private IEnumerator FittingRoom()
    {
        while (true)
        {
            if (infoArwork.ContainsKey(ArtistInfo.artistKey))
            {
                if (infoArwork[ArtistInfo.artistKey].@object.transform.childCount > 1)
                {
                    GameObject artwork = infoArwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject;
                    Vector3 addOn = artwork.transform.localPosition;
                    if (Input.GetKey(KeyCode.C) & Input.GetMouseButton(0))
                    {
                        float horizontalSpeed = 5.0F;
                        float verticalSpeed = 5.0F;
                        float h = horizontalSpeed * Input.GetAxis("Mouse X");
                        float v = verticalSpeed * Input.GetAxis("Mouse Y");
                        artwork.transform.Rotate(v, h, 0);
                    }
                    else
                    {
                        if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            if (Input.GetKey(KeyCode.Space)) addOn = new Vector3(addOn.x, addOn.y + 1, addOn.z);
                            else
                                addOn = new Vector3(addOn.x, addOn.y, addOn.z + 1);
                        }
                        if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            if (Input.GetKey(KeyCode.Space)) addOn = new Vector3(addOn.x, addOn.y - 1, addOn.z);
                            else
                                addOn = new Vector3(addOn.x, addOn.y, addOn.z - 1);
                        }
                        if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            addOn = new Vector3(addOn.x + 1, addOn.y, addOn.z);
                        }
                        if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            addOn = new Vector3(addOn.x - 1, addOn.y, addOn.z);
                        }

                        artwork.transform.localPosition = addOn;
                    }
                }
            }
            yield return null;
        }
    }
    public void SetScale()
    {
        GameObject artwork = infoArwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject;
        artwork.transform.localScale = new Vector3(mainSlider.value, mainSlider.value, mainSlider.value);
    }
  
   public  void ResetPosition()
    {
        GameObject artwork = infoArwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject;
        artwork.transform.position = infoArwork[ArtistInfo.artistKey].@object.transform.position;
        artwork.transform.localScale = Vector3.one;
        artwork.transform.localEulerAngles = Vector3.zero;

    }
    



}
