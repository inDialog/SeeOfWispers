using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColiderCheck : MonoBehaviour
{
    int[,] order = {
            { 0, 1, 3, 2, 0, 3, 2, 1 },
            { 0, 4, 6, 2, 0, 6, 2, 4 },
            { 5, 4, 6, 7, 5, 6, 7, 4 },
            { 5, 1, 3, 7, 5, 3, 7, 1 },
            { 2, 3, 7, 6, 2, 7, 3, 6 },//bottom
            { 0, 1, 5, 4, 0, 5, 1, 4 },//top
    };
    BoxCollider bx;
    Rigidbody rs;
    const uint NUM_VERTICES = 8;
    public GameObject myArt;
    // Start is called before the first frame update
    private void OnEnable()
    {
        bx = GetComponent<BoxCollider>();
        Check();

        //rs = gameObject.AddComponent<Rigidbody>();
        //rs.isKinematic = true;
        //bx.size = ArtistInfo.colderSize;
        bx.isTrigger = true;
        ConstrcutCube();
   
        //StartCoroutine("Check");
    }

    private void OnDisable()
    {
        //bx.isTrigger = true;
        Destroy(rs);
        GameObject[] gm = GameObject.FindGameObjectsWithTag("Base");
        foreach (var item in gm)
        {
            if (item.GetComponent<MeshCollider>())
                item.GetComponent<MeshCollider>().convex = false;
        }
    }

    private void Update()
    {
        //StartTest();
    }

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
///todo check if all mesh objects are in the box 
        int i = 0;
        MeshCollider[] meshRenderes = transform.GetChild(1).GetComponentsInChildren<MeshCollider>();

        bool temp = true;
        while (i < meshRenderes.Length)
        {
            if (bx.bounds.Contains(meshRenderes[i].bounds.center))
            {
                Debug.Log("Bounds contain the point : " + meshRenderes[i].bounds);
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
        for (int j = 0; j < dist*2; j += 1)
        {
            Vector3 temps = transform.TransformDirection(Vector3.forward).normalized * -j/2;
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
        for (int j = 0; j < dist*4; j += 1)
        {
            Vector3 temps = transform.TransformDirection(Vector3.up).normalized * -j/4;
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
        bx.size = ArtistInfo.colderSize;
        Vector3[] POSITION = new Vector3[NUM_VERTICES];
        Vector3 colliderCentre = bx.center;
        Vector3 colliderExtents = bx.extents;
        Debug.Log(bx.extents * 2);
        for (int i = 0; i != NUM_VERTICES; ++i)
        {
            Vector3 extents = colliderExtents;

            extents.Scale(new Vector3((i & 1) == 0 ? 1 : -1, (i & 2) == 0 ? 1 : -1, (i & 4) == 0 ? 1 : -1));

            Vector3 vertexPosLocal = colliderCentre + extents;
            Vector3 vertexPosGlobal = bx.transform.TransformPoint(vertexPosLocal);
            POSITION[i] = vertexPosGlobal;
        }
        Debug.Log("oance");
        return POSITION;
    }
    bool ConstructRaycast(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;
        Ray ra = new Ray(origin,  direction- origin);
        Debug.DrawRay(origin, direction - origin, Color.red);
        float dist = Vector3.Distance(origin, direction); 
        if (Physics.Raycast(ra, out hit, dist))
        {
            if (hit.transform.name != this.transform.name & hit.transform.tag != "Untagged"& hit.transform.tag != "Player")
            {
                Debug.DrawLine(origin, hit.point, Color.green);
                GeneralState.colided = true;
                return true;
            }
            else
                return false;
                
        }
        else
        {
            GeneralState.colided = false;
            return false;
        }

    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log(collision.gameObject.name);
    //}
    //private void OnCollisionStay(Collision collision)
    //{
    //    Debug.Log(collision.gameObject.name);
    //}
    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log(other.gameObject.name);

    //}
    //private void OnTriggerStay(Collider other)
    //{
    //    Debug.Log(other.gameObject.name);

    //}
}
