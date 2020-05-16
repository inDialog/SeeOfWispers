using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using AsImpL;
using TMPro;
public class InspectorManager : MonoBehaviour
{
    public GameObject InspectorMode;
    public GameObject ArtistInfoDropDown;
    public GameObject Bird;

    private CameraController cam2Controller;
    private GameObject teleportButton;
    private AssetManager assetManger;
    private TMP_Dropdown dropDown;
    private TMP_InputField searchField;
    private Text[] inputfields_ArtistInfo;
    private int count;

    private void Awake() //// activates ones time 
    {
        assetManger = FindObjectOfType<AssetManager>();
        cam2Controller = GetComponentInChildren<CameraController>();
        GameObject navigationUI = InspectorMode.GetComponentsInChildren<Transform>(true)[4].gameObject;
        dropDown = navigationUI.GetComponentInChildren<TMP_Dropdown>();
        searchField = navigationUI.GetComponentInChildren<TMP_InputField>();
        Button[] buttons = navigationUI.GetComponentsInChildren<Button>();
        inputfields_ArtistInfo = ArtistInfoDropDown.GetComponentsInChildren<Text>();
        teleportButton = GameObject.FindWithTag("BtnTeleport");
        Button telButton = teleportButton.GetComponent<Button>();

        telButton.onClick.AddListener(MoveCharacter);
        buttons[1].onClick.AddListener(NextArtwork);
        buttons[0].onClick.AddListener(PrviousArtwork);
        dropDown.onValueChanged.AddListener(ChangeDropDownValue);
        searchField.onEndEdit.AddListener(SearchForArtist);
        navigationUI.SetActive(false);
    }

    private void OnEnable() /// is callled every the inspectore mode in toggled in uxMnaganger
    {
        if (assetManger.infoArwork.Count > 0)
        {
            MoveCamera(cam2Controller, assetManger.infoArwork.ElementAt(count).Value.@object);
            InstantiateDropDown(assetManger.artistName); //Instantiate DropDown with the values received at enable
            ExtensionMethods.FillInputText(GETartistKey(dropDown.options[0].text), inputfields_ArtistInfo, assetManger);
        }
    }
    void SearchForArtist(string artist)
    {
        if (artist != "")
        {
            string tmp_key = "";
            tmp_key = GETartistKey(artist);
            if (tmp_key != "")
                dropDown.value = assetManger.infoArwork.Keys.ToList().FindIndex(tmp => tmp == tmp_key);
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
            MoveCamera(cam2Controller, assetManger.infoArwork[key].@object);
            ExtensionMethods.FillInputText(key, inputfields_ArtistInfo, assetManger);
        }
        else
            Debug.LogWarning("No value found for this given name in DropDown");
    }
    string GETartistKey(string artistNAME)
    {
        string tmp_keyvalue;
        tmp_keyvalue = assetManger.infoArwork.Where(temp => temp.Value.description.Split('\n')[0] == artistNAME)
                                             .Select(tmp => tmp.Key).FirstOrDefault();
        if (tmp_keyvalue != null)
            if (assetManger.infoArwork.ContainsKey(tmp_keyvalue))
                return tmp_keyvalue;
        return "";
    }
    //void FillInputText(string tmp_key, Text[] inputField, AssetManager astMan)
    //{
    //    if (tmp_key != "" & tmp_key != null)
    //    {
    //        string[] des_art;
    //        des_art = astMan.infoArwork[tmp_key].description.Split('\n');
    //        inputField[0].text = "";
    //        inputField[1].text = "";
    //        inputField[0].text = string.Format("Artist : {0}  -  Title : {1} - Format: {2} - Year : {3}",
    //                                           "NAME", des_art[0], des_art[2], (des_art[1].Split('-').Count() < 3 ? "****" : "20" + des_art[1].Split('-')[2]));
    //        inputField[1].text = des_art[3];
    //    }
    //    else
    //        Debug.LogWarning("They key for inputing text in the artist description its missing");
    //}
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
        if (count <= assetManger.infoArwork.Count - 2)
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
            count = assetManger.infoArwork.Count - 1;
        dropDown.value = count;
    }

    void MoveCamera(CameraController camCon, GameObject ArtContainer)
    {
        if (camCon == null && ArtContainer == null)
        {
            Debug.LogWarning("There is not values for Camera Controller or Transform target in MoveCamera");
            return;
        }
        GameObject containerMesh;
        if (!(containerMesh = ArtContainer.transform.GetChild(1).gameObject))
        {
            Debug.LogWarning("No mesh container associated with this Art Container");
            return;
        }
        Bounds tmp_mesh_bounds;
        GameObject tmp_target = new GameObject();
        tmp_mesh_bounds = ObjectBounds(containerMesh.transform);
        float biggest_side = Check_Sides_Values(tmp_mesh_bounds.size);
        tmp_target.transform.position = tmp_mesh_bounds.center;
        camCon.target = tmp_target.transform;
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
        Debug.Log(key);
        Teleport(key, Bird);
    }

    void Teleport(string artKey, GameObject obj)
    {
        if (artKey != "" & artKey != null)
        {
            GameObject artContainer;
            GameObject containerMesh;
            if (!(artContainer = assetManger.infoArwork[artKey].@object))
            {
                Debug.LogWarning("No Art container associated with this key");
                return;
            }
            if (!(containerMesh = artContainer.transform.GetChild(1).gameObject))
            {
                Debug.LogWarning("No mesh container associated with this Art Container");
                return;
            }
            Bounds tmp_mesh_bounds;
            tmp_mesh_bounds = ObjectBounds(containerMesh.transform);
            float biggest_side = Check_Sides_Values(tmp_mesh_bounds.size);
            obj.transform.position = tmp_mesh_bounds.center - new Vector3(0, 0, biggest_side);
        }
        else
            Debug.LogWarning("They key for teleporting object its missing");
    }
    float Check_Sides_Values(Vector3 vec3)
    {
        float tmp;
        tmp = 0;
        if (vec3.x > vec3.y && vec3.x > vec3.z)
            tmp = vec3.x;
        else if (vec3.y > vec3.x && vec3.y > vec3.z)
            tmp = vec3.y;
        else if (vec3.z > vec3.y && vec3.z > vec3.x)
            tmp = vec3.z;
        return (tmp);
    }

    Bounds ObjectBounds(Transform obj)
    {
        Bounds meshesBounds = new Bounds(obj.position, Vector3.zero);
        MeshRenderer[] mrs = obj.GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0; i < mrs.Length; i++)
        {
            if (i == 0) meshesBounds = mrs[i].bounds;
            else meshesBounds.Encapsulate(mrs[i].bounds);
        }
        return meshesBounds;
    }
}