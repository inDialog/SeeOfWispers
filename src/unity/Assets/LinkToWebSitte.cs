using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
public class LinkToWebSitte : MonoBehaviour
{
    Button GotoWWebsite;
    // Start is called before the first frame update
    void Start()
    {
        GotoWWebsite = this.GetComponent<Button>();
        GotoWWebsite.onClick.AddListener(GoToWebsite);
    }

    // Update is called once per frame
    void GoToWebsite()
    {
        Debug.Log(UxInfo.curentArtisUrl);
        CustomButton_onClick(UxInfo.curentArtisUrl);
    }
    // Update is called once per frame
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
	private static extern void openWindow(string url);

    void CustomButton_onClick(string urlArtis)
    {

        if (urlArtis.Contains("www"))
        {
         string Url_webpage = "http://" + urlArtis + "/";
         openWindow(Url_webpage);

        }

    }
#else
    void CustomButton_onClick(string urlArtis)
    {

        if (urlArtis.Contains("www"))
        {
            if (!urlArtis.Contains("www"))
                Application.OpenURL("http://" + urlArtis + "/");
            else
                Application.OpenURL(urlArtis);

            print("App");
        }

    }
#endif
}
