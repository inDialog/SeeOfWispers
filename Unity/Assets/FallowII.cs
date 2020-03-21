using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallowII : MonoBehaviour
{
    public GameObject master;
    Vector3 origin;
    public bool moveble;
    public Color _color;
    public Rigidbody rg;
    bool playClip;
  public   bool locked;
   public bool singleAction = true;
    int nr;
    // Start is called before the first frame update
    void Start()
    {
        _color = Color.black;
        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", _color);

    }


    // Update is called once per frame
    void Update()
    {
        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", _color);

        _color = Color.white;
        rg.mass = 1;
        Vector3 scaleSize = Vector3.one;
        if (locked)
        {
            if (playClip)
            {
                _color = Color.cyan;
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(0).localScale = new Vector3(1, 0.1f, 1);
            }
            else
            {
                _color = Color.black;
                transform.GetChild(0).gameObject.SetActive(false);
            }
            LockBox(scaleSize);
            return;
        }
        else
        {
            playClip = false;
            transform.GetChild(0).gameObject.SetActive(false);
        }
        if (master != null)
        {
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
        
        transform.localScale = scaleSize;
    }

    void LockBox(Vector3 scaleSize)
    {
        transform.position = new Vector3(origin.x, origin.y + 5, origin.z);
        rg.velocity = Vector3.zero;
        rg.angularVelocity = Vector3.zero;
        rg.mass = 100;
        scaleSize = new Vector3(1, 10, 1);
        transform.localScale = scaleSize;
        if (transform.rotation != new Quaternion(0, 0, 0, 1))
            transform.rotation = new Quaternion(0, 0, 0, 1);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
            origin = collision.collider.ClosestPoint(transform.position);

        if (collision.gameObject.tag == "Player")
        {
            if (targetHit() & locked)
            {
                playClip = !playClip;
                return;
            }
            if ((Input.GetKey(KeyCode.K) || Input.GetMouseButtonDown(0)))
            {
                locked = !locked;
            }

           
            if (Input.GetKey(KeyCode.L) || Input.GetMouseButtonDown(0))
            {
                master = collision.gameObject;
                collision.gameObject.GetComponent<MoveII>().numberOfFallowers++;
                nr = collision.gameObject.GetComponent<MoveII>().numberOfFallowers;
            }


        }
    }
 
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
            origin = collision.collider.ClosestPoint(transform.position);
    }
    bool targetHit()
    {
        RaycastHit hit;
        Ray ray = new Ray(this.transform.position, transform.up);
        if (Physics.Raycast(ray, out hit))
            return true;
        else
            return false;
    }

}