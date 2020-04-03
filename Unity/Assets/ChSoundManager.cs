using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChSoundManager : MonoBehaviour
{
    AudioSource audio;
   public  List<AudioClip> touchingSound;
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }
    public void ActivateSound(Vector3 force,int _index)
    {
        float volume = force.x + force.y + force.y;
        if (volume < 0) volume *= -1;
        if (volume == 0) volume = 0.1f;
        volume = volume.Remap(0, 20, 0.1f, 1);
        //print(volume);
        audio.enabled = false;
        audio.clip = touchingSound[_index];
        audio.volume = volume;
        audio.enabled = true;
    }
}
