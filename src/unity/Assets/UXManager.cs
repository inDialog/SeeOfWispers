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
    public GameObject InspectorMode;
    public GameObject Worning;

    public Toggle fittingRoomToggle;
    public Toggle inspectorTogggle;
    public Toggle fpCameraToggle;
    public GameObject fittingRoomUi;
    public Animator uiAnim;
    public Button ArtWorkRequest;


    GameObject cm_inspector;
    GameObject navigationUI;

    GameObject cm_bird;
    CameraController cam2Controller;
    AssetManager assetManger;
    ToggleGroup toggleGroup;
    int count;
    CameraMode cmMode;
    // Start is called before the first frame update
    private void Awake()
    {
        cm_inspector = GameObject.FindGameObjectWithTag("SecondCamera");
        cm_bird = GameObject.FindGameObjectWithTag("MainCamera");
        fittingRoomUi.SetActive(false);
        cmMode = FindObjectOfType<CameraMode>();
        toggleGroup = FindObjectOfType<ToggleGroup>();

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
    }
    void Start()
    {
        cam2Controller = FindObjectOfType<CameraController>();
        inspectorTogggle.interactable = false;
        fittingRoomToggle.interactable = false;
        assetManger = FindObjectOfType<AssetManager>();
        FindObjectOfType<ObjectImporter>().ImportedModel += ActivateFittingRoom;
        FindObjectOfType<Multiplayer>().AccountVerified += EnableFittingRoom;
        FindObjectOfType<Multiplayer>().NewArtwork += NewArtWorkReques;

        fpCameraToggle.onValueChanged.AddListener(ActivateFPView);
        inspectorTogggle.onValueChanged.AddListener(StartInSpectorMode);
        fittingRoomToggle.onValueChanged.AddListener(StartFittingRoom);

        navigationUI = InspectorMode.GetComponentsInChildren<Transform>(true)[4].gameObject;
        Button[] buttons = navigationUI.GetComponentsInChildren<Button>();
        buttons[1].onClick.AddListener(NextArtwork);
        buttons[0].onClick.AddListener(PrviousArtwork);


        //cm_inspector.transform.GetChild(1).gameObject.SetActive(false);
        //print(Worning.transform.childCount);
        //cm_bird.SetActive(false);
        cm_inspector.SetActive(false);
        navigationUI.SetActive(false);

    }

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
        Debug.Log("CameraMode" + state);
    }

    /// Allow fitting room button to be active if user has key
    void EnableFittingRoom(bool state)
    {
        if (assetManger.infoArwork.ContainsKey(ArtistInfo.artistKey))
        {
            ArtistInfo.hasArt = true;
            uiAnim.Play("GoUp");
        }
        NeutralState();
        fittingRoomToggle.interactable = true;
        logInForm.SetActive(false);
        uploadForm.SetActive(true);
        GeneralState.AceptAssets = true;
       
    }

    /// triggers fitting room  when object loaded and set inspectorMode interacteble
    void ActivateFittingRoom(GameObject gm, string st)
    {
        if (st == ArtistInfo.urlArt)
        {
            StartFittingRoom(true);
        }
        inspectorTogggle.interactable = true;
    }

    public void StartFittingRoom(bool state)
    {
        if (state)
        {
            /// Fitting Room
            if (assetManger.infoArwork.ContainsKey(ArtistInfo.artistKey))
            {
                if (assetManger.infoArwork[ArtistInfo.artistKey].@object != null)
                    if (assetManger.infoArwork[ArtistInfo.artistKey].@object.transform.childCount > 1)
                    {
                        ArtistInfo.busy = true;
                        cm_inspector.SetActive(true);
                        cm_bird.SetActive(false);
                        fittingRoomUi.SetActive(true);
                        FindObjectOfType<MoveII>().enabled = false; //move function TODO find a better sollution
                        cam2Controller.target = assetManger.infoArwork[ArtistInfo.artistKey].@object.transform;
                        assetManger.StartCoroutine("FittingRoom");
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
            assetManger.StopAllCoroutines();
            StopSecondCamera();
            uiAnim.Play("GoUp");
            ArtistInfo.busy = false;

        }

    }

    /// <summary>
    /// Inspector Mode
    /// </summary>
    void StartInSpectorMode(bool state)
    {
        fittingRoomUi.SetActive(false);
        assetManger.StopAllCoroutines();
        cm_bird.SetActive(false);
        if (state & assetManger.infoArwork.Count > 1)
        {
            FindObjectOfType<MoveII>().enabled = false; //move function TODO find a better sollution
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
        //navigationUI.SetActive(false);
        cm_bird.SetActive(true);
        FindObjectOfType<MoveII>().enabled = true; //move function TODO find a better sollution
    }

    /// Bird Camera
    void NeutralState()
    {
        cm_bird.SetActive(true);
        cm_inspector.SetActive(false);
        cmMode.StartReset();
        FindObjectOfType<MoveII>().enabled = true; //move function TODO find a better sollution
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey)
        {
            //Debug.Log("Detected key code: " + e.keyCode);
            if (e.keyCode ==  KeyCode.Escape)
            {
                NeutralState();
               
                toggleGroup.SetAllTogglesOff();
            }
        }
    }
    //todo a better action remove call from ObjectBuilder and LoaderObj
    public void BadMeshData(string key)
    {
        if (key == ArtistInfo.artistKey)
        {
            FindObjectOfType<Multiplayer>().w.SendString(key + '\t' + "DeleteArtwork");
            Worning.SetActive(true);
            Worning.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Bad mesh Data";
        }
    }
    public void BadUrl(string url)
    {
        if (ArtistInfo.urlArt == url)
        {
            FindObjectOfType<Multiplayer>().w.SendString(ArtistInfo.artistKey + '\t' + "DeleteArtwork");
            Worning.SetActive(true);
            Worning.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Bad url";
        }
    }
    public void NewArtWorkReques(bool state)
    {
        ArtWorkRequest.interactable = true;
    }

}

