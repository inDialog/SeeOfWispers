using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class ArtistTextManager : MonoBehaviour
{
    public GameObject ArtistInfoDropDown;
    private AssetManager assetManager;
    private string keyCollision;
    private Text[] inputfields_ArtistInfo;

    private bool hoverOverObject;
    private string keyHover;

    // Start is called before the first frame update
    void Start()
    {
        keyCollision = "";
        inputfields_ArtistInfo = ArtistInfoDropDown.GetComponentsInChildren<Text>();
        assetManager = FindObjectOfType<AssetManager>();
        hoverOverObject = false;
        keyHover = null;
    }
    private void OnGUI()
    {
        Event e = Event.current;
        if (e.button == 0 && e.isMouse && e.type == EventType.MouseDown)
        {
            Camera tmp_cam = FindActiveCamera();
            if (tmp_cam != null)
            {
                keyHover = GetKeyFromRay(tmp_cam);
                if (keyHover != null)
                {
                    hoverOverObject = true;
                    ExtensionMethods.FillInputText(keyHover, inputfields_ArtistInfo, assetManager);
                }
                else
                    keyHover = null;
            }
        }
        if (hoverOverObject && keyHover != null)
            GenerateRectangleOnGUI(keyHover, assetManager);
        if (e.button == 0 && e.isMouse && e.type == EventType.MouseUp)
        {
            hoverOverObject = false;
            keyHover = null;
        }
    }

    void GenerateRectangleOnGUI(string _key, AssetManager astMan)
    {
        string[] des_art;
        string FinalString;
        des_art = astMan.infoArwork[_key].description.Split('\n');
        FinalString = "";
        FinalString = string.Format("Artist : {0}  -  Title : {1} - Format: {2} - Year : {3}",
                                           "NAME", des_art[0], des_art[2], (des_art[1].Split('-').Count() < 3 ? "****" : "20" + des_art[1].Split('-')[2]));
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.name = FinalString;
        guiStyle.normal.textColor = Color.yellow;
        GUI.Label(new Rect(Input.mousePosition.x - 10, Screen.height - Input.mousePosition.y, 100, 100),""+ FinalString, guiStyle);
    }

    string GetKeyFromRay(Camera tmp_cam)
    {
        string tmp_key;
        Ray ray2;
        RaycastHit hit2;
        ray2 = tmp_cam.ScreenPointToRay(Input.mousePosition);
        tmp_key = null;
        if (Physics.Raycast(ray2, out hit2))
        {
            tmp_key = hit2.transform.root.name;
            if (assetManager.infoArwork.ContainsKey(tmp_key))
                return tmp_key;
            else
            {
                tmp_key = null;
                Debug.LogWarning("I do not have the key in the dictionary");
            }
        }
        return tmp_key;
    }

    Camera FindActiveCamera()
    {
        Camera tmp_camera;

        tmp_camera = null;
        foreach (Camera cam in Camera.allCameras)
            if (cam.isActiveAndEnabled && cam.name != "Map_Camera")
                tmp_camera = cam;
        return tmp_camera;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (keyCollision != collision.gameObject.transform.root.name)
        {
            keyCollision = collision.gameObject.transform.root.name;
            ExtensionMethods.FillInputText(keyCollision, inputfields_ArtistInfo, assetManager);
        }
    }
}
