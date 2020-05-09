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
    static BoxCollider bx;
    static Rigidbody rb;
    static bool hadColider = true;
    const uint NUM_VERTICES = 8;
    static List<string> isNotConvex = new List<string>();

    // Start is called before the first frame update
    private void OnEnable()
    {
        if (GetComponent<BoxCollider>())
            bx = GetComponent<BoxCollider>();
        else
        {
            hadColider = false;
            bx = this.gameObject.AddComponent<BoxCollider>();
            bx.size = GeneralState.maxColideSize;
            rb = this.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
         
        }

        AssetManager asset = FindObjectOfType<AssetManager>();
        ExtensionMethods.ConvertConvexObjects(asset.infoArwork, out isNotConvex);
        ConstrcutCube();
        Check();
        SetMeshTrigger();
    }

    void SetMeshTrigger()
    {
        MeshCollider[] ms = transform.GetChild(1).GetComponentsInChildren<MeshCollider>();
        foreach (var item in ms)
        {
            item.isTrigger = !item.isTrigger;
        }
    }
    private void OnDisable()
    {
        SetMeshTrigger();
        Destroy(rb);
        if (!hadColider)
        {
            Destroy(bx);
        }
        StartCoroutine("Disable");
    }
    IEnumerable Disable()
    {
        while (true)
        {
            ExtensionMethods.RestitureConvertedObjects(isNotConvex);
            yield return new WaitForSeconds(3f);
         

            isNotConvex.Clear();
            break;
        }
    }

    //private void Update()
    //{
    //    //StartTest();
    //}

    private IEnumerator Check()
    {
        while (true)
        {
            GeneralState.colided = CheckIfMeshIsContatined();
            if (CheckIfMeshIsContatined()) break;

            GeneralState.colided = StartTest();
            if (StartTest()) break;
            yield return null;
        }
    }

    bool CheckIfMeshIsContatined()
    {
        MeshCollider[] meshRenderes = transform.GetChild(1).GetComponentsInChildren<MeshCollider>();
        bool temp = true;
        int i = 0;
        while (i < meshRenderes.Length)
        {
            if (bx.bounds.Contains(meshRenderes[i].bounds.center))
            {
                //Debug.Log("Bounds contain the point : " + meshRenderes[i].bounds);
                temp = false;
            }
            i++;
        }
        return temp;
    }
    bool StartTest()
    {
        //Debug.Log("started");
        Vector3[] POSITION = ConstrcutCube();
        bool temp = true;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < NUM_VERTICES - 1; j++)
            {
                temp = (ConstructRaycast(POSITION[order[i, j]], POSITION[order[i, j + 1]]));
                if (temp == true) return temp;

            }
            if (temp == false & i == 5)
                return Test2(POSITION);
        }
        return temp;
    }

    bool Test2(Vector3[] POSITION)
    {
        float dist = Vector3.Distance(POSITION[2], POSITION[6]);
        bool temp;
        for (int j = 0; j < dist * 2; j += 1)
        {
            Vector3 temps = transform.TransformDirection(Vector3.forward).normalized * -j / 2;
            for (int i = 0; i < 4; i++)
            {
                Vector3 target = POSITION[order[0, i]];
                Vector3 direction = POSITION[order[0, i + 1]];
                temp = (ConstructRaycast(temps + target, temps + direction));
                if (temp)
                    return true;
            }
        }
        dist = Vector3.Distance(POSITION[2], POSITION[0]);
        for (int j = 0; j < dist * 4; j += 1)
        {
            Vector3 temps = transform.TransformDirection(Vector3.up).normalized * -j / 4;
            for (int i = 0; i < 4; i++)
            {
                Vector3 target = POSITION[order[5, i]];
                Vector3 direction = POSITION[order[5, i + 1]];
                temp = (ConstructRaycast(temps + target, temps + direction));
                if (temp)
                    return true;
            }
        }
        //Debug.Log("ended");
        return true;

    }

    Vector3[] ConstrcutCube()
    {
        if (ArtistInfo.colderSize != Vector3.zero)
            bx.size = ArtistInfo.colderSize;
        Vector3[] POSITION = new Vector3[NUM_VERTICES];
        Vector3 colliderCentre = bx.center;
        Vector3 colliderExtents = bx.extents;
        for (int i = 0; i != NUM_VERTICES; ++i)
        {
            Vector3 extents = colliderExtents;

            extents.Scale(new Vector3((i & 1) == 0 ? 1 : -1, (i & 2) == 0 ? 1 : -1, (i & 4) == 0 ? 1 : -1));

            Vector3 vertexPosLocal = colliderCentre + extents;
            Vector3 vertexPosGlobal = bx.transform.TransformPoint(vertexPosLocal);
            POSITION[i] = vertexPosGlobal;
        }
        Debug.Log("Checking For Colisions");
        return POSITION;
    }
    bool ConstructRaycast(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;
        Ray ra = new Ray(origin, direction - origin);
        Debug.DrawRay(origin, direction - origin, Color.red);
        float dist = Vector3.Distance(origin, direction);
        if (Physics.Raycast(ra, out hit, dist))
        {
            if (hit.transform.tag == "Base" & ArtistInfo.colderSize == Vector3.zero)
            {
                GeneralState.colided = false;
                return false;
            }

            if (hit.transform.name != this.transform.name & hit.transform.tag != "Untagged" & hit.transform.tag != "Player")
            {
                Debug.DrawLine(origin, hit.point, Color.green);
                GeneralState.colided = true;
                return true;
            }
            else
            {
                GeneralState.colided = false;
                return false;
            }

        }
        else
        {
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
