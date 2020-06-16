using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ChambreAnanke : MonoBehaviour
{


    public AudioSource audioSource2;
    public AudioClip Rumble;
    public Animator Animator;
    public Animator AnimButton;
    public Collider c;

    void Start()
    {
        audioSource2 = GetComponent<AudioSource>();


    }
    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Player")
            audioSource2.PlayOneShot(Rumble, 1);
        Debug.Log("Entrée dans la chambre");
        Animator.SetBool("MessageOn", true);
        AnimButton.SetBool("ButtonOn", true);
    }


    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag == "Player")

            Debug.Log("Sortie de la chambre");
        audioSource2.Stop();
        Animator.SetBool("MessageOn", false);
        AnimButton.SetBool("ButtonOn", false);

    }
}
