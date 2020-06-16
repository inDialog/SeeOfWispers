using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnColision : MonoBehaviour
{
    SphereCollider sphereC;
    AudioSource audioS;
    Renderer _renderer;
    public ParticleSystem Prefab;
    public ParticleSystem _PS;

    // Start is called before the first frame update
    void Start()
    {
        audioS = GetComponent<AudioSource>();
        sphereC = gameObject.AddComponent<SphereCollider>();
        _renderer = GetComponentInChildren<MeshRenderer>();
        if(Prefab)
        {
            _PS = Instantiate(Prefab, this.transform);
            _PS.gameObject.SetActive(false);

        }
        sphereC.radius = audioS.maxDistance/1.2f;
        sphereC.isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.tag == "Player")
        {
            float dist = Vector3.Distance(other.transform.position, this.transform.position);
            if (!audioS.isPlaying)
            {
                audioS.Play();
                audioS.loop = true;
            }
            if (_renderer)
            {
                dist = dist.Remap(0, audioS.maxDistance, 10, 0.1f);
                _renderer.material.SetColor("_EmissionColor", Color.white*dist);
                if (_PS)
                {
                    _PS.gameObject.SetActive(true);

                    //_PS.e();

                    _PS.transform.LookAt(other.transform.position);

                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if(_PS)
                _PS.gameObject.SetActive(false);




            audioS.loop = false;
        }
    }
}
