//////
///Note:
/// in LoaderObject there is a fileSize check to limit big file set at 12Mb
/////


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AsImpL;
using System.Linq;
using TMPro;
public class UXManager : MonoBehaviour
{
    public GameObject logInForm;
    public GameObject uploadForm;
    public GameObject OptionsUpload;

    public GameObject InspectorMode;
    public GameObject Worning;
    public GameObject PrefabRadar;
    public GameObject SpanArtwork;



    public Toggle fittingRoomToggle;
    public Toggle inspectorTogggle;
    public Toggle fpCameraToggle;
    Toggle keepLocation;
    Button spawnArtwork;


    public GameObject fittingRoomUi;
    public Animator uiAnim;
    public Button ArtWorkRequest;
    public Image Compas;

    GameObject cm_inspector;
    GameObject navigationUI;
    GameObject radar;
    GameObject cm_bird;
    CameraMode cmMode;
    ToggleGroup toggleGroup;
    AssetManager assetManger;
    UploadForm upForm;
    FittingRoom fittingRoom;
    CameraController cam2Controller;

    int count;
    // Start is called before the first frame update
    private void Awake()
    {
        spawnArtwork = SpanArtwork.GetComponentInChildren<Button>();
        keepLocation = SpanArtwork.GetComponentInChildren<Toggle>();
        cm_inspector = GameObject.FindGameObjectWithTag("SecondCamera");
        cm_bird = GameObject.FindGameObjectWithTag("MainCamera");
        fittingRoomUi.SetActive(false);
        cmMode = FindObjectOfType<CameraMode>();
        toggleGroup = FindObjectOfType<ToggleGroup>();
        upForm = GetComponent<UploadForm>();
        fittingRoom = FindObjectOfType<FittingRoom>();
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 30;
    }
    void Start()
    {
        cam2Controller = FindObjectOfType<CameraController>();
        inspectorTogggle.interactable = false;
        fittingRoomToggle.interactable = false;
        assetManger = FindObjectOfType<AssetManager>();
        assetManger.NewArtwork += NewArtWorkReques;

        FindObjectOfType<ObjectImporter>().ImportedModel += ActivateFittingRoom;
        FindObjectOfType<Multiplayer>().AccountVerified += AfterAccountVerification;
        fpCameraToggle.onValueChanged.AddListener(ActivateFPView);
        inspectorTogggle.onValueChanged.AddListener(StartInSpectorMode);
        fittingRoomToggle.onValueChanged.AddListener(StartFittingRoom);
        spawnArtwork.onClick.AddListener(SendMessegeToSpawn);
        navigationUI = InspectorMode.GetComponentsInChildren<Transform>(true)[4].gameObject;
        Button[] buttons = navigationUI.GetComponentsInChildren<Button>();
        buttons[1].onClick.AddListener(NextArtwork);
        buttons[0].onClick.AddListener(PrviousArtwork);
        SpanArtwork.gameObject.SetActive(false);
        cm_inspector.SetActive(false);
        navigationUI.SetActive(false);
        Compas.gameObject.SetActive(false);

    }

    void SendMessegeToSpawn()
    {
        ArtistInfo.keepInPlace = keepLocation.isOn;
        if (upForm.SendArtwork()) StopRadar();
    }


    /// Randar to look for a free parking spot. Is triggerd after the uploadOptions form is compleated
    public void ActivateRadar()
    {
        if (upForm.GatherColiderData())
        {
            Compas.gameObject.SetActive(true);
            SpanArtwork.gameObject.SetActive(true);
            if (radar == null)
            {
                radar = Instantiate(PrefabRadar);
                FindObjectOfType<RadarController>().InRangeOfArtwork += CheckForArtWorkAround;


            }
            else
            {
                radar.SetActive(false);
                radar.SetActive(true);

            }
            uiAnim.Play("GoUp");
        }
        if (FindObjectOfType<ColiderCheck>())
            FindObjectOfType<ColiderCheck>().StartCoroutine("Check");

    }

    public void StopRadar()
    {
        Compas.gameObject.SetActive(false);
        if (radar != null)
        {
            FindObjectOfType<RadarController>().InRangeOfArtwork -= CheckForArtWorkAround;
            Destroy(radar);
            radar = null;
        }
        SpanArtwork.gameObject.SetActive(false);

    }
    void CheckForArtWorkAround(bool state)
    {
        if (state)
            Compas.color = Color.red;
        else
            Compas.color = Color.blue;

    }



    void AfterAccountVerification(bool state)
    {
        /// Allow fitting room button to be active 
        if (assetManger.infoArwork.ContainsKey(ArtistInfo.artistKey))
        {
            ////Fill in Values
            upForm.FillInForms();
            FindObjectOfType<Multiplayer>().askForArtwork();
            uiAnim.Play("GoUp");
        }
        fittingRoomToggle.interactable = true;
        logInForm.SetActive(false);
        uploadForm.SetActive(true);
        NeutralState();


    }

    /// triggers fitting room  when object imported 
    void ActivateFittingRoom(GameObject gm, string st)
    {
        if (st == ArtistInfo.urlArt &gm.name=="Tester")
        {
            ArtistInfo.hasArt = true;
            GeneralState.AceptAssets = true;
            Debug.LogWarning("Test");
            upForm.UpdateExistingArtwork();
        }
        else
        {
            if(assetManger.infoArwork.ContainsKey(gm.name)&ArtistInfo.hasArt)
                StartFittingRoom(true);

        }
        //Debug.Log(gm.name);
        /// ActivateInspectorMode
        inspectorTogggle.interactable = true;
    }

    public void StartFittingRoom(bool state)
    {
        if (state)
        {
            /// Fitting Room
            if (ArtistInfo.hasArt)
            {
                if (assetManger.infoArwork.ContainsKey(ArtistInfo.artistKey))

                    if (assetManger.infoArwork[ArtistInfo.artistKey].@object != null)
                        if (assetManger.infoArwork[ArtistInfo.artistKey].@object.transform.childCount > 1)
                        {
                            ArtistInfo.busy = true;
                            if (!assetManger.infoArwork[ArtistInfo.artistKey].@object.GetComponent<ColiderCheck>())
                                assetManger.infoArwork[ArtistInfo.artistKey].@object.AddComponent<ColiderCheck>();
                            FindObjectOfType<ColiderCheck>().StartCoroutine("Check");
                            cm_inspector.SetActive(true);
                            cm_bird.SetActive(false);
                            fittingRoomUi.SetActive(true);
                            cam2Controller.target = assetManger.infoArwork[ArtistInfo.artistKey].@object.transform;
                            fittingRoom.StartCoroutine("StartFittingRoom");
                        }
            }
            else
            {
                /// Fill in form 
                uiAnim.Play("dropDown");
            }
        }
        else
        {
            DestroyColideCjeck();
            fittingRoom.coliderBox = null;
            StopSecondCamera();
            uiAnim.Play("GoUp");
            fittingRoom.StopAllCoroutines();
            ArtistInfo.busy = false;

        }

    }
    void DestroyColideCjeck()
    {
        if (ArtistInfo.hasArt)
        {
            if (assetManger.infoArwork.ContainsKey(ArtistInfo.artistKey))
            {
                Destroy(assetManger.infoArwork[ArtistInfo.artistKey].@object.GetComponent<ColiderCheck>());
                Destroy(fittingRoom.coliderBox);
            }


        }

    }
    /// <summary>
    /// todo create a seperete class for inspector mode when expanding
    /// </summary>
    /// <param name="StartInSpectorMode"></param>
    void StartInSpectorMode(bool state)
    {
        fittingRoomUi.SetActive(false);
        assetManger.StopAllCoroutines();
        cm_bird.SetActive(false);
        if (state & assetManger.infoArwork.Count > 1)
        {
            cm_inspector.SetActive(true);
            cam2Controller.target = assetManger.infoArwork.ElementAt(count).Value.@object.transform;

        }
        else
            NeutralState();
    }

    void NextArtwork()
    {
        if (count <= assetManger.infoArwork.Count - 2)
            count++;
        else
            count = 0;
        cam2Controller.target = assetManger.infoArwork.ElementAt(count).Value.@object.transform;

    }
    void PrviousArtwork()
    {
        if (count != 0)
            count--;
        else
            count = assetManger.infoArwork.Count - 1;
        cam2Controller.target = assetManger.infoArwork.ElementAt(count).Value.@object.transform;
    }
    void StopSecondCamera()
    {
        cm_inspector.SetActive(false);
        cm_bird.SetActive(true);
    }
    ////////////

    /// Activate FP view of main camera
    void ActivateFPView(bool state)
    {
        NeutralState();
        if (state)
        {
            cmMode.StopCoroutine("ZoomOut");
            cmMode.StartCoroutine("ZoomIn");
        }
        else
        {
            cmMode.StopCoroutine("ZoomIn");
            cmMode.StartCoroutine("ZoomOut");
        }
        //Debug.Log("CameraMode" + state);
    }


    /// Bird Camera
    void NeutralState()
    {
        cm_bird.SetActive(true);
        cm_inspector.SetActive(false);
        cmMode.StartReset();
        assetManger.StopAllCoroutines();
    }

    /// <summary>
    /// General Escpe
    /// </summary>
    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey)
        {
            //Debug.Log("Detected key code: " + e.keyCode);
            if (e.keyCode == KeyCode.Escape)
            {

                NeutralState();
                toggleGroup.SetAllTogglesOff();
                StartFittingRoom(false);

            }
        }
    }

    /// <summary>
    /// Callbacks on bad url or mesh data
    /// </summary>
    //todo a better action remove call from ObjectBuilder and LoaderObj
    public void BadMeshData(string key, string worning)
    {
        if (key == ArtistInfo.artistKey & !Worning.activeInHierarchy)
        {
            FindObjectOfType<Multiplayer>().w.SendString(key + '\t' + "DeleteArtwork");
            Worning.SetActive(true);
            Worning.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = worning;
            FindObjectOfType<Multiplayer>().w.SendString(ArtistInfo.artistKey + "DeleteArtwork");
        }
    }
    public void BadUrl(string url)
    {
        if (ArtistInfo.urlArt == url & !Worning.activeInHierarchy)
        {
            FindObjectOfType<Multiplayer>().w.SendString(ArtistInfo.artistKey + '\t' + "DeleteArtwork");
            Worning.SetActive(true);
            Worning.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Bad url";
            FindObjectOfType<Multiplayer>().w.SendString(ArtistInfo.artistKey + "DeleteArtwork");

        }
    }
    public void NewArtWorkReques(bool state)
    {
        ArtWorkRequest.interactable = true;
    }

}

