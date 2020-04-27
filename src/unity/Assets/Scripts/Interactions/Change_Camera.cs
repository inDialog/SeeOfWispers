using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change_Camera : MonoBehaviour
{

    public Camera Camera;

    public Vector3 Pos_ThirsPerson = new Vector3(0, 4.62f, -12.4f);
    public Vector3 Pos_FirstPerson = new Vector3(0, 0, 0.59f);

    private float transitionTime;
    private bool Inside_Cam;
    private bool Outside_Cam;

    private void Start()
    {
        Inside_Cam = false;
        Outside_Cam = false;
    }


    private void Update()
    {
        if (Inside_Cam == true || Outside_Cam == true)
        {
            transitionTime += Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        MoveCamera();
    }
    void MoveCamera()
    {
        if (Inside_Cam == true)
        {
            Camera.transform.localPosition = Vector3.Lerp(Camera.transform.localPosition, Pos_FirstPerson, transitionTime);
            if (Camera.transform.localPosition.y == Pos_FirstPerson.y)
            {
                Inside_Cam = false;
            }
        }
        if (Outside_Cam == true)
        {
            Camera.transform.localPosition = Vector3.Lerp(Camera.transform.localPosition, Pos_ThirsPerson, transitionTime);
            if (Camera.transform.localPosition.y == Pos_ThirsPerson.y)
            {
                Outside_Cam = false;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Camera_Change")
        {
            Inside_Cam = true;
            transitionTime = 0.0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Camera_Change")
        {
            Outside_Cam = true;
            transitionTime = 0.0f;
        }
    }


}
