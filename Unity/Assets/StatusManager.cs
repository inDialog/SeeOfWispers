using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public List<GameObject> selected = new List<GameObject>();
    public Mesh tower;
    public Mesh box;
    public MeshRenderer meshRenderer;
    ChSoundManager chSound;
    public  bool ready;
    public bool active;
    int count;
    private void Start()
    {
             chSound = GetComponentInChildren<ChSoundManager>();

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
            ///////reset object
            foreach (var item in selected)
            {
                item.GetComponent<Status>().State = "Neutral";
                item.GetComponent<Status>().master = null;
                item.GetComponent<Status>().locked = false;
                item.GetComponent<Status>()._play = false;
                item.GetComponent<Status>().oance = true;
                item.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
                item.GetComponent<Rigidbody>().isKinematic = false;
                item.GetComponent<MeshCollider>().enabled = true;


            }
            selected.Clear();
            ready = false;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
                ready = true;
        }
        if (Input.GetKey(KeyCode.L))
            ready = false;

        if (ready)
            meshRenderer.material.SetColor("_EmissionColor", Color.red);
        else
            meshRenderer.material.SetColor("_EmissionColor", Color.white);

    }
    public void SmallBox(GameObject @object)
    {
        @object.GetComponent<MeshFilter>().mesh = box;
        @object.GetComponent<MeshCollider>().sharedMesh = box;
        @object.GetComponent<Rigidbody>().freezeRotation = false;
        @object.GetComponent<Rigidbody>().mass = 1;

        @object.GetComponent<Status>().State = "Unlock";
        @object.GetComponent<Status>().locked = false;
        @object.GetComponent<Status>()._play = false;
        @object.GetComponent<Status>().oance = true;
    }
    public void Tower(GameObject @object)
    {
        @object.GetComponent<MeshFilter>().mesh = tower;
        @object.GetComponent<MeshCollider>().sharedMesh = tower;
        @object.GetComponent<Rigidbody>().freezeRotation = true;
        @object.GetComponent<Rigidbody>().mass = 1000;
        @object.transform.rotation = new Quaternion(0, 0, 0, 1);

        @object.GetComponent<Status>().State = "Lock";
        @object.GetComponent<Status>().oance = true;
        @object.GetComponent<Status>().locked = true;
        @object.GetComponent<Status>()._play = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ST")
        {
            string state = collision.gameObject.GetComponent<Status>().State;
            if (state == "Neutral")
                chSound.ActivateSound(collision.relativeVelocity, 0);
            if (state == "Lock")
                chSound.ActivateSound(collision.relativeVelocity, 2);
        }
        else if (collision.gameObject.tag == "ground")
            chSound.ActivateSound(collision.relativeVelocity, 1);


    }
    private void OnCollisionExit(Collision collision)
    {
        if (targetHit() & collision.gameObject.tag == "ST")
        {
            count = 0;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "ST")
        {
            Status status = collision.gameObject.GetComponent<Status>();
            ////////pull object
            if (Input.GetKey(KeyCode.L))
            {
                if (!status.locked)
                {
                    status.State = "Fallow";
                    status.oance = true;
                    if (!selected.Contains(collision.gameObject))
                        selected.Add(collision.gameObject);
                    status.master = this.transform.gameObject;
                    status.nr = selected.Count;
                    collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
            ////////change object
            if (ready & selected.Count == 0)
            {
                if (!status.locked)
                {
                    Tower(collision.gameObject);
                }
                else
                    SmallBox(collision.gameObject);
                ready = false;
            }
            //////Activate Media
            if (targetHit())
            {
                if (count < 6)
                    count++;
                if (status.locked & count == 5)
                    status._play = !status._play;
            }
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
}