using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UploadForm : MonoBehaviour
{
    Multiplayer multiplayer;
    AssetManager assetManager;
    // Start is called before the first frame update
    void Start()
    {
        assetManager = FindObjectOfType<AssetManager>();
        multiplayer = FindObjectOfType<Multiplayer>();
    }
    
    public bool SendArtwork()
    {
        if (FindObjectOfType<ColiderCheck>())
            FindObjectOfType<ColiderCheck>().StartCoroutine("Check");
        string toSend = "";
        if (!GatherString()) return false;
        if (!GatherColiderData()) return false;

        if (ArtistInfo.hasArt & ArtistInfo.keepInPlace) /// change everithing but the artworkInfo
        {
            if (!GeneralState.colided)
            {
                if (assetManager.infoArwork[ArtistInfo.artistKey].url != ArtistInfo.urlArt | ArtistInfo.uploadOptionsA != assetManager.infoArwork[ArtistInfo.artistKey].uploadOptions)
                {
                    ArtistInfo.hasArt = false;
                    assetManager.DeletArtWork(ArtistInfo.artistKey.ToString());
                }
                GeneralState.AceptAssets = true;
                toSend = FormatMessege(assetManager.infoArwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject.transform,
                        assetManager.infoArwork[ArtistInfo.artistKey].@object.transform.position,
                        ArtistInfo.colderSize,
                        ArtistInfo.urlArt,
                        ArtistInfo.description,
                        ArtistInfo.uploadOptionsA,
                        "ArtWork");
                multiplayer.w.SendString(toSend);
                return true;
            }
            else
                return false;
        }

        else /// change the position or  respwon a new object all together 
        {
            Vector3 frontalVector = multiplayer.crena.transform.position - multiplayer.crena.transform.forward * (-10);
            //if any changes to url destroy and veryfy again if not just update position
            if (ArtistInfo.hasArt)
            {
                if (GeneralState.colided)
                {
                    FindObjectOfType<FittingRoom>().ResetPosition();
                    //restartPosition(assetManager.infoArwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject);
                }
                toSend = FormatMessege(assetManager.infoArwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject.transform, frontalVector, ArtistInfo.colderSize, ArtistInfo.urlArt, ArtistInfo.description, ArtistInfo.uploadOptionsA, "ArtWork");


                if (assetManager.infoArwork[ArtistInfo.artistKey].url != ArtistInfo.urlArt | ArtistInfo.uploadOptionsA != assetManager.infoArwork[ArtistInfo.artistKey].uploadOptions)
                {
                    ArtistInfo.hasArt = false;
                    assetManager.DeletArtWork(ArtistInfo.artistKey.ToString());
                }
            }
            else
            {
                toSend = FormatMessege(this.transform, frontalVector, ArtistInfo.colderSize, ArtistInfo.urlArt, ArtistInfo.description, ArtistInfo.uploadOptionsA, "ArtWork");
            }
            if (GeneralState.InRangeOfArtWork)
            {
                GeneralState.AceptAssets = true;
                multiplayer.w.SendString(toSend);

                return true;
            }
            else
                return false;
        }

    }
    public void UpdateExistingArtwork()
    {

        string toSend = FormatMessege(assetManager.infoArwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject.transform,
            assetManager.infoArwork[ArtistInfo.artistKey].@object.transform.position,
           assetManager.infoArwork[ArtistInfo.artistKey].colideScale,
           assetManager.infoArwork[ArtistInfo.artistKey].url,
           assetManager.infoArwork[ArtistInfo.artistKey].description,
           assetManager.infoArwork[ArtistInfo.artistKey].uploadOptions,
            "ArtWork");
        if (!GeneralState.colided)
            multiplayer.w.SendString(toSend);
    }
    public void UpdateExistingArtwork(bool force)
    {

        string toSend = FormatMessege(assetManager.infoArwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject.transform,
            assetManager.infoArwork[ArtistInfo.artistKey].@object.transform.position,
           assetManager.infoArwork[ArtistInfo.artistKey].colideScale,
           assetManager.infoArwork[ArtistInfo.artistKey].url,
           assetManager.infoArwork[ArtistInfo.artistKey].description,
           assetManager.infoArwork[ArtistInfo.artistKey].uploadOptions,
            "ArtWork");
            multiplayer.w.SendString(toSend);
    }


    public string FormatMessege(Transform artWork, Vector3 platform, Vector3 perimeter, string url, string description, string uploadOptions, string type)
    {
        return ArtistInfo.artistKey
            + "\t" + artWork.localPosition.x + "\t" + artWork.localPosition.y + "\t" + artWork.localPosition.z
            + "\t" + artWork.localScale.x + "\t" + artWork.localScale.y + "\t" + artWork.localScale.z
            + "\t" + platform.x + "\t" + platform.y + "\t" + platform.z
            + "\t" + artWork.rotation.eulerAngles.x + "\t" + artWork.rotation.eulerAngles.y + "\t" + artWork.rotation.eulerAngles.z
            + "\t" + perimeter.x + "\t" + perimeter.y + "\t" + perimeter.z
            + "\t" + url + "\t" + description + "\t" + uploadOptions + "\t" + ArtistInfo.hasArt
            + "\t" + type + "\t";
    }

    public bool GatherString()
    {
        TMP_InputField[] inputFields = FindObjectOfType<UXManager>().uploadForm.GetComponentsInChildren<TMP_InputField>();

        if (inputFields[0].text == "")
            inputFields[0].text = "Timmy" + ArtistInfo.artistKey.Split('-')[3];

        if (ExtensionMethods.CheckURl(inputFields[5].text))
        {
            ArtistInfo.urlArt = inputFields[5].text;
            ArtistInfo.description = ExtensionMethods.ComposeString(inputFields);
            return true;
        }
        else
            return false;
    }
    public void FillInForms()
    {
        ArtistInfo.colderSize = assetManager.infoArwork[ArtistInfo.artistKey].colideScale;
        ArtistInfo.urlArt = assetManager.infoArwork[ArtistInfo.artistKey].url;
        ArtistInfo.uploadOptionsA = assetManager.infoArwork[ArtistInfo.artistKey].uploadOptions;
        //// Artist Information
        ///
        TMP_InputField[] inputFields = FindObjectOfType<UXManager>().uploadForm.GetComponentsInChildren<TMP_InputField>();
        string[] st = assetManager.infoArwork[ArtistInfo.artistKey].description.Split('\n');
        for (int i = 0; i < st.Length; i++)
        {
            inputFields[i].text = st[i];
        }
    
        inputFields[5].text = assetManager.infoArwork[ArtistInfo.artistKey].url;
        //// Upload uptions
        ///
        TMP_InputField[] _inputFields = FindObjectOfType<UXManager>().OptionsUpload.GetComponentsInChildren<TMP_InputField>();
        Toggle[] _toggles = FindObjectOfType<UXManager>().OptionsUpload.GetComponentsInChildren<Toggle>();
        bool [] states = ExtensionMethods.ConcertToBool(ArtistInfo.uploadOptionsA);
        if (states[4]==false)
            states[5] = false;
        for (int i = 0; i < _toggles.Length; i++)
        {
            _toggles[i].isOn = states[i];
        }
        if (_toggles[0])
        {
            _inputFields[0].text = ArtistInfo.colderSize.x.ToString();
            _inputFields[1].text = ArtistInfo.colderSize.y.ToString();
            _inputFields[2].text = ArtistInfo.colderSize.z.ToString();
        }

    }

    public bool GatherColiderData()
    {
        TMP_InputField[] inputFields = FindObjectOfType<UXManager>().OptionsUpload.GetComponentsInChildren<TMP_InputField>();
        Toggle[] toggles = FindObjectOfType<UXManager>().OptionsUpload.GetComponentsInChildren<Toggle>();
        ArtistInfo.uploadOptionsA = "";
        foreach (var item in inputFields)
        {
            item.image.color = Color.white;
        }
        if (toggles[0].isOn)
        {
            if (inputFields[0].text == "" || inputFields[1].text == "" || inputFields[2].text == "")
            {
                foreach (var item in inputFields)
                {
                    item.image.color = Color.yellow;
                }
                return false;
            }
            else
            {
                int max =(int)GeneralState.maxColideSize.x;
                float x = 1, y = 1, z = 1;
                float.TryParse(inputFields[0].text, out x);
                float.TryParse(inputFields[1].text, out y);
                float.TryParse(inputFields[2].text, out z);
                if (x > max)
                {
                    inputFields[0].text = max.ToString();
                    inputFields[0].image.color = Color.yellow;
                    return false;

                }
                if (y > max)
                {
                    inputFields[1].text = max.ToString();
                    inputFields[1].image.color = Color.yellow;
                    return false;

                }
                if (z > max)
                {
                    inputFields[2].text = max.ToString();
                    inputFields[2].image.color = Color.yellow;
                    return false;

                }

                ArtistInfo.colderSize = new Vector3(x, y, z);
            }
        }
        else
        {
            ArtistInfo.colderSize = Vector3.zero;
        }
        if (assetManager.infoArwork.ContainsKey(ArtistInfo.artistKey))
                assetManager.infoArwork[ArtistInfo.artistKey].colideScale = ArtistInfo.colderSize;
        ArtistInfo.uploadOptionsA = "";
        if (toggles[4].isOn ==false)
            toggles[5].isOn = false;
        for (int i = 0; i <= toggles.Length - 1; i++)
        {
            ArtistInfo.uploadOptionsA += (toggles[i].isOn.ToString() + '/');
        }
        //Debug.Log(ArtistInfo.colderSize + " " + ArtistInfo.uploadOptionsA);
        return true;
    }

    void restartPosition( GameObject obj)
    {
        FindObjectOfType<FittingRoom>().ResetPosition();

        //obj.transform.localScale = Vector3.zero;
        //if (log.IsActive())
        //{
        //    log.text =
        //        "Mesh out of bounds, Scale set to: 0 and Pos to: 0 " +
        //        "WORNING ACTION IS NOT SAVED TILL object ajusted corectly in  the colider!!!!";
        //    log.color = Color.red;
        //}



    }
}
