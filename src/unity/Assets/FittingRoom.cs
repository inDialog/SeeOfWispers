using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
public class FittingRoom : MonoBehaviour
{
    public Slider SpliderScale;
    public GameObject coliderBox;
    public Material worningMaterial, normalMaterial;
    public Button DeleteArtwork;
    public InputField SetStep;
    public InputField SetMoveStep;
    private float step = 1;
    
    AssetManager assetManager;
    UploadForm upload;
    public TMP_Text log;
    // Start is called before the first frame update
    void Start()
    {

        assetManager = GetComponent<AssetManager>();
        SpliderScale.onValueChanged.AddListener(SetScale);
        upload = FindObjectOfType<UploadForm>();
        DeleteArtwork.onClick.AddListener(DestroyObject);
        SetStep.onEndEdit.AddListener(setScaleTreashold);
        SetMoveStep.onEndEdit.AddListener(setMoveStep);

    }
    public void MoveArtwork(string direction)
    {
        if (!ArtistInfo.hasArt) return;
        Vector3 addOn = assetManager.InfoArtwork[ArtistInfo.artistKey].@object.transform.GetChild(1).transform.position;
        switch (direction)
        {
            case "Right":
                addOn = new Vector3(addOn.x, addOn.y, addOn.z + step);
                break;
            case "Left":
                addOn = new Vector3(addOn.x, addOn.y, addOn.z - step);
                break;
            case "Up":
                addOn = new Vector3(addOn.x, addOn.y + step, addOn.z);
                break;
            case "Down":
                addOn = new Vector3(addOn.x, addOn.y - step, addOn.z);
                break;
            case "Fonnt":
                addOn = new Vector3(addOn.x + step, addOn.y , addOn.z);
                break;
            case "Back":
                addOn = new Vector3(addOn.x - step, addOn.y, addOn.z);
                break;
            default:
                break;

        }
        assetManager.InfoArtwork[ArtistInfo.artistKey].@object.transform.GetChild(1).transform.position = addOn;
    }
    void setScaleTreashold (string input)
    {
        int value = 0;
        int.TryParse(input, out value);
        SpliderScale.maxValue = value;
    }
    void setMoveStep(string input)
    {
        float.TryParse(input, out step);
    }

    void DestroyObject()
    {
        if(ArtistInfo.hasArt)
        assetManager.BroadcastDeleteArtwork(ArtistInfo.artistKey);
        if (coliderBox != null)
            Destroy(coliderBox);
    }

    public void SetScale(float value)
    {
        if (!ArtistInfo.hasArt) return;
        GameObject artwork = assetManager.InfoArtwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject;
        artwork.transform.localScale = new Vector3(value, value, value);
        UpdateArt(artwork);
        log.text = "SET SCALE :" + value;
        log.color = Color.white;

    }

    public void ResetPosition()
    {
        GameObject artwork = assetManager.InfoArtwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject;
        AjustPosition(artwork, assetManager.InfoArtwork[ArtistInfo.artistKey].colideScale);
    }



    void DisplayColider(Transform target)
    {
        if (coliderBox != null )
        {
            if (coliderBox.transform.localScale != ArtistInfo.colderSize)
                Destroy(coliderBox);

            if (GeneralState.colided)
                coliderBox.GetComponent<MeshRenderer>().material = worningMaterial;
            else
                coliderBox.GetComponent<MeshRenderer>().material = normalMaterial;
        }
        else
        {
           coliderBox = CreateColider(target);
           log.text = "perimeter create: " + assetManager.InfoArtwork[target.name].colideScale;

        }
    }
    public GameObject CreateColider(Transform target)
    {
        GameObject _coliderBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _coliderBox.name = "MyCube";
        Destroy(_coliderBox.GetComponent<BoxCollider>());
        _coliderBox.transform.position = target.position;
        _coliderBox.layer = 12;
        _coliderBox.transform.rotation = target.rotation;
        _coliderBox.transform.rotation = target.rotation;
        _coliderBox.transform.localScale = assetManager.InfoArtwork[target.name].colideScale;
        _coliderBox.GetComponent<MeshRenderer>().material = normalMaterial;

        //Debug.Log(ArtistInfo.colderSize);
        return _coliderBox;
    }
    public void UpdateArt(GameObject artwork)
    {
        FindObjectOfType<ColiderCheck>().StartCoroutine("Check");
        //Debug.Log(ObjectBounds(artwork.transform).size);
        upload.UpdateExistingArtwork();
    }

    public  void ResetRada()
    {
        if (FindObjectOfType<RadarController>())
        {
            FindObjectOfType<RadarController>().enabled = false;
            FindObjectOfType<RadarController>().enabled = true;
        }
    }

    Bounds ObjectBounds(Transform obj , out MeshRenderer[] mrs)
    {
        Bounds meshesBounds = new Bounds(obj.position, Vector3.zero);
        mrs = obj.GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0; i < mrs.Length; i++)
        {
            if (i == 0) meshesBounds = mrs[i].bounds;
            else meshesBounds.Encapsulate(mrs[i].bounds);
        }
        return meshesBounds;
    }
    public void AjustPosition(GameObject obj, Vector3 boxSize)
    {
        MeshRenderer[] mrs;
        Bounds meshesBounds = ObjectBounds(obj.transform, out mrs);
        obj.transform.localScale = Vector3.one / 100;

        for (int i = 0; i < mrs.Length; i++)
        {
            if (i == 0) meshesBounds = mrs[i].bounds;
            else meshesBounds.Encapsulate(mrs[i].bounds);
        }
        Vector3 target = obj.transform.position - meshesBounds.center;
        obj.transform.localPosition = target;
        Debug.Log("Moved to the center" + target);
        log.color = Color.red;
        log.text = "Re-scale to:" + obj.transform.localScale + " and moving in the center of the platform";

        //}
    }
    void SetLog(GameObject artwork,bool state)
    {
        if (!state)
        {
            log.color = Color.white;
            log.text = "ArtWork pos:" + artwork.transform.localPosition + '\n'
         + "artwork scale: " + artwork.transform.localScale;
        }
        else
        {
            log.color = Color.red;
            log.text = "ArtWork can be eighter :"
                +'\n' + "   -> outside bounds"
                +'\n' + "   -> to big "  
                +'\n' + "   -> to far away from  the  center "
                +'\n' + "ajust size and position";
            ;

        }

    }

    void RotateAroundPoint(Transform aTrans, Vector3 aPoint, Vector3 aAxis, float aAngle)
    {
        var dir = aTrans.position - aPoint;
        var q = Quaternion.AngleAxis(aAngle, aAxis);
        aTrans.position = aPoint + q * dir;
        aTrans.rotation = q * aTrans.rotation;
    }



    private IEnumerator StartFittingRoom()
    {
        log.text = "Starting Fitting room";
        Debug.Log("1§");
        while (true)
        {
            if (assetManager.InfoArtwork.ContainsKey(ArtistInfo.artistKey))
            {
                if (assetManager.InfoArtwork[ArtistInfo.artistKey].@object.transform.childCount > 1)
                {

                    DisplayColider(assetManager.InfoArtwork[ArtistInfo.artistKey].@object.transform);
                    GameObject artwork = assetManager.InfoArtwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject;
                    Vector3 addOn = artwork.transform.localPosition;
                    if (Input.anyKey)
                    {
                        SetLog(artwork, GeneralState.colided);
                    }
                 

                    if (Input.GetKey(KeyCode.C) & Input.GetMouseButton(1))
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
                                addOn = new Vector3(addOn.x, addOn.y, addOn.z + step);

                        }
                        if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            if (Input.GetKey(KeyCode.Space)) addOn = new Vector3(addOn.x, addOn.y - 1, addOn.z);
                            else
                                addOn = new Vector3(addOn.x, addOn.y, addOn.z - step);

                        }
                        if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            addOn = new Vector3(addOn.x + step, addOn.y, addOn.z);

                        }
                        if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            addOn = new Vector3(addOn.x - step, addOn.y, addOn.z);
                        }
                        //Debug.Log("Colision state = "+GeneralState.colided);
                        if (artwork.transform.localPosition != addOn)
                        {
                            artwork.transform.localPosition = addOn;
                            UpdateArt(artwork);
                        }
                        if (Input.GetKeyUp(KeyCode.C))
                        {
                            UpdateArt(artwork);
                        }
                    }
                }
            }
            yield return null;
        }
    }
}

