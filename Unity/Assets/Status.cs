using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    Rigidbody rigidB;
    AudioSource audio;
    public AudioClip audioClipCreation, audioShrink, audioFallow;
    public GameObject master;
    public int nr;
    public bool locked;
    public bool _play;
    public string State = "Neutral";
    public bool oance = true;
    public LayerMask layer1;
    public LayerMask layer2;


    void Start()
    {
        rigidB = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "ground")
        GetComponentInChildren<SoundTrigger>().TriigerGround(collision.relativeVelocity);
    }
    void Update()
    {
        GetComponent<MeshCollider>().enabled = true;
        if (_play )
            State = "Play";
        else if(locked)
            State = "Lock";

        Color _color = Color.white;


        switch (State)
        {
            case "Fallow":
                if (master != null)
                {
                    float dist = Vector3.Distance(master.transform.position - master.transform.forward * nr, transform.position) / 100;
                    GetComponent<MeshCollider>().enabled = false;
                    transform.position = Vector3.MoveTowards(transform.position, (master.transform.position - master.transform.forward * nr), 11 * Time.deltaTime + dist);
                    transform.rotation = master.transform.rotation;
                    _color = Color.yellow;
                    if (oance)
                    {
                        if (!audio.isPlaying)
                        {
                            audio.clip = audioFallow;
                            audio.Play();
                        }
                        oance = false;
                    }
                }
                break;
            case "Lock":

                if (oance)
                {
                    if (!audio.isPlaying)
                    {
                        audio.clip = audioClipCreation;
                        audio.Play();
                    }
                    oance = false;
                }
                this.transform.gameObject.layer = layer2;
                _color = Color.black;
                break;

            case "Unlock":
                if (oance)
                {
                        audio.clip = audioShrink;
                        audio.Play();
                    oance = false;
                }
                _color = Color.white;
                this.transform.gameObject.layer = layer2;
                State = "Neutral";
                break;

            case "Play":
                if (oance)
                {
                    if (!audio.isPlaying)
                    {
                        audio.clip = audioShrink;
                        audio.Play();
                    }
                    oance = false;
                }
                this.transform.gameObject.layer = layer1;
                _color = Color.cyan;
                break;


            default:
                break;
        }

        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", _color);

    }


}
