using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallowII : MonoBehaviour
{
    public GameObject master;
    public bool moveble;
    public Color _color;
    public Rigidbody rg;
    bool onTarget;
    bool playClip;
    bool locked;


    int nr;
    // Start is called before the first frame update
    void Start()
    {
        _color = Color.black;
    }


    // Update is called once per frame
    void Update()
    {
        GetComponent<MeshRenderer>().material.color = _color;
        _color = Color.black;
        rg.mass = 1;
        Vector3 scaleSize = Vector3.one;
        if (locked)
        {
            if (playClip)
                _color = Color.cyan;
            else
                _color = Color.green;

            rg.velocity = Vector3.zero;
            rg.angularVelocity = Vector3.zero;
            rg.mass = 100;
            scaleSize = new Vector3(1, 10, 1);
            transform.position = new Vector3(transform.position.x, 5, transform.position.z);
            transform.localScale = scaleSize;
            if (transform.rotation != new Quaternion(0, 0, 0, 1))
                transform.rotation = new Quaternion(0, 0, 0, 1);
            return;
        }
        transform.localScale = scaleSize;

        if (master != null)
        {
            //transform.Translate((master.transform.position - transform.position) * Time.deltaTime * 11);
            float dist = Vector3.Distance(master.transform.position - master.transform.forward * nr, transform.position) / 100;
            _color = Color.yellow;
            GetComponent<BoxCollider>().enabled = false;
            rg.isKinematic = true;
            transform.position = Vector3.MoveTowards(transform.position, (master.transform.position - master.transform.forward * nr), 11 * Time.deltaTime + dist);
            transform.rotation = master.transform.rotation;
        }

        if (Input.GetKeyUp(KeyCode.L) || Input.GetMouseButtonDown(0))
        {
            if (master != null)
            {
                rg.velocity = master.GetComponent<Rigidbody>().velocity;
                GetComponent<BoxCollider>().enabled = true;
                rg.isKinematic = false;
                master.gameObject.GetComponent<MoveII>().numberOfFallowers = 0;
            }
            master = null;
        }


    }



    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            playClip = !playClip;

            if((Input.GetKey(KeyCode.K) || Input.GetMouseButtonDown(0))& targetHit())
            {
                locked = !locked;
            }
            if (locked) return;
            if (Input.GetKey(KeyCode.L) || Input.GetMouseButtonDown(0))
            {
                master = collision.gameObject;
                collision.gameObject.GetComponent<MoveII>().numberOfFallowers++;
                nr = collision.gameObject.GetComponent<MoveII>().numberOfFallowers;
            }
        }

    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            moveble = false;
        }
    }

    bool targetHit()
    {

        RaycastHit hit;
        Ray ray = new Ray(this.transform.position, transform.up);
        if (Physics.Raycast(ray, out hit))
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}