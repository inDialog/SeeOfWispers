using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fallow : MonoBehaviour
{
    public GameObject master;
    public bool moveble;
    public Color _color;
    public Rigidbody rg;
    bool onTarget;
    bool palayClip;

    // Start is called before the first frame update
    void Start()
    {
        _color = Color.black;
    }

    public bool OnTarget
    {
        set
        {
            onTarget = value;
        }
    }

    public bool Play
    {
        set
        {
            palayClip = value;
        }
        get
        {
            return palayClip;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (palayClip)
            _color = Color.cyan;
        else
            _color = Color.black;


        if (onTarget)
            _color = Color.red;


        if (master != null)
        {

            transform.position = Vector3.MoveTowards(transform.position, master.transform.position - master.transform.forward, 11 * Time.deltaTime);
            transform.rotation = master.transform.rotation;
            _color = Color.yellow;
            GetComponent<BoxCollider>().enabled = false;
            rg.isKinematic = true;

        }

        if (Input.GetKeyUp(KeyCode.L))
        {
            if (master != null)
            {
                rg.velocity = master.GetComponent<Rigidbody>().velocity;
                GetComponent<BoxCollider>().enabled = true;
                rg.isKinematic = false;


            }
            master = null;
        }

        GetComponent<MeshRenderer>().material.color = _color;

    }

    public GameObject SetMaster
    {
        set
        {
            if (moveble)
                master = value;
            else
                master = null;
        }
    }



    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            moveble = true;
            palayClip = !palayClip;
        }
        else
        {
            moveble = false;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        moveble = false;
    }

}