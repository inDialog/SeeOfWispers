﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public List<GameObject> selected = new List<GameObject>();
    public Mesh tower;
    public Mesh box;
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyUp(KeyCode.K))
        {
            foreach (var item in selected)
            {
                item.GetComponent<Status>().master = null;
                item.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
                item.GetComponent<Rigidbody>().isKinematic = false;

            }
            selected.Clear();
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        if (selected.Count == 0)
            if (Input.GetKey(KeyCode.K) & collision.gameObject.tag == "ST")
            {
                if (!collision.gameObject.GetComponent<Status>().locked)
                {
                    Status st = collision.gameObject.GetComponent<Status>();
                    if (!selected.Contains(collision.gameObject))
                        selected.Add(collision.gameObject);
                    st.master = this.transform.gameObject;
                    st.nr = selected.Count;
                    collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                }
                else
                {
                    collision.gameObject.GetComponent<MeshFilter>().mesh = box;
                    collision.gameObject.GetComponent<MeshCollider>().sharedMesh = box;
                    collision.gameObject.GetComponent<Rigidbody>().freezeRotation = false;
                    collision.gameObject.GetComponent<Rigidbody>().mass = 1;
                    collision.gameObject.GetComponent<Status>().locked = false;
                    collision.gameObject.GetComponent<Status>()._play = false;

                }

            }

        if (Input.GetKey(KeyCode.L) & collision.gameObject.tag == "ST")
        {
            collision.gameObject.GetComponent<MeshFilter>().mesh = tower;
            collision.gameObject.GetComponent<MeshCollider>().sharedMesh = tower;
            collision.gameObject.GetComponent<Rigidbody>().freezeRotation = true;
            collision.gameObject.GetComponent<Rigidbody>().mass = 1000;
            collision.transform.rotation = new Quaternion(0, 0, 0, 1);
            collision.gameObject.GetComponent<Status>().locked = true;
            collision.gameObject.GetComponent<Status>()._play = false;

        }



    }

    bool targetHit()
    {
        RaycastHit hit;
        Ray ray = new Ray(this.transform.position, transform.up * -1);
        if (Physics.Raycast(ray, out hit))
            if (hit.collider.tag == "ST")
                return true;
            else
                return false;
        else
            return false;
    } 

    private void OnCollisionEnter (Collision collision)
    {
        if (targetHit() & collision.gameObject.tag == "ST")
        {
            if(collision.gameObject.GetComponent<Status>().locked)
                collision.gameObject.GetComponent<Status>()._play=!collision.gameObject.GetComponent<Status>()._play;
        }
        //if (targetHit() & collision.gameObject.tag == "ST")
        //{
        //    collision.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        //}
    }
}