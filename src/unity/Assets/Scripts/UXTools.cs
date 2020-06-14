using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class UxInfo
{
    public static string curentArtisUrl = "";
}

public static class UXTools
{
    private static MonoBehaviour forCoroutine;
    private static GameObject mainChar;
    private static AssetManager astMan;

    private static bool hoverOverObject;
    private static string keyHover;
    private static string curKeyClick;
    private static Vector2 mousePos = Vector2.zero;
    static string[] dictonery = {
        "now", "here","happening", "will happen",
        "<b>madness</b>",
        "a pig prostituting for corn flower",
        "a unicorn screaming in a jar of pickecles",
        "a duck cheeting a snake at poker",
        "a frog jumping in front of the car",
        "<b>madness</b>",
        "to forget", "to be forgoten", "to forgive", "to force",
        "nothing","maybe","posibly","...","what we say it is" };
   static string ASCI = "|_!#$%&\'()*+,-./:;<=>?@[\\]^_`{}~";
   static string DescriptionRosny = "L'exposition Retour à l'Anormal vous amène dans le Cyberespace pour découvrir les travaux des élèves de la Fabrique Artistique et Numérique Explorez une galaxie constituée des restitutions d'ateliers d'arts plastiques et numériques, des résidences d'artistes, des partenariats avec l'éducation nationale, l'IME et Ecole de la 2e chance. " + '\n' +
       " Tout est organisé pour garder les pieds sur terre et permettre d'avoir la tête dans les toiles ! " + '\n' +
        " Bonne exploration ! " + '\n';


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
        if (e.button == 0)
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
        else if (HoveringMouse(e, mousePos, 2f) || e.type == EventType.MouseUp)
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
        Vector3 startPos = Character.transform.position;
        Quaternion lookOnLook = Quaternion.LookRotation(endPos.transform.position - Character.transform.position);
        while (true)
        {
            float distCovered = (Time.time - StartTime) * 1.0f;
            float fractionOfJourney = distCovered / startDistance;
            float currentDistance = Vector3.Distance(Character.transform.position, endPos.transform.position);
            float percentage = (currentDistance / startDistance) * 100f;
            Character.GetComponent<Rigidbody>().isKinematic = true;
            Vector3 direction = endPos.transform.position - Character.transform.position;
            if (Vector3.Angle(Character.transform.forward, endPos.transform.position - Character.transform.position) >= 1 && percentage >= 90f)
            {
                Character.transform.rotation = Quaternion.Slerp(Character.transform.rotation, lookOnLook, elapsedTime * 0.5f);
                elapsedTime += Time.deltaTime;
            }
            else
            {
                Character.transform.position = Vector3.Lerp(startPos, endPos.transform.position, fractionOfJourney);
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



    public static void FillInputText(Text[] inputField)
    {
        inputField[0].text = "Welcome to <b>MA.D</b>";
        string madString = "Some may ask what is MA.D?";
        madString += '\n';
        madString += '\n';
        madString += "MA:D is .... MA:D is:";
        madString += '\n';
        madString += '\n';
        madString += '\n';

        int lenth = Random.Range(3, dictonery.Length);
        int j = 0;
        while ( j != lenth-1)
        {
            RandomizeArray(dictonery);
            for (int i = 0; i < dictonery[i].Length - 2; i++)
            {
                int randm2 = (int)Random.Range(1, dictonery[i + 1].Length - 1);
                madString += " <b>MA.D</b> is : ";
                Random.InitState(Random.Range(0, 100000));
                if (randm2 % 2 == 0| randm2 % 9 == 0)
                    for (int x = 0; x < (int)Random.Range(1, 120); x++)
                    {
                        if (randm2 % 13 == 0)
                        {
                            madString += ASCI[0] + ASCI[1];
                            continue;
                        }
                        madString += ASCI[Random.Range(0, ASCI.Length - 1)];
                    }
                else
                    madString += dictonery[i];

                madString += '\n';
            }
            madString += '\n';
            madString += '\n';
            j++;
        }
        madString += " <b>MA.D</b> is :<b>us</b>";
        inputField[1].text = madString;
    }
    public static void FillInputTextRosny(Text[] inputField)
    {
        inputField[0].text = "Bienvenu dans la <b>FanZone</b>";
        inputField[1].text = DescriptionRosny;
    }


    public static void FillInputText(string tmp_key, Text[] inputField)
    {
        if (tmp_key == "Null")
        {
            //FillInputText(inputField);
            FillInputTextRosny(inputField);
            return;
        }
        AssetManager _as = GameObject.FindObjectOfType<AssetManager>();
        if (_as.InfoArtwork.ContainsKey(tmp_key))
        {
            string[] des_art = _as.InfoArtwork[tmp_key].description.Split('§');
            inputField[0].text = FormtCartel(des_art);
            inputField[1].text = FormateDEscription(des_art);
            if(des_art.Length>5)////todo check way the keyDisplay is coming in the description count 
            UxInfo.curentArtisUrl = des_art[5];
        }
        else
            Debug.LogWarning("They key for inputing text in the artist description its missing");
    }

    static void GenerateRectangleOnGUI(string _key, AssetManager astMan)
    {
        string[] des_art;
        string FinalString;
        des_art = astMan.InfoArtwork[_key].description.Split('§');
        FinalString = FormtCartel(des_art);
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.name = FinalString;
        guiStyle.normal.textColor = Color.yellow;
        GUI.Label(new Rect(Input.mousePosition.x - 10, Screen.height - Input.mousePosition.y, 100, 100), "" + FinalString, guiStyle);
    }
    static string FormtCartel(string[] des_art)
    {
        return string.Format(
            "Title : <color=orange><b>{0}</b></color> " +
          "|| Artis : <color=orange><b>{1}</b></color> " +
         "|| Format : <color=orange><b>{2}</b></color> " +
           "|| Date : <color=orange><b>{3}</b></color> ",
                                    des_art[0], des_art[2], des_art[3], des_art[1]);
    }
    static string FormateDEscription(string[] des_art)
    {
        return string.Format(
            "Title : <i>{0}</i>" + '\n' +
            "   Artist : {1}" + '\n'+
            " " + '\n' +
            "Description :" + '\n' +
            "<i>{2}</i>" + '\n' ,
                                    des_art[0], des_art[2], des_art[4]);;
    }
    public static string GetKeyFromRay(Camera tmp_cam)
    {
        string tmp_key=null;
        Ray ray2;
        RaycastHit[] hits;
        ray2 = tmp_cam.ScreenPointToRay(Input.mousePosition);
        hits = Physics.RaycastAll(ray2, Mathf.Infinity);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.gameObject.layer == 5) return null;
            MeshCollider meshCollider = hits[i].collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
            {
                Debug.LogWarning("I did not hit an mesh");
            }
            else
            {
                tmp_key = hits[i].transform.root.name;
                if (astMan.InfoArtwork.ContainsKey(tmp_key))
                    return tmp_key;
                else
                {
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
    static void RandomizeArray( string [] arr)
    {
        for (var i = arr.Length - 1; i > 0; i--)
        {
            var r = Random.Range(0, i);
            var tmp = arr[i];
            arr[i] = arr[r];
            arr[r] = tmp;
        }
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