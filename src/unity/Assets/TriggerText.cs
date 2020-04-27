using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
public class TriggerText : MonoBehaviour
{
    public Sprite artistImage;
    public string artistDescription, shortDescription, urlArtis;

    public string sAction,eAction;
    static GameObject textUI;
    static Text sDescription, lDescription;
    Image image;
    Button button;

    CameraMode cm;
    public Material baseSkyMaterial;
    public Material newSkyMaterial;

    private void Start()
    {
        textUI = GameObject.FindGameObjectWithTag("ArtistUI");
        sDescription = textUI.GetComponentsInChildren<Text>()[0];
        lDescription = textUI.GetComponentsInChildren<Text>()[1];
        image = textUI.GetComponentsInChildren<Image>()[1];
        button = textUI.GetComponentsInChildren<Button>()[0];
        cm = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<CameraMode>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            sDescription.text = shortDescription;
            lDescription.text = artistDescription;
            image.sprite = artistImage;
            button.onClick.AddListener(CustomButton_onClick);
            textUI.SetActive(true);
            CheckForActions();
            if (baseSkyMaterial != null & newSkyMaterial != null)
                RenderSettings.skybox = newSkyMaterial;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            textUI.SetActive(false);
            button.onClick.RemoveAllListeners();
            CloseAcrions();
            Debug.Log("exit");
            if (baseSkyMaterial != null & newSkyMaterial != null)
                RenderSettings.skybox = baseSkyMaterial;
        }
    }

    void CheckForActions()
    {
        if (sAction != "")
        {
            cm.StopAllCoroutines();
            cm.StartCoroutine(sAction);
        }
    }
    void CloseAcrions()
    {
        if (eAction != "")
        {
            cm.StopAllCoroutines();
            cm.StartCoroutine(eAction);
        }
    }


#if UNITY_WEBGL && !UNITY_EDITOR
         [DllImport("__Internal")]
	private static extern void openWindow(string url);

    void CustomButton_onClick()
    {

        if (urlArtis.Contains("www"))
        {
         string Url_webpage = "http://" + urlArtis + "/";
         openWindow(Url_webpage);

        }

    }
#else
    void CustomButton_onClick()
    {

        if (urlArtis.Contains("www"))
        {
            Application.OpenURL("http://" + urlArtis + "/");
            print("App");
        }

    }
#endif
}

