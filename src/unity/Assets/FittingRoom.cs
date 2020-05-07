using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FittingRoom : MonoBehaviour
{
    public Slider mainSlider;
    AssetManager assetManager;
    public GameObject coliderBox;
    UploadForm upload;
    public Material worningMaterial, normalMaterial;
    bool holdingDown;
    // Start is called before the first frame update
    void Start()
    {
        assetManager = GetComponent<AssetManager>();
        mainSlider.onValueChanged.AddListener(SetScale);
        upload = FindObjectOfType<UploadForm>();
    }

    public void SetScale(float value)
    {
        GameObject artwork = assetManager.infoArwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject;
        artwork.transform.localScale = new Vector3(value, value, value);
        UpdateArt();
    }

    public void ResetPosition()
    {
        GameObject artwork = assetManager.infoArwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject;
        artwork.transform.position = assetManager.infoArwork[ArtistInfo.artistKey].@object.transform.position;
        artwork.transform.localScale = Vector3.one;
        artwork.transform.localEulerAngles = Vector3.zero;
        UpdateArt();
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
        }
    }
    public GameObject CreateColider(Transform target)
    {
        GameObject _coliderBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(_coliderBox.GetComponent<BoxCollider>());
        _coliderBox.transform.position = target.position;
        _coliderBox.layer = 12;
        _coliderBox.transform.rotation = target.rotation;
        _coliderBox.transform.rotation = target.rotation;
        _coliderBox.transform.localScale = assetManager.infoArwork[target.name].colideScale;
        _coliderBox.GetComponent<MeshRenderer>().material = normalMaterial;

        Debug.Log(ArtistInfo.colderSize);
        return _coliderBox;
    }
    public void UpdateArt()
    {
        FindObjectOfType<ColiderCheck>().StartCoroutine("Check");
        upload.UpdateExistingArtwork();
    }
    private IEnumerator StartFittingRoom()
    {
        while (true)
        {
            if (assetManager.infoArwork.ContainsKey(ArtistInfo.artistKey))
            {
                if (assetManager.infoArwork[ArtistInfo.artistKey].@object.transform.childCount > 1)
                {
                    DisplayColider(assetManager.infoArwork[ArtistInfo.artistKey].@object.transform);
                    GameObject artwork = assetManager.infoArwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject;
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
                        Debug.Log(GeneralState.colided);
                        if(artwork.transform.localPosition != addOn)
                        {
                            artwork.transform.localPosition = addOn;
                            UpdateArt();
                        }
                        if (Input.GetKeyUp(KeyCode.C))
                        {
                            UpdateArt();
                        }
                    }
                }
            }
            yield return null;
        }
    }
}

