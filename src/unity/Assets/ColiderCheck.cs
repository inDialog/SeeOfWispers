using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColiderCheck : MonoBehaviour
{
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
    bool hadColider = true;
    const uint NUM_VERTICES = 8;
    public static List<string> isNotConvex = new List<string>();
    AssetManager asset;
    Vector3[] POSITION;
    // Start is called before the first frame update
    private void OnEnable()
    {
        asset = FindObjectOfType<AssetManager>();
        rb = this.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        if (ArtistInfo.colderSize == Vector3.zero)
        {
            bx = this.gameObject.AddComponent<BoxCollider>();
            hadColider = false;
            bx.isTrigger = false;

        }
        else
        {
            bx = this.gameObject.GetComponent<BoxCollider>();
            bx.isTrigger = true;

        }
        ExtensionMethods.ConvertConvexObjects(asset.infoArwork, out isNotConvex);
        SetMeshTrigger();
        ConstrcutCube();
    }

    void SetMeshTrigger()
    {
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
            bool[] options = ExtensionMethods.ConcertToBool(asset.infoArwork[key].uploadOptions);
            item.isTrigger = false; //todo set it as option
            item.convex = options[4];
        }
    }
    private void OnDisable()
    {
        bx.isTrigger = true;
        Rever(this.name);
        Destroy(rb);
        if (!hadColider)
        {
            Destroy(bx);
        }
        ExtensionMethods.RestitureConvertedObjects(isNotConvex);
        isNotConvex.Clear();
        StopAllCoroutines();
    }

    private IEnumerator Check()
    {
        while (true)
        {

            Debug.Log("Started Cehack" + ArtistInfo.colderSize);

            if (ArtistInfo.colderSize != Vector3.zero)
            {
                bx.size = ArtistInfo.colderSize;
                bx.isTrigger = true;

            }
            else
            {
                bx.isTrigger = false;
                bx.size = GeneralState.maxColideSize;
            }


            GeneralState.colided = CheckIfMeshIsContatined();
            Debug.Log("Colisions Check 1" + GeneralState.colided);
            if (GeneralState.colided) break;

            GeneralState.colided = StartTest1();
            Debug.Log("Colisions check 2" + GeneralState.colided);
            if (GeneralState.colided) break;

            GeneralState.colided = StartTest2();
            Debug.Log("Colisions check 3" + GeneralState.colided);
            StopAllCoroutines();
            yield return null;
        }
    }

    bool CheckIfMeshIsContatined()
    {
        MeshCollider[] meshRenderes = transform.GetChild(1).GetComponentsInChildren<MeshCollider>();
        int i = 0;
        while (i < meshRenderes.Length)
        {
            if (bx.bounds.Contains(meshRenderes[i].bounds.center))
            {
                //Debug.Log("Bounds contain the point : " + meshRenderes[i].bounds);
                return false;
            }
            i++;
        }
        return true;
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
                //Debug.DrawRay(origin, direction - origin, Color.red);
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
        if (other.GetType() == typeof(MeshCollider) & other.transform.root.name != ArtistInfo.artistKey)
        {
            GeneralState.colided = false;
            Debug.Log(other.gameObject.name);

            MeshRenderer[] ms = transform.GetChild(1).GetComponentsInChildren<MeshRenderer>();
            foreach (var item in ms)
            {
                item.material.color = Color.white; ////todo save material to put back in original state
            }

        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.GetType() == typeof(MeshCollider) & other.transform.root.name != ArtistInfo.artistKey)
        {
            GeneralState.colided = true;
            Debug.Log(other.gameObject.name);
            MeshRenderer[] ms = transform.GetChild(1).GetComponentsInChildren<MeshRenderer>();
            foreach (var item in ms)
            {
                item.material.color = Color.red;
            }

        }

    }
}
