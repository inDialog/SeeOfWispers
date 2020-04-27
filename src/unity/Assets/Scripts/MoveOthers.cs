using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOthers : MonoBehaviour
{
    public Vector3 pastPosition;
    Multiplayer multiplayer;
    private Animator animations;

    bool moving;
  
    private void Start()
    {
        multiplayer = FindObjectOfType<Multiplayer>();
        animations = GetComponent<Animator>();

    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 curentP = this.transform.position;
        UpdatePosition();
        pastPosition = this.transform.position;
        float dist = DistanceToGround();
        if (dist < 20 & dist != 0)
        {
            if (moving)
            {
                animations.SetTrigger("Walk");
            }
            else
                animations.SetTrigger("idelGround");

        }
        else
        {

            if (curentP.y < pastPosition.y)
            {
               animations.SetTrigger("takeoff");
            }
            else
            {
               animations.SetTrigger("ForwordUp");
            }
        }
    }
    
    int DistanceToGround()
    {
        RaycastHit hit;
        Ray ray = new Ray(this.transform.position, transform.up * -1);
        if (Physics.Raycast(ray, out hit))
        {
            return (int)(hit.distance * 100);
        }
        else return 0;
    }

    void UpdatePosition()
    {
        if (multiplayer.infoPl.ContainsKey(this.name))
        {
            if (multiplayer.infoPl[this.name].position != transform.position)
            {
                float step = 10f * Time.deltaTime; // calculate distance to move
                transform.position = Vector3.MoveTowards(transform.position, multiplayer.infoPl[this.name].position, step);
                transform.rotation = Quaternion.Euler(multiplayer.infoPl[this.name].rotation);
                moving = true;
            }
            else
                moving = false;

        }
    }
}
