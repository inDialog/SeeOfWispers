using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnColision : MonoBehaviour
{

    public ParticleSystem Prefab;
    public AudioClip[] AudioFiles;

    ParticleSystem _PS;
    SphereCollider sphereC;
    AudioSource audioS;
    Renderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        audioS = GetComponent<AudioSource>();
        sphereC = gameObject.AddComponent<SphereCollider>();
        _renderer = GetComponentInChildren<MeshRenderer>();
        if (Prefab)
        {
            _PS = Instantiate(Prefab, this.transform);
            _PS.gameObject.SetActive(false);

        }
        sphereC.radius = audioS.maxDistance / 1.2f;
        sphereC.isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.tag == "Player")
        {
            float dist = Vector3.Distance(other.transform.position, this.transform.position);
            if (!audioS.isPlaying)
            {
                //audioS.loop = true;
                if (AudioFiles.Length != 0)
                {
                    RandomizePlay();
                }
                audioS.Play();
            }
            if (_renderer)
            {
                dist = dist.Remap(0, audioS.maxDistance, 10, 0.1f);
                _renderer.material.SetColor("_EmissionColor", Color.white * dist);
            }
            if (_PS)
            {
                _PS.gameObject.SetActive(true);
                _PS.transform.LookAt(other.transform.position);
            }

        }
    }
    void RandomizePlay()
    {
        Random.seed = Random.Range(12, 100000);
        int index = Random.Range(0, AudioFiles.Length );
        Debug.Log(index);
        audioS.clip = AudioFiles[index];
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (_PS)
            {
                _PS.gameObject.SetActive(false);
                //audioS.loop = false;
            }


        }
    }
}
