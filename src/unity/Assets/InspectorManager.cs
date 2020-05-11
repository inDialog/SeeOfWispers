using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InspectorManager : MonoBehaviour
{
    CameraController cam2Controller;
    AssetManager assetManger;

    public GameObject InspectorMode;
    GameObject navigationUI;
    int count;
    private void Awake() //// activates ones time 
    {
        assetManger = FindObjectOfType<AssetManager>();
        cam2Controller = GetComponentInChildren<CameraController>();

        navigationUI = InspectorMode.GetComponentsInChildren<Transform>(true)[4].gameObject;
        Button[] buttons = navigationUI.GetComponentsInChildren<Button>();

        buttons[1].onClick.AddListener(NextArtwork);
        buttons[0].onClick.AddListener(PrviousArtwork);

        navigationUI.SetActive(false);
    }
    private void OnEnable() /// is callled every the inspectore mode in toggled in uxMnaganger

    {
        if (assetManger.infoArwork.Count>1)
        cam2Controller.target = assetManger.infoArwork.ElementAt(count).Value.@object.transform;

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

}
