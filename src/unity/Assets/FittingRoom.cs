using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
public class FittingRoom : MonoBehaviour
{
    public Slider mainSlider;
    public GameObject coliderBox;
    public Material worningMaterial, normalMaterial;
    public Button DeleteArtwork;
    AssetManager assetManager;
    UploadForm upload;
    public TMP_Text log;
    Vector3 original;
    // Start is called before the first frame update
    void Start()
    {

        assetManager = GetComponent<AssetManager>();
        mainSlider.onValueChanged.AddListener(SetScale);
        upload = FindObjectOfType<UploadForm>();
        DeleteArtwork.onClick.AddListener(DestroyObject);
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
        GameObject artwork = assetManager.infoArwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject;
        artwork.transform.localScale = new Vector3(value, value, value);
        UpdateArt(artwork);
        log.text = "SET SCALE :" + value;
        log.color = Color.white;

    }

    public void ResetPosition()
    {
        GameObject artwork = assetManager.infoArwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject;
        AjustPosition(artwork, assetManager.infoArwork[ArtistInfo.artistKey].colideScale);
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
           log.text = "perimeter create: " + assetManager.infoArwork[target.name].colideScale;

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
        _coliderBox.transform.localScale = assetManager.infoArwork[target.name].colideScale;
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

        //if (boxSize == Vector3.zero)
        //    boxSize = Vector3.one * 10;

        //float[] distances = new float[3];
        //distances[0] = boxSize.x - meshesBounds.size.x;
        //distances[1] = boxSize.y - meshesBounds.size.y;
        //distances[2] = boxSize.z - meshesBounds.size.z;

        //if (distances[0] < 0 | distances[1] < 0 | distances[2] < 0)
        //{
        //    float[] temp = distances;
        //    Array.Sort(temp);
        //    int axis = Array.IndexOf(distances, temp[0]);
        //    float ratio = 1;
        //    if (axis == 0)
        //    {
        //        ratio = obj.transform.localScale.x / meshesBounds.size.x;
        //    }
        //    if (axis == 1)
        //    {
        //        ratio = obj.transform.localScale.x / meshesBounds.size.y;
        //    }
        //    if (axis == 2)
        //    {
        //        ratio = obj.transform.localScale.x / meshesBounds.size.z;
        //    }
        //    obj.transform.localScale = (ratio * obj.transform.localScale) * 2;

        //}
        //    Debug.Log("bounds size: " + meshesBounds.size + "ColideSize" + boxSize + "distances" + distances[0]);
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
        if (state)
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
    private IEnumerator StartFittingRoom()
    {
        log.text = "Starting Fitting room";
        Debug.Log("1§");
        while (true)
        {
            if (assetManager.infoArwork.ContainsKey(ArtistInfo.artistKey))
            {
                if (assetManager.infoArwork[ArtistInfo.artistKey].@object.transform.childCount > 1)
                {

                    DisplayColider(assetManager.infoArwork[ArtistInfo.artistKey].@object.transform);
                    GameObject artwork = assetManager.infoArwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject;
                    Vector3 addOn = artwork.transform.localPosition;
                    if (Input.anyKey)
                    {
                        SetLog(artwork, !GeneralState.colided);
                    }
                 

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
                        //Debug.Log("Colision state = "+GeneralState.colided);
                        if (artwork.transform.localPosition != addOn)
                        {
                            UpdateArt(artwork);
                            artwork.transform.localPosition = addOn;
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

