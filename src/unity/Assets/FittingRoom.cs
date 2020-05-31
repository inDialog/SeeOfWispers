using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class FittingRoom : MonoBehaviour
{
    public Slider SpliderScale;
    public GameObject coliderBox;
    public Material worningMaterial, normalMaterial;
    public Button DeleteArtwork;
    public InputField SetStep;
    public InputField SetMoveStep;
    private float step = 1;
    public InputField[] XYZ_Move;
    public InputField[] XYZ_Rotate;
    public GameObject PublicInfoPagge;
  public   GameObject artwork;
    AssetManager assetManager;
    UploadForm upload;
    public TMP_Text log;
  static int index = 1;

    // Start is called before the first frame update
    void Start()
    {

        assetManager = FindObjectOfType<AssetManager>();
        SpliderScale.onValueChanged.AddListener(SetScale);
        upload = FindObjectOfType<UploadForm>();
        DeleteArtwork.onClick.AddListener(DestroyObject);
        SetStep.onEndEdit.AddListener(setScaleTreashold);
        SetMoveStep.onEndEdit.AddListener(setMoveStep);

    }
    InputField[] XYZ_M
    {
        set
        {
            value[0].text = artwork.transform.localPosition.x.ToString();
            value[1].text = artwork.transform.localPosition.y.ToString();
            value[2].text = artwork.transform.localPosition.z.ToString();
        }
    }
    InputField[] XYZ_R
    {
        set
        {
            value[0].text = artwork.transform.localEulerAngles.x.ToString();
            value[1].text = artwork.transform.localEulerAngles.y.ToString();
            value[2].text = artwork.transform.localEulerAngles.z.ToString();
        }
    }
    public void PositionXYZ()
    {
        if (!ArtistInfo.hasArt) return;
        float[] posion = new float[3];
        for (int i = 0; i < 3; i++)
        {
            float.TryParse(XYZ_Move[i].text, out posion[i]);
        }
        assetManager.InfoArtwork[ArtistInfo.artistKey].@object.transform.GetChild(1).transform.localPosition
            = new Vector3(posion[0], posion[1], posion[2]);
    }
    public void RotationSet()
    {
        if (!ArtistInfo.hasArt) return;
        float[] posion = new float[3];
        for (int i = 0; i < 3; i++)
        {
            float.TryParse(XYZ_Rotate[i].text, out posion[i]);
        }
        assetManager.InfoArtwork[ArtistInfo.artistKey].@object.transform.GetChild(1).transform.localEulerAngles
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

    public void DestroyObject()
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
        _coliderBox.transform.localScale = assetManager.InfoArtwork[target.name].colideScale;
        _coliderBox.transform.position = assetManager.InfoArtwork[target.name].@object.transform.position;
        _coliderBox.transform.parent = assetManager.InfoArtwork[target.name].@object.transform;
        _coliderBox.transform.localPosition = Vector3.zero;
        _coliderBox.transform.rotation = target.rotation;
        _coliderBox.GetComponent<MeshRenderer>().material = normalMaterial;
        _coliderBox.layer = 12;
        return _coliderBox;
    }
    public void UpdateArt()
    {
        if(artwork.transform.root.GetComponent<ColiderCheck>())
            artwork.transform.root.GetComponent<ColiderCheck>().StartCoroutine("Check");
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
        PublicInfoPagge.SetActive(false);
        while (true)
        {
            while (artwork == null)
            {
                if (!assetManager.InfoArtwork.ContainsKey(ArtistInfo.artistKey))
                {
                    yield return null;
                    continue;
                }
                if (assetManager.InfoArtwork[ArtistInfo.artistKey].@object.transform.childCount == 1)
                {
                    yield return null;
                    continue;
                }
                artwork = assetManager.InfoArtwork[ArtistInfo.artistKey].@object.transform.GetChild(index).gameObject;
                if (!assetManager.InfoArtwork[ArtistInfo.artistKey].@object.GetComponent<ColiderCheck>())
                    assetManager.InfoArtwork[ArtistInfo.artistKey].@object.AddComponent<ColiderCheck>();

                artwork.transform.root.GetComponent<ColiderCheck>().StartCoroutine("Check");
                yield return null;
            }
            if (!assetManager.InfoArtwork.ContainsKey(ArtistInfo.artistKey))
            {
                artwork = null;
                yield return null;
                continue;
            }
            DisplayColider(assetManager.InfoArtwork[ArtistInfo.artistKey].@object.transform);

            FindObjectOfType<UXManager>().StartFittingRoomCamera(artwork.transform);
            artwork.transform.root.GetChild(0).localPosition = new Vector3(0, -ArtistInfo.colderSize.y/1.8f, 0);

            if (EventSystem.current.currentSelectedGameObject != null)
                if (EventSystem.current.currentSelectedGameObject.layer == 13)
                {
                    yield return null;
                    continue;
                }

            XYZ_M = XYZ_Move;
            XYZ_R = XYZ_Rotate;


            if (Input.anyKey)
            {
                SetLog(artwork.transform, GeneralState.colided);
            }

            Controller();

            yield return null;
        }
    }
    void Controller()
    {
        if (Input.GetKey(KeyCode.C) & Input.GetMouseButton(0))
        {
            float horizontalSpeed = 5.0F;
            float verticalSpeed = 5.0F;
            float h = horizontalSpeed * Input.GetAxis("Mouse X");
            float v = verticalSpeed * Input.GetAxis("Mouse Y");
            artwork.transform.Rotate(v, h, 0);
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

    public void MoveArtwork(string direction)
    {
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
   public void DestroyColideCjeck()
    {
        if (ArtistInfo.hasArt)
        {
            if (assetManager.InfoArtwork.ContainsKey(ArtistInfo.artistKey) & coliderBox != null)
            {
                Destroy(assetManager.InfoArtwork[ArtistInfo.artistKey].@object.GetComponent<ColiderCheck>());
                Destroy(coliderBox);
                coliderBox = null;
                artwork = null;
            }


        }

    }
    void SetLog(Transform artwork, bool state)
    {
        if (!state)
        {
            log.color = Color.white;
            log.text = "ArtWork pos:" + artwork.position + '\n'
         + "artwork scale: " + artwork.localScale;
        }
        else
        {
            log.color = Color.red;
            log.text = "ArtWork pos:" + artwork.position + '\n'
       + "artwork scale: " + artwork.localScale + '\n';
            log.text += "Posible Poroblems :"
                + '\n' + "   -> outside bounds"
                + '\n' + "   -> to big "
                + '\n' + "   -> to far away from the origin "
                + '\n' + "  -> or to low<Y axis must be above -2> "

                + '\n' + "ajust size and position";
            ;

        }

    }
}

