using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CameraMode : MonoBehaviour
{
    public Toggle toggle;
    Camera mainCamera;
    GameObject originalState;
    float originalFOV;
    Vector3 Pos_FirstPerson = new Vector3(0, 0, 0.59f);
    StructuredVolumeSampling volumeSampling;
    MoveII moveII;
    public bool stop;
    LayerMask origginalMask;
    public LayerMask lm;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = this.GetComponent<Camera>();
        originalState = new GameObject();
        originalState.transform.localPosition = transform.localPosition;
        originalState.transform.localRotation = transform.localRotation;
        originalState.transform.localScale = this.transform.parent.localScale;
        origginalMask = mainCamera.cullingMask;
        originalFOV = mainCamera.fieldOfView;
        volumeSampling = GetComponent<StructuredVolumeSampling>();
        moveII = mainCamera.transform.parent.GetComponent<MoveII>();
        //QualitySettings.vSyncCount = 2;
        //Application.targetFrameRate = 30;
    }


    public IEnumerator ScaleDown()
    {
        while (true)
        {
            TrailRenderer[] tr = FindObjectsOfType<TrailRenderer>();

            foreach (var item in tr)
            {
                item.enabled = false;
            }
            moveII.WalkingOnly = true;
            float speed = 30 * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, originalState.transform.up - (originalState.transform.forward * 3), speed);
            this.transform.parent.transform.localScale = Vector3.Lerp(this.transform.parent.transform.localScale, originalState.transform.localScale / 25, 2 * Time.deltaTime);//Crane Scale
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, originalFOV * 2f, 2 * Time.deltaTime);//Camera Field of View
            if (this.transform.parent.transform.localScale == originalState.transform.localScale)
                break;

            if (Input.GetKey(KeyCode.Escape))
            {
                ScaleUp();
                break;
            }
            yield return null;
        }
    }

    public IEnumerator ScaleUp()
    {
        while (true)
        {
            TrailRenderer[] tr = FindObjectsOfType<TrailRenderer>();
            foreach (var item in tr)
            {
                item.enabled = true;
            }
            moveII.WalkingOnly = false;
            transform.localRotation = originalState.transform.rotation;

            float speed = 30 * Time.deltaTime;
            this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, originalState.transform.localPosition, speed);
            this.transform.parent.transform.localScale = Vector3.Lerp(this.transform.parent.transform.localScale, originalState.transform.localScale, 2 * Time.deltaTime);//Crane Scale
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, originalFOV, 2 * Time.deltaTime);//Camera Field of View
            if (this.transform.parent.transform.localScale == originalState.transform.localScale)
            {
                break;
            }
            yield return null;
        }
    }
    public IEnumerator ZoomIn()
    {
        StopCoroutine("ZoomOut");

        while (true)
        {
            if (transform.localPosition != Pos_FirstPerson)
            {
                float speed = 10 * Time.deltaTime;
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, Pos_FirstPerson, speed);
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, 80, 2 * Time.deltaTime);//Camera Field of View
                mainCamera.cullingMask = lm;
            }
            else
            {
                FreeCam();
                if (Input.GetKey(KeyCode.Escape))
                {
                    ZoomOut();
                    break;
                }
            }
            yield return null;
        }

    }
    public IEnumerator ZoomOut()
    {
        StopCoroutine("ZoomIn");
        while (true)
        {
            mainCamera.cullingMask = origginalMask;
            float speed = 30 * Time.deltaTime;
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, originalState.transform.localRotation, speed * 5);
            transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, originalState.transform.localPosition, speed);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, originalFOV, 2 * Time.deltaTime);//Camera Field of View
            if (this.transform.localPosition == originalState.transform.position & mainCamera.fieldOfView <= 45.1f)
            {
                break;
            }
            yield return null;
        }
    }
    public void StartReset()
    {
        StopAllCoroutines();
        this.transform.localPosition = originalState.transform.position;
        mainCamera.fieldOfView = 45.1f;
        transform.localRotation = originalState.transform.localRotation;
        mainCamera.cullingMask = origginalMask;
    }

    public float cameraSensitivity = 90;
    public float normalMoveSpeed = 10;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;

    void FreeCam()
    {
        if (Input.GetMouseButton(0))
        {
            Camera mycam = GetComponent<Camera>();
            float sensitivity = 0.01f;
            Vector3 vp = mycam.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mycam.nearClipPlane));
            vp.x -= 0.5f;
            vp.y -= 0.5f;
            vp.x *= sensitivity;
            vp.y *= sensitivity;
            vp.x += 0.5f;
            vp.y += 0.5f;
            Vector3 sp = mycam.ViewportToScreenPoint(vp);
            Vector3 v = mycam.ScreenToWorldPoint(sp);
            mainCamera.transform.LookAt(v, Vector3.up);
        }
    }
}
