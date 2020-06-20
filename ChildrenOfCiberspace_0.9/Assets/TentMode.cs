using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentMode : MonoBehaviour
{
    MoveII Move;
    CameraMode cameraMode;
    Material skyMaterialBasic ;
    public Material newSkyMaterial;
    public LayerMask lm;
    // Start is called before the first frame update
    void Start()
    {
        Move = FindObjectOfType<MoveII>();
        cameraMode = FindObjectOfType<CameraMode>();
        skyMaterialBasic = RenderSettings.skybox;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            Move.WalkingOnly = true;
     
            Camera.main.cullingMask = lm;
            RenderSettings.skybox = newSkyMaterial;

            cameraMode.StartCoroutine("ZoomIn");

        }


    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Move.WalkingOnly = false;
            cameraMode.StartCoroutine("ZoomOut");
            RenderSettings.skybox = skyMaterialBasic;
        }
    }
}
