using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
public class InspectorManager : MonoBehaviour
{
    public GameObject InspectorMode;
    public GameObject PublicArtistInfo;
    public GameObject Bird;

    private CameraController cam2Controller;
    private GameObject teleportButton;
    private AssetManager assetManger;
    private TMP_Dropdown dropDown;
    private TMP_InputField searchField;
    public Text[] inputfields_ArtistInfo;
    private RawImage imgHolderArtist;
    private int count;
    private bool MoveAlsoCharacter;

    private void Awake() //// activates ones time 
    {
        assetManger = FindObjectOfType<AssetManager>();
        cam2Controller = GetComponentInChildren<CameraController>();
        dropDown = InspectorMode.GetComponentInChildren<TMP_Dropdown>();
        searchField = InspectorMode.GetComponentInChildren<TMP_InputField>();
        Button[] buttons = InspectorMode.gameObject.GetComponentsInChildren<Button>();
        teleportButton = GameObject.FindWithTag("Teleport");
        Button telButton = teleportButton.GetComponent<Button>();

        telButton.onClick.AddListener(MoveCharacter);
        buttons[1].onClick.AddListener(NextArtwork);
        buttons[0].onClick.AddListener(PrviousArtwork);
        dropDown.onValueChanged.AddListener(ChangeDropDownValue);
        searchField.onEndEdit.AddListener(SearchForArtist);
        FindObjectOfType<ArtistTextManager>().Colided += FillTextOnCollision;
        inputfields_ArtistInfo = PublicArtistInfo.GetComponentsInChildren<Text>();
        imgHolderArtist = PublicArtistInfo.GetComponentInChildren<RawImage>();
        InspectorMode.SetActive(false);
        UXTools.FillInputTextOthers(inputfields_ArtistInfo);
    }
    void FillTextOnCollision(string key, MonoBehaviour forCor)
    {
        if (key != "Null")
        {
            if (this.isActiveAndEnabled)
            {
                UXTools.FillInputText(key, inputfields_ArtistInfo, this, imgHolderArtist);
            }
            else if (!this.isActiveAndEnabled && forCor != null)
            {
                UXTools.FillInputText(key, inputfields_ArtistInfo, forCor, imgHolderArtist);
            }
        }
        else if (key == "Null")
        {
            imgHolderArtist.texture = null;
            Color currColor = imgHolderArtist.color;
            currColor.a = 0;
            imgHolderArtist.color = currColor;
        }
    }
    private void OnDisable()
    {
        InspectorMode.SetActive(false);
    }
    private void OnEnable() /// is callled every the inspectore mode in toggled in uxMnaganger
    {
        if (assetManger.InfoArtwork.Count > 0)
        {
            InspectorMode.SetActive(true);
            MoveCamera(cam2Controller, assetManger.InfoArtwork.ElementAt(count).Value.@object);
            InstantiateDropDown(assetManger.artistName); //Instantiate DropDown with the values received at enable
            UXTools.FillInputText(GETartistKey(dropDown.options[0].text), inputfields_ArtistInfo, this, imgHolderArtist);
            MoveAlsoCharacter = true;
        }
    }

    private void OnGUI()
    {
        //if (!Event.current.isMouse) return;
        //if (!this.isActiveAndEnabled | (ArtistInfo.busy & !Application.isPlaying)) return;
        //Event e = Event.current;
        //ChangeCameraPosition(e);
    }

    void ChangeCameraPosition(Event e)
    {
        if (e.button == 1 && e.isMouse && e.type == EventType.MouseDown)
        {
            Camera activeCamere = UXTools.FindActiveCamera();
            string tmp_key = UXTools.GetKeyFromRay(activeCamere);
            if (tmp_key != null && assetManger.InfoArtwork.ContainsKey(tmp_key))
            {
                string name;
                name = assetManger.InfoArtwork[tmp_key].description.Split('§')[0];
                dropDown.value = assetManger.artistName.IndexOf(name);
                if (MoveAlsoCharacter)
                {
                    MoveCharacter();
                }
            }
            else
            {
                Debug.LogWarning("This key its not valid in Change Camera Inspector");
            }
        }
    }

    void SearchForArtist(string artist)
    {
        if (artist != "")
        {
            string tmp_key = "";
            tmp_key = GETartistKey(artist);
            if (tmp_key != "")
                dropDown.value = assetManger.InfoArtwork.Keys.ToList().FindIndex(tmp => tmp == tmp_key);
            else
                Debug.LogWarning("There was no given key associated with this name in SearchBar");
        }
    }
    void ChangeDropDownValue(int i)
    {
        string artistSelected;
        artistSelected = dropDown.options[i].text;
        string key = GETartistKey(artistSelected);
        if (key != "")
        {
            MoveCamera(cam2Controller, assetManger.InfoArtwork[key].@object);
            UXTools.FillInputText(key, inputfields_ArtistInfo, this, imgHolderArtist);
        }
        else
            Debug.LogWarning("No value found for this given name in DropDown");
    }
    string GETartistKey(string artistNAME)
    {
        string tmp_keyvalue;
        tmp_keyvalue = assetManger.InfoArtwork.Where(temp => temp.Value.description.Split('§')[0] == artistNAME)
                                             .Select(tmp => tmp.Key).FirstOrDefault();
        if (tmp_keyvalue != null)
            if (assetManger.InfoArtwork.ContainsKey(tmp_keyvalue))
                return tmp_keyvalue;
        return "";
    }

    void InstantiateDropDown(List<string> artistName)
    {
        if (artistName.Count > 0)
        {
            dropDown.ClearOptions();
            dropDown.AddOptions(artistName);
        }
        else
            Debug.LogWarning("There are no elements in the artist name list");
    }

    void NextArtwork()
    {
        if (count <= assetManger.InfoArtwork.Count - 2)
            count++;
        else
            count = 0;
        dropDown.value = count;
    }
    void PrviousArtwork()
    {
        if (count != 0)
            count--;
        else
            count = assetManger.InfoArtwork.Count - 1;
        dropDown.value = count;
    }

    void MoveCamera(CameraController camCon, GameObject ArtContainer)
    {
        if (camCon == null && ArtContainer == null)
        {
            Debug.LogWarning("There is not values for Camera Controller or Transform target in MoveCamera");
            return;
        }
        //if (ArtContainer.transform.childCount<1)
        //{
        //    Debug.LogWarning("No artwork container associated with this Art Container");
        //    return;
        //}
        GameObject containerMesh;
        if (ArtContainer.transform.childCount >= 2)
        {
            containerMesh = ArtContainer.transform.GetChild(1).gameObject;
        }
        else
        {
            containerMesh = ArtContainer.transform.GetChild(0).gameObject;

        }
        Bounds tmp_mesh_bounds = ExtensionMethods.ObjectBounds(containerMesh.transform);
        camCon.target = containerMesh.transform;
        camCon.targetOffset =   containerMesh.transform.position - tmp_mesh_bounds.center;
        float biggest_side = ExtensionMethods.Check_Sides_Values(tmp_mesh_bounds.size);
        camCon.desiredDistance = tmp_mesh_bounds.size.magnitude + biggest_side / 2;
    }

    void MoveCharacter()
    {
        int index_artwork;
        string artistSelected;
        string key;

        index_artwork = dropDown.value;
        artistSelected = dropDown.options[index_artwork].text;
        key = GETartistKey(artistSelected);
        Bird.transform.position = ExtensionMethods.LocationTeleport(key, assetManger);
    }
}