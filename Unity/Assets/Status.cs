using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    Rigidbody rigidB;
    public Color _color;
    public GameObject master;
    public int nr;
    public bool locked;
    public bool _play;

    // Start is called before the first frame update
    void Start()
    {
        rigidB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (master != null)
        {
            float dist = Vector3.Distance(master.transform.position - master.transform.forward * nr, transform.position) / 100;
            GetComponent<MeshCollider>().enabled = false;
            transform.position = Vector3.MoveTowards(transform.position, (master.transform.position - master.transform.forward * nr), 11 * Time.deltaTime + dist);
            transform.rotation = master.transform.rotation;
            _color = Color.yellow;
        }
        else
            GetComponent<MeshCollider>().enabled = true;
        _color = Color.white;

        if (locked)
            _color = Color.black;


        if (_play)
            _color = Color.cyan;

        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", _color);
    }


}
