using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactions : MonoBehaviour
{
    public string action = "";

    Vector3 Pos_FirstPerson = new Vector3(0, 0, 0.59f);
    Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);
    GameObject _cameraObject;
    Vector3 originalPos;
    Quaternion originalRot;
    Camera mainCamera;
    float originalFOV;
    int oiginalMask;
    public Material matDefault;
    public Material mat1;
    bool alreadyPassed;

  
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" & alreadyPassed == false)
        {
            _cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            mainCamera = _cameraObject.GetComponent<Camera>();
            originalPos = _cameraObject.transform.localPosition;
            originalRot = _cameraObject.transform.localRotation;
            originalFOV = mainCamera.fieldOfView;
            oiginalMask = mainCamera.cullingMask;
            alreadyPassed = true;
        }
    }
    ///---- execute change
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (action == "FP") ZoomIn(_cameraObject);
            else
            if (action == "Scale") ScaleDown(_cameraObject);
        }
    }
    ///--- revert to before state
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (action == "FP")
                StartCoroutine(ZoomOut(_cameraObject));
            else
            if (action == "Scale") StartCoroutine(ScaleUp(_cameraObject));
        }
    }
    ///////// --- Functions
    ///
    void ScaleDown(GameObject camera)
    {
        StopAllCoroutines();
        camera.transform.parent.transform.localScale = Vector3.Lerp(camera.transform.parent.transform.localScale, scale/10 ,2 *Time.deltaTime);//Crane Scale
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, originalFOV/2, 2 * Time.deltaTime);//Camera Field of View

    }
    private IEnumerator ScaleUp(GameObject camera)
    {
        while (true)
        {
            camera.transform.parent.transform.localScale = Vector3.Lerp(camera.transform.parent.transform.localScale, scale, 2 * Time.deltaTime);//Crane Scale
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, originalFOV, 2 * Time.deltaTime);//Camera Field of View
            if (camera.transform.parent.transform.localScale == scale)
            {
                break;
            }
            yield return null;
        }
    }
    void ZoomIn(GameObject camera)
    {
        StopAllCoroutines();
        if (camera.transform.localPosition != Pos_FirstPerson)
        {
            float speed = 10 * Time.deltaTime;
            camera.transform.localPosition = Vector3.MoveTowards(camera.transform.localPosition, Pos_FirstPerson, speed);
            camera.GetComponent<StructuredVolumeSampling>().enabled = false;
            RenderSettings.skybox = mat1;

        }
        else
        {
            mainCamera.cullingMask = 0;
            float _speed = Time.deltaTime * 70;
            mainCamera.transform.Rotate(new Vector3(Input.GetAxis("Vertical") * _speed, 0, 0));
            mainCamera.transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * _speed, 0));

        }

    }
    private IEnumerator ZoomOut(GameObject camera)
    {
        while (true)
        {

            mainCamera.cullingMask = oiginalMask;
            float speed = 30 * Time.deltaTime;
            camera.transform.localPosition = Vector3.MoveTowards(camera.transform.localPosition, originalPos, speed);
            RenderSettings.skybox = matDefault;
            if (camera.transform.localPosition == originalPos)
            {
            mainCamera.transform.localRotation = originalRot ;

                break;
            }
            yield return null;
        }
    }
}
