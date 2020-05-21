using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FittingRoom : MonoBehaviour
{
    public Slider SpliderScale;
    public GameObject coliderBox;
    public Material worningMaterial, normalMaterial;
    public Button DeleteArtwork;
    public InputField SetStep;
    public InputField SetMoveStep;
    private float step = 1;
    public InputField[] XYZinputs;

    AssetManager assetManager;
    UploadForm upload;
    public TMP_Text log;
    Transform Artwork;
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
    InputField[] XYZ_I
    {
        set
        {
            value[0].text = Artwork.transform.localPosition.x.ToString();
            value[1].text = Artwork.transform.localPosition.y.ToString();
            value[2].text = Artwork.transform.localPosition.z.ToString();
        }
    }
    public void PositionXYZ()
    {
        if (!ArtistInfo.hasArt) return;
        float[] posion = new float[3];
        for (int i = 0; i < 4; i++)
        {
            float.TryParse(XYZinputs[i].text, out posion[i]);
        }
        assetManager.InfoArtwork[ArtistInfo.artistKey].@object.transform.GetChild(1).transform.position
            = new Vector3(posion[0], posion[1], posion[2]);
    }
    void setScaleTreashold (string input)
    {
        if (!ArtistInfo.hasArt) return;
        int value = 0;
        int.TryParse(input, out value);
        SpliderScale.maxValue = value;
    }
    void setMoveStep(string input)
    {
        if (!ArtistInfo.hasArt) return;
        float.TryParse(input, out step);
    }

    void DestroyObject()
    {
        if (!ArtistInfo.hasArt) return;
        assetManager.BroadcastDeleteArtwork(ArtistInfo.artistKey);
        if (coliderBox != null)
            Destroy(coliderBox);
    }

    public void SetScale(float value)
    {
        if (!ArtistInfo.hasArt) return;
        GameObject artwork = assetManager.InfoArtwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject;
        artwork.transform.localScale = new Vector3(value, value, value);
        UpdateArt();
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
        return _coliderBox;
    }
    public void UpdateArt()
    {
        FindObjectOfType<ColiderCheck>().StartCoroutine("Check");
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
        //// log
        log.color = Color.red;
        log.text = "Re-scale to:" + obj.transform.localScale + " and moving in the center of the platform";
        Debug.Log("Moved to the center" + target);
    }
    void SetLog(Transform artwork,bool state)
    {
        if (!state)
        {
            log.color = Color.white;
            log.text = "ArtWork pos:" + artwork.localPosition + '\n'
         + "artwork scale: " + artwork.localScale;
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
                    if (Artwork == null)
                        Artwork = assetManager.InfoArtwork[ArtistInfo.artistKey].@object.transform.GetChild(1).transform;
                    XYZ_I = XYZinputs;
                    DisplayColider(assetManager.InfoArtwork[ArtistInfo.artistKey].@object.transform);
                    if (Input.anyKey)
                    {
                        SetLog(Artwork, GeneralState.colided);
                    }
                    if (Input.GetKey(KeyCode.C) & Input.GetMouseButton(1))
                    {
                        float horizontalSpeed = 5.0F;
                        float verticalSpeed = 5.0F;
                        float h = horizontalSpeed * Input.GetAxis("Mouse X");
                        float v = verticalSpeed * Input.GetAxis("Mouse Y");
                        Artwork.transform.Rotate(v, h, 0);
                    }
                    else if (Input.GetKeyUp(KeyCode.C))
                    {
                        UpdateArt();
                    }

                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        if (Input.GetKey(KeyCode.Space))
                            MoveArtwork("Up");
                        else
                            MoveArtwork("Right");

                    }
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        if (Input.GetKey(KeyCode.Space))
                            MoveArtwork("Down");

                        else
                            MoveArtwork("Left");

                    }
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        MoveArtwork("Fonnt");

                    }
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        MoveArtwork("Back");
                    }
                }

            }
            yield return null;
        }
    }
    public void MoveArtwork(string direction)
    {
        GameObject artwork = assetManager.InfoArtwork[ArtistInfo.artistKey].@object.transform.GetChild(1).gameObject;
        if (!ArtistInfo.hasArt) return;
        Vector3 addOn = artwork.transform.position;
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
                addOn = new Vector3(addOn.x + step, addOn.y, addOn.z);
                break;
            case "Back":
                addOn = new Vector3(addOn.x - step, addOn.y, addOn.z);
                break;
            default:
                break;

        }
        artwork.transform.position = addOn;
        UpdateArt();

    }
}

