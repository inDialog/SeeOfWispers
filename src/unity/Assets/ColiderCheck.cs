using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// todo find a better way to change the collor of the material when  is colided
/// </summary>
public class ColiderCheck : MonoBehaviour
{
    public static List<string> isNotConvex = new List<string>();
    /// <summary>
    /// Colider vertex order
    /// </summary>
    static int[,] order = {
            { 0, 1, 3, 2, 0, 3, 2, 1 },
            { 0, 4, 6, 2, 0, 6, 2, 4 },
            { 5, 4, 6, 7, 5, 6, 7, 4 },
            { 5, 1, 3, 7, 5, 3, 7, 1 },
            { 2, 3, 7, 6, 2, 7, 3, 6 },//bottom
            { 0, 1, 5, 4, 0, 5, 1, 4 },//top
    };
    BoxCollider bx;
    Rigidbody rb;
    const uint NUM_VERTICES = 8;
    AssetManager asset;
    Vector3[] POSITION;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        asset = FindObjectOfType<AssetManager>();
        rb = this.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        ExtensionMethods.ConvertConvexObjects(asset.InfoArtwork, out isNotConvex);
        SetMeshTrigger();
        if (ArtistInfo.colderSize != Vector3.zero)
        { 
            bx = this.gameObject.GetComponent<BoxCollider>();
            bx.isTrigger = true;
            ConstrcutCube();
        }


    }

    void SetMeshTrigger()
    {
        if (ExtensionMethods.ConcertToBool(asset.InfoArtwork[ArtistInfo.artistKey].uploadOptions)[5]) return;
        MeshCollider[] ms = transform.GetChild(1).GetComponentsInChildren<MeshCollider>();
        foreach (var item in ms)
        {
            item.convex = true;
            item.isTrigger = true;
        }
    }
    void Rever(string key)
    {
        MeshCollider[] ms = transform.GetChild(1).GetComponentsInChildren<MeshCollider>();
        foreach (var item in ms)
        {
            bool[] options = ExtensionMethods.ConcertToBool(asset.InfoArtwork[key].uploadOptions);
            item.isTrigger = options[5];
            item.convex = options[4];

        }
    }
    private void OnDisable()
    {
        Rever(this.name);
        Destroy(rb);

        ExtensionMethods.RestitureConvertedObjects(isNotConvex);
        isNotConvex.Clear();
        StopAllCoroutines();
    }

    private IEnumerator Check()
    {

        while (true)
        {
            if(ArtistInfo.colderSize != Vector3.zero)
            while (transform.childCount!=3)
            {
                Debug.Log(transform.childCount +"!!!!");
                Debug.LogWarning("NoArt");
                GeneralState.colided = true;
                yield return null;
            }
            else
            {
                while (transform.childCount < 1)
                {
                    Debug.Log(transform.childCount + "!!!!");
                    Debug.LogWarning("NoArt");
                    GeneralState.colided = true;
                    yield return null;
                }
            }

            rb.WakeUp();

            Debug.Log("Started Cehack" + ArtistInfo.colderSize);

            if (ArtistInfo.colderSize != Vector3.zero)
            {
                Debug.Log(transform.childCount + "_|_");

                if (!bx)
                    bx = this.gameObject.AddComponent<BoxCollider>();

                bx.size = ArtistInfo.colderSize;
                bx.isTrigger = true;
            }
            else
            {
                //bx.isTrigger = false;
                GeneralState.colided = SizeCheck();    //<- check if mesh size and center 
                Debug.Log("Size Check :" + GeneralState.colided);
                SetColor(GeneralState.colided);
                break;
            }


            GeneralState.colided = CheckIfMeshIsContatined();  //<-  check if mesh center is contained in box
            SetColor(GeneralState.colided);
            Debug.Log("Colisions Check 1" + GeneralState.colided);
            if (GeneralState.colided) break;

            GeneralState.colided = StartTest1();//<- Liht Grid rayPerimiter to search if any part of the mesh passes the perimeter
            SetColor(GeneralState.colided);
            Debug.Log("Colisions check 2" + GeneralState.colided);
            if (GeneralState.colided) break;

            GeneralState.colided = StartTest2();  //<-  Dense Grid rayPerimiter to search if any part of the mesh passes the perimeter
            SetColor(GeneralState.colided);
            Debug.Log("Colisions check 3" + GeneralState.colided);
            StopAllCoroutines();
            yield return null;
        }
    }

    bool SizeCheck()
    {
        int index = 1;

        GameObject artwork = this.transform.GetChild(index).gameObject;
        Bounds bs = ObjectBounds(this.transform);
        float dist = Vector3.Distance(bs.center, artwork.transform.parent.position);
        Debug.Log(dist);
        bool temp;
        if (bs.size.magnitude > GeneralState.maxColideSize.magnitude || bs.center.y < GeneralState.Y_axisMax || dist > GeneralState.maxDistance)
            temp = true;
        else
            temp = false;
        return temp;
    }
    Bounds ObjectBounds(Transform obj)
    {
        Bounds meshesBounds = new Bounds(obj.position, Vector3.zero);
      
        MeshRenderer[] mrs = obj.GetComponentsInChildren<MeshRenderer>(true);

        for (int i = 0; i < mrs.Length; i++)
        {
            if (i == 0) meshesBounds = mrs[i].bounds;
            else meshesBounds.Encapsulate(mrs[i].bounds);
        }
        return meshesBounds;
    }

    bool CheckIfMeshIsContatined()
    {
        int index = 1;
        MeshCollider[] meshRenderes = transform.GetChild(index).GetComponentsInChildren<MeshCollider>();
        int i = 0;
        bool tmp = false;
        while (i < meshRenderes.Length)
        {
            //Debug.Log(meshRenderes[i].bounds.center.y);
            if (!bx.bounds.Contains(meshRenderes[i].bounds.center))
                return true;
            i++;
        }
        return tmp;
    }
    bool StartTest1()
    {
        //Debug.Log("started");
        POSITION = ConstrcutCube();
        int i = 0;
        while (i < 6)
        {
            int j = 0;
            while (j < NUM_VERTICES - 1)
            {
                bool temp = (ConstructRaycast(POSITION[order[i, j]], POSITION[order[i, j + 1]]));
                //Debug.LogError(i.ToString() + " j:" + j.ToString() + temp);
                if (temp == true) return true;
                j++;

            }

            i++;
        }
        return false;
    }

    bool StartTest2()
    {
        POSITION = ConstrcutCube();
        float dist = Vector3.Distance(POSITION[2], POSITION[6]);
        bool temp;
        int j = 0;
        while (j < dist * 2)
        {
            Vector3 temps = transform.TransformDirection(Vector3.forward).normalized * -j / 2;
            int i = 0;
            while (i < 4)
            {
                Vector3 target = POSITION[order[0, i]];
                Vector3 direction = POSITION[order[0, i + 1]];
                temp = (ConstructRaycast(temps + target, temps + direction));
                if (temp)
                    return true;
                i++;
            }
            j++;
        }
        dist = Vector3.Distance(POSITION[2], POSITION[0]);
        j = 0;
        while (j < dist * 4)
        {
            Vector3 temps = transform.TransformDirection(Vector3.up).normalized * -j / 4;
            int i = 0;
            while (i < 4)
            {
                Vector3 target = POSITION[order[5, i]];
                Vector3 direction = POSITION[order[5, i + 1]];
                temp = (ConstructRaycast(temps + target, temps + direction));
                if (temp)
                    return true;
                i++;
            }
            j++;
        }
        return false;

    }

    Vector3[] ConstrcutCube()
    {
        Vector3[] _POSITION = new Vector3[NUM_VERTICES];
        Vector3 colliderCentre = bx.center;
        Vector3 colliderExtents = bx.extents + bx.extents / 1000;
        for (int i = 0; i != NUM_VERTICES; ++i)
        {
            Vector3 extents = colliderExtents;

            extents.Scale(new Vector3((i & 1) == 0 ? 1 : -1, (i & 2) == 0 ? 1 : -1, (i & 4) == 0 ? 1 : -1));

            Vector3 vertexPosLocal = colliderCentre + extents;
            Vector3 vertexPosGlobal = bx.transform.TransformPoint(vertexPosLocal);
            _POSITION[i] = vertexPosGlobal;
        }
        return _POSITION;
    }
    bool ConstructRaycast(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;


        Ray ra = new Ray(origin, direction - origin);
        float dist = Vector3.Distance(origin, direction);
        if (Physics.Raycast(ra, out hit, dist))
        {

            if (hit.transform.tag == "Base")
            {
                Debug.DrawLine(origin, hit.point, Color.green);
                Debug.Log("OutsideTheBunds");
                GeneralState.colided = true;
                return true;
            }
            else
            {
                //Debug.Log("HitSomething" + hit.GetType());
                GeneralState.colided = false;
                return false;
            }

        }
        else
        {
            //Debug.Log("AllClear");
            Debug.DrawRay(origin, direction - origin, Color.magenta);
            GeneralState.colided = false;
            return false;
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.GetType() == typeof(MeshCollider) & other.transform.root.name != ArtistInfo.artistKey & other.tag != "Player")
        {
            GeneralState.colided = false;
            SetColor(GeneralState.colided);
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.GetType() == typeof(MeshCollider) & other.transform.root.name != ArtistInfo.artistKey & other.tag != "Player")
        {
            GeneralState.colided = true;
            SetColor(GeneralState.colided);
        }

    }

    void SetColor(bool state  )
    {
     
            
        MeshRenderer []  ms = transform.GetChild(1).GetComponentsInChildren<MeshRenderer>();
            foreach (var item in ms)
            {
            if (!state)
                item.material.color = Color.white;

            else
                item.material.color = Color.red;

        }
    }
}
