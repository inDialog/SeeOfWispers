using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AnankeBehaviour : MonoBehaviour {

    public AudioSource audioSource;
    public AudioClip Sleep;
    public AudioClip Awake;
    public bool IsAwake = false;
    public Animator animator;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();

	}




	// Update is called once per frame
	void AnankeAwake() {
        animator.SetBool("Awake", true);
        audioSource.PlayOneShot(Awake, 1);
	}

    void OnTriggerEnter(Collider c ){
        if (c.gameObject.tag == "Player")
            IsAwake = true;
        AnankeAwake();

        Debug.Log("Player On Ananke zone");
    }
    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag == "Player")
            IsAwake = false;
        animator.SetBool("Awake", false);
        audioSource.Stop();
       

        Debug.Log("Ananke se calme");

    }
}
