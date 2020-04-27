using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneManger : MonoBehaviour
{
    public float length = 2.0F;
    private float startTime,factor;
    private bool up,chord1;
    public Reaktion.TurbulentMotion[] motions;
    public AudioSource pad1, pad2;
    // Start is called before the first frame update
    void Start()
    {
       
        up = false;
        chord1 = true;
        factor = 1f;
        InvokeRepeating("turnDown",1f, 10f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(factor < 1f) { 
        float dist = (Time.time - startTime);
        factor = dist / length;
           // float t = Mathf.SmoothStep(0.0f, length, factor);
            if (up)
            {
               transform.position= Vector3.Lerp(new Vector3(0f, 0f, 0f), new Vector3(0f, 3f, 0f), factor);
                  

                for (int i = 0; i < motions.Length; i++)
                {
                    motions[i].displacement = Vector3.Lerp(new Vector3(0f, 0f, 0f), new Vector3(0.1f, 1.5f, 1f), factor);
                    motions[i].rotation = Vector3.Lerp(new Vector3(0f, 0f, 0f), new Vector3(60f, 60f, 180f), factor);

                    //   motions[i].coeffRotation = Mathf.Lerp(0f, 1.3f, factor);
                    //  motions[i].coeffDisplacement = Mathf.Lerp(0f, 1.3f, factor);
                }
            }
            else
            {
                transform.position = Vector3.Lerp(new Vector3(0f, 3f, 0f), new Vector3(0f, 0f, 0f), factor);
                for (int i = 0; i < motions.Length; i++)
                {
                    motions[i].displacement = Vector3.Lerp(new Vector3(0.1f, 1.5f, 1f), new Vector3(0f, 0f, 0f), factor);
                    motions[i].rotation = Vector3.Lerp(new Vector3(60f, 60f, 180f), new Vector3(0f, 0f, 0f), factor);
                    //  motions[i].coeffRotation = Mathf.Lerp(1.3f, 0f, factor);
                    //   motions[i].coeffDisplacement = Mathf.Lerp(1.3f, 0f, factor);
                }
            }
        }
    }

    public void turnDown()
    {
        factor = 0f;
        startTime = Time.time;
        up = !up;
        if (!up)
            return;
        if (chord1)
            pad1.Play();
        else
            pad2.Play();

        chord1 = !chord1;
    }
}
