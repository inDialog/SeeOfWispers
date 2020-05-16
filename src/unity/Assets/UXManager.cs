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

    //public GameObject InspectorMode;
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
    //GameObject navigationUI;
    GameObject radar;
    GameObject cm_bird;
    CameraMode cmMode;
    ToggleGroup toggleGroup;
    AssetManager assetManger;
    UploadForm upForm;
    FittingRoom fittingRoom;
    CameraController cam2Controller;

    //int count;
    // Start is called before the first frame update
    private void Awake()
    {
        spawnArtwork = SpanArtwork.GetComponentInChildren<Button>();
        keepLocation = SpanArtwork.GetComponentInChildren<Toggle>();
        cm_inspector = GameObject.FindGameObjectWithTag("SecondCamera");
        cm_bird = GameObject.FindGameObjectWithTag("MainCamera");
        cmMode = FindObjectOfType<CameraMode>();
        toggleGroup = FindObjectOfType<ToggleGroup>();
        upForm = GetComponent<UploadForm>();
        fittingRoom = FindObjectOfType<FittingRoom>();

        assetManger = FindObjectOfType<AssetManager>();
        cam2Controller = FindObjectOfType<CameraController>();
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 30;
    }
    void Start()
    {


        FindObjectOfType<ObjectImporter>().ImportedModel += ArtWorkPresent;
        FindObjectOfType<Multiplayer>().AccountVerified += AfterAccountVerification;
        assetManger.NewArtwork += NewArtWorkReques;

        inspectorTogggle.onValueChanged.AddListener(InspectorMode);
        fittingRoomToggle.onValueChanged.AddListener(StartFittingRoom);
        spawnArtwork.onClick.AddListener(SendMessegeToSpawn);
        fpCameraToggle.onValueChanged.AddListener(ActivateFPView);

        SpanArtwork.gameObject.SetActive(false);
        cm_inspector.SetActive(false);
        Compas.gameObject.SetActive(false);
        inspectorTogggle.interactable = false;
        fittingRoomToggle.interactable = false;
        fittingRoomUi.SetActive(false);

    }





    void AfterAccountVerification(bool state)
    {
        /// Allow fitting room button to be active 
        if (assetManger.infoArwork.ContainsKey(ArtistInfo.artistKey))
        {
            ////Fill in Values
            upForm.FillInForms();
            uiAnim.Play("GoUp");
        }
        FindObjectOfType<Multiplayer>().askForArtwork();
        fittingRoomToggle.interactable = true;
        logInForm.SetActive(false);
        uploadForm.SetActive(true);
        SStartBirdMode();
        fittingRoom.log.text = "Welcome: " + ArtistInfo.artistKey;
    }

    void ArtWorkPresent(GameObject gm, string st)
    {
        /// <!----> trigers oance a artist inport new art
        if (st == ArtistInfo.urlArt & gm.name == "Tester")
        {
            ArtistInfo.hasArt = true;
            GeneralState.AceptAssets = true;
            Debug.LogWarning("Test");
            upForm.UpdateExistingArtwork(true);
        }
        else
        {
            /// triggers fitting for artist  because artist has art
            if (assetManger.infoArwork.ContainsKey(gm.name) & ArtistInfo.hasArt)
                StartFittingRoom(true);
        }
        /// ActivateInspectorMode <!----> < remarks
        inspectorTogggle.interactable = true;


    }
    /// <summary></summary>
    /// Handels all  the callbacks for managing fiting room interactions and spawning permisions and ui
    /// Pariking rules sensenor 
    ///         1. radar for enviroment
    ///         2. <ColiderCheck> for within spwning space 

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

    void SendMessegeToSpawn()
    {
        ArtistInfo.keepInPlace = keepLocation.isOn;
        if (upForm.SendArtwork()) StopRadar();
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
                            Debug.Log("!");
                            fittingRoom.StartCoroutine("StartFittingRoom");
                            return;
                        }
            }
            else
            {
                /// there is no artwork present so default fill in form 
                uiAnim.Play("dropDown");
                return;
            }
        }
        else
        {
            DestroyColideCjeck();
            StopSecondCamera();
            if(uiAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name== "dropDown")
            uiAnim.Play("GoUp");
            fittingRoom.StopAllCoroutines();
            ArtistInfo.busy = false;

            SStartBirdMode();
            return;
        }

    }
    void DestroyColideCjeck()
    {
        if (ArtistInfo.hasArt)
        {
            if (assetManger.infoArwork.ContainsKey(ArtistInfo.artistKey)& fittingRoom.coliderBox != null)
            {
                Destroy(assetManger.infoArwork[ArtistInfo.artistKey].@object.GetComponent<ColiderCheck>());
                Destroy(fittingRoom.coliderBox);
                fittingRoom.coliderBox = null;
            }


        }

    }
    /// <summary>
    /// Truggers inspectomode class on 
    /// </summary>
    /// <param name="InspectorMode"></param>
    void InspectorMode(bool state)
    {
        fittingRoomUi.SetActive(!state);
        assetManger.StopAllCoroutines();
        cm_bird.SetActive(!state);
        if (state & assetManger.infoArwork.Count > 1)
        {
            cm_inspector.SetActive(state);
        }
        else
            SStartBirdMode();
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
        SStartBirdMode();
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
    void SStartBirdMode()
    {
        cm_bird.SetActive(true);
        cm_inspector.SetActive(false);
        cmMode.StartReset();
        assetManger.StopAllCoroutines();
    }

    /// <summary>
    /// General Escpe All roads must lead to natural state
    /// </summary>
    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey)
        {
            if (e.keyCode == KeyCode.Escape)
            {

                toggleGroup.SetAllTogglesOff();
                if (fittingRoomToggle.isOn)
                    StartFittingRoom(false);
                else
                    SStartBirdMode();
            }
        }
    }

    public void NewArtWorkReques(bool state, string id)
    {
        if (ExtensionMethods.CheckKey(id))
            fittingRoom.ResetRada();
        else
            ArtWorkRequest.interactable = true;
    }

    /// <summary>
    /// Callbacks on bad url or mesh data
    /// </summary>
    //todo a better action remove call from ObjectBuilder and LoaderObj
    public void BadMeshData(string key, string worning)
    {
        if (key == ArtistInfo.artistKey)
        {
            FindObjectOfType<Multiplayer>().w.SendString(key + '\t' + "DeleteArtwork");
            Worning.SetActive(true);
            Worning.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = worning;
            //FindObjectOfType<Multiplayer>().w.SendString(ArtistInfo.artistKey + "DeleteArtwork");
        }
    }
    public void BadUrl(string url)
    {
        if (ArtistInfo.urlArt == url)
        {
            FindObjectOfType<Multiplayer>().w.SendString(ArtistInfo.artistKey + '\t' + "DeleteArtwork");
            Worning.SetActive(true);
            Worning.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Bad url";
            //FindObjectOfType<Multiplayer>().w.SendString(ArtistInfo.artistKey + "DeleteArtwork");

        }
    }
  

}

