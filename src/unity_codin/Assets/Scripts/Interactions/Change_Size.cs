using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change_Size : MonoBehaviour
{
    public float ScaleMultiplayer = 2;
    public float Weight_FOV = 1/4;
    public float Weight_Cam_Posz = 3/4;
    public float Weight_Cam_Posy = 3 / 4;
    public Transform Crane_Transform;
    public Camera Main_Camera;
    public Vector3 Pos_ThirsPerson = new Vector3(0, 4.62f, -12.4f);

    private Vector3 InitialScale_Crane;
    private float Initial_FOV;

    private float transitionTime = 0.0f;
    private bool Inside_Box;
    private bool Outside_Box;

    private void Start()
    {
        InitialScale_Crane = Crane_Transform.localScale;
        Initial_FOV = Main_Camera.fieldOfView;
    }

    private void Update()
    {
        if (Inside_Box == true || Outside_Box == true)
        {
            transitionTime += Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (Inside_Box == true)
        {
            Crane_Transform.localScale = Vector3.Lerp(Crane_Transform.localScale, InitialScale_Crane / ScaleMultiplayer, transitionTime);//Crane Scale
            Main_Camera.fieldOfView = Mathf.Lerp(Main_Camera.fieldOfView, Initial_FOV - (ScaleMultiplayer * Weight_FOV), transitionTime);//Camera Field of View
            Main_Camera.transform.localPosition = new Vector3
                (
                Pos_ThirsPerson.x,
                Mathf.Lerp(Pos_ThirsPerson.y, Pos_ThirsPerson.y - (ScaleMultiplayer * Weight_Cam_Posy), transitionTime),
                Mathf.Lerp(Pos_ThirsPerson.z, Pos_ThirsPerson.z + (ScaleMultiplayer * Weight_Cam_Posz), transitionTime)
                ); // Camera Postion

            if (Crane_Transform.localScale == InitialScale_Crane / ScaleMultiplayer )
            {
                Inside_Box = false;
            }
        }
        if (Outside_Box == true)
        {
            Crane_Transform.localScale = Vector3.Lerp(Crane_Transform.localScale, InitialScale_Crane, transitionTime); //Crane Scale
            Main_Camera.fieldOfView = Mathf.Lerp(Main_Camera.fieldOfView, Initial_FOV, transitionTime);// Camera Field of View
            Main_Camera.transform.localPosition = Vector3.Lerp(Main_Camera.transform.localPosition, Pos_ThirsPerson, transitionTime); // Camera Position
            if (Crane_Transform.localScale == InitialScale_Crane && Main_Camera.transform.localPosition == Pos_ThirsPerson)
            {
                Outside_Box = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Scale")
        {
            Inside_Box = true;
            transitionTime = 0.0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Scale")
        {
            Outside_Box = true;
            transitionTime = 0.0f;
        }
    }
}
