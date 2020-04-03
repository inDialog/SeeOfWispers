using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    AudioSource audio;
    public AudioClip audioFall;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }
    public void TriigerGround( Vector3 force) {
       float  volume = force.x + force.y + force.y;
        if (volume < 0) volume *= -1;
        if (volume == 0) volume =0.1f;
        volume = volume.Remap(0, 10, 0.1f, 1);
        //print(volume);
        audio.enabled = false;
        audio.clip = audioFall;
        audio.volume = volume;
        audio.enabled = true;
    }

 
}