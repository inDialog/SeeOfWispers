using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UXTools
{
    private static MonoBehaviour forCoroutine;
    private static GameObject mainChar;
    private static AssetManager astMan;

    private static bool hoverOverObject;
    private static string keyHover;
    private static string curKeyClick;
    private static Vector2 mousePos = Vector2.zero;

    public static void UXTols(MonoBehaviour currentMono, GameObject MainCharacter, AssetManager assetManager)
    {
        forCoroutine = currentMono;
        mainChar = MainCharacter;
        astMan = assetManager;
        hoverOverObject = false;
        keyHover = null;
        curKeyClick = null;
    }

    public static void MouseEvents(Event e)
    {
        if (e.button == 0 && e.isMouse && e.type == EventType.MouseDown)
        {
            Camera tmp_cam = FindActiveCamera();
            if (tmp_cam != null)
            {
                keyHover = GetKeyFromRay(tmp_cam);
                if (keyHover != null)
                {
                    hoverOverObject = true;
                    mousePos = e.mousePosition;
                }
            }
        }
        if (e.isMouse && (HoveringMouse(e, mousePos, 2f) || e.type == EventType.MouseUp))
        {
            hoverOverObject = false;
            keyHover = null;
        }
        if (hoverOverObject && keyHover != null)
            GenerateRectangleOnGUI(keyHover, astMan);
        if (e.button == 1 && e.isMouse && e.type == EventType.MouseDown)
        {
            Camera tmp_cam = FindActiveCamera();
            if (tmp_cam.name == "Main Camera")
            {
                string tmp_key;
                GameObject endPos = new GameObject();
                if (tmp_cam != null)
                {
                    tmp_key = GetKeyFromRay(tmp_cam);
                    if (tmp_key != null && curKeyClick != tmp_key)
                    {
                        curKeyClick = tmp_key;
                        endPos.transform.position = ExtensionMethods.LocationTeleport(tmp_key, astMan);
                        if (endPos.transform.position != Vector3.zero)
                        {
                            forCoroutine.StartCoroutine(LerpCharacterPos(endPos, mainChar, forCoroutine));
                        }
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(endPos);
                    }
                }
            }
        }
    }

    public static bool HoveringMouse(Event e, Vector2 initialPos, float precission)
    {
        bool hovering = true;
        if ((e.mousePosition.x >= initialPos.x - precission && e.mousePosition.x <= initialPos.x + precission)
            && (e.mousePosition.y >= initialPos.y - precission && e.mousePosition.y <= initialPos.y + precission))
        {
            return false;
        }

        return hovering;
    }

    /// <returns></returns>
    public static IEnumerator LerpCharacterPos(GameObject endPos, GameObject Character, MonoBehaviour stopCor)
    {
        if (endPos == null)
        {
            yield return null;
        }
        float elapsedTime = 0;
        float startDistance = Vector3.Distance(Character.transform.position, endPos.transform.position);
        float StartTime = Time.time;

        Quaternion lookOnLook =Quaternion.LookRotation(endPos.transform.position - Character.transform.position);
        float angleBetweenCharAndObj = Vector3.Angle(Character.transform.forward, endPos.transform.position - Character.transform.position);
        while (true)
        {
            float distCovered = (Time.time - StartTime) * 1.0f;
            float fractionOfJourney = distCovered / startDistance;
            float currentDistance = Vector3.Distance(Character.transform.position, endPos.transform.position);
            float percentage = (currentDistance / startDistance) * 100f;

            Character.GetComponent<Rigidbody>().isKinematic = true;
            Vector3 direction = endPos.transform.position - Character.transform.position;
            Quaternion toRotation = Quaternion.FromToRotation(Character.transform.forward, direction);
            if (Vector3.Angle(Character.transform.forward, endPos.transform.position - Character.transform.position) >= 1 && percentage >= 90f)
            {
                Character.transform.rotation = Quaternion.Slerp(Character.transform.rotation, lookOnLook, elapsedTime * 0.5f);
                elapsedTime += Time.deltaTime;
            }
            else
            {
                Character.transform.position = Vector3.Lerp(Character.transform.position, endPos.transform.position, fractionOfJourney);
                if (percentage <= 20f)
                {
                    Character.transform.rotation = Quaternion.Lerp(Character.transform.rotation, Quaternion.identity, fractionOfJourney);
                }
            }
            if (percentage <= 1f)
            {
                Debug.LogWarning("I am stopping the teleportation");
                Character.GetComponent<Rigidbody>().isKinematic = false;
                UnityEngine.Object.Destroy(endPos);
                stopCor.StopAllCoroutines();
            }
            yield return null;
        }
    }

    static void GenerateRectangleOnGUI(string _key, AssetManager astMan)
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
        GUI.Label(new Rect(Input.mousePosition.x - 10, Screen.height - Input.mousePosition.y, 100, 100), "" + FinalString, guiStyle);
    }
    public static string GetKeyFromRay(Camera tmp_cam)
    {
        string tmp_key;
        Ray ray2;
        RaycastHit[] hits;
        ray2 = tmp_cam.ScreenPointToRay(Input.mousePosition);
        tmp_key = null;
        hits = Physics.RaycastAll(ray2, Mathf.Infinity);
        for (int i = 0; i < hits.Length; i++)
        {
            MeshCollider meshCollider = hits[i].collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
            {
                Debug.LogWarning("I did not hit an mesh");
                tmp_key = null;
            }
            else
            {
                tmp_key = hits[i].transform.root.name;
                if (astMan.infoArwork.ContainsKey(tmp_key))
                    return tmp_key;
                else
                {
                    tmp_key = null;
                    Debug.LogWarning("I do not have the key in the dictionary");
                }
            }
        }
        return tmp_key;
    }
    public static Camera FindActiveCamera()
    {
        Camera tmp_camera;

        tmp_camera = null;
        foreach (Camera cam in Camera.allCameras)
            if (cam.isActiveAndEnabled && cam.name != "Map_Camera")
                tmp_camera = cam;
        return tmp_camera;
    }
}


//public static IEnumerator LerpCharacterPos(GameObject endPos, GameObject Character, MonoBehaviour stopCor)
//{
//    if (endPos == null)
//    {
//        yield return null;
//    }
//    float elapsedTime = 0;
//    float startDistance = Vector3.Distance(Character.transform.position, endPos.transform.position);
//    while (true)
//    {
//        float currentDistance = Vector3.Distance(Character.transform.position, endPos.transform.position);
//        float percentage = (currentDistance / startDistance) * 100f;
//        //Debug.Log("This is the the percentage distance ========" + percentage);
//        GameObject target;
//        int sideHit = -1;
//        if (sideHit != 0 && percentage > 25f)
//        {
//            sideHit = CheckDirectionForDeparture(Character, out target);
//            if (sideHit >= 1)
//            {
//                Debug.Log("I need to rotate at the biggining");
//                Debug.Log(sideHit);
//                Character.transform.RotateAround(target.transform.position, target.transform.root.up, 30 * Time.deltaTime);
//            }
//        }
//        if (sideHit == 0)
//        {
//            Vector3 direction = endPos.transform.position - Character.transform.position;
//            Quaternion toRotation = Quaternion.FromToRotation(Character.transform.forward, direction);
//            Character.transform.position = Vector3.Lerp(Character.transform.position, endPos.transform.position, elapsedTime * 0.25f);
//            Character.transform.rotation = Quaternion.Lerp(Character.transform.rotation, toRotation, elapsedTime * 0.25f);
//            //MainCharacter.transform.LookAt(endPos);

//            elapsedTime += Time.deltaTime;
//            if (percentage <= 20f)
//            {
//                Debug.Log(endPos.transform);
//                Character.transform.rotation = Quaternion.Lerp(Character.transform.rotation, Quaternion.identity, elapsedTime * 0.25f);
//                Character.transform.position = Vector3.Lerp(Character.transform.position, endPos.transform.position, elapsedTime * 0.25f);
//                elapsedTime += Time.deltaTime;
//                //Character.transform.LookAt(endPos.transform.parent.gameObject.transform);

//            }
//        }
//        if (percentage <= 5f)
//        {
//            UnityEngine.Object.Destroy(endPos);
//            stopCor.StopAllCoroutines();
//        }
//        yield return null;
//    }
//}
//static int CheckDirectionForDeparture(GameObject character, out GameObject gm)
//{
//    int direction;
//    direction = 0;
//    RaycastHit hitf;
//    RaycastHit hitl;
//    RaycastHit hitr;
//    Vector3 curPos = character.transform.position;
//    Vector3 rayDir1 = character.transform.forward;
//    Vector3 rayDir3 = (character.transform.forward + character.transform.right).normalized;
//    Vector3 rayDir2 = (character.transform.forward - character.transform.right).normalized;
//    gm = null;
//    if (Physics.Raycast(curPos, rayDir1, out hitf, 20f))
//    {
//        Debug.DrawLine(curPos, hitf.transform.position);
//        Debug.Log("I have hit forward an object");
//        direction = 1;
//        if (Physics.Raycast(curPos, rayDir2, out hitl, 10f))
//        {
//            direction = 2;
//            Debug.DrawLine(curPos, hitl.transform.position);
//            Debug.Log("I have hit left an object");
//        }
//        if (Physics.Raycast(curPos, rayDir3, out hitr, 10f))
//        {
//            if (direction == 2)
//                direction = 4;
//            else
//                direction = 3;
//            Debug.DrawLine(curPos, hitr.transform.position);
//            Debug.Log("I have hit right an object");
//        }
//        gm = hitf.transform.gameObject;
//    }
//    Debug.Log("This is my direction for rotation ============= " + direction);
//    return direction;
//}