using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOthers : MonoBehaviour
{
    public Vector3 pastPosition;
    private Animator animations;
    Vector3 target;
    bool moving;
  
    private void Start()
    {
        animations = GetComponent<Animator>();
        target = NetworkPlayerManager.infoPl[this.name].position;

    }

    // Update is called once per frame
    void Update()
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
        if (NetworkPlayerManager.infoPl.ContainsKey(this.name))
        {
            target = NetworkPlayerManager.infoPl[this.name].position;
            float dist = Vector3.Distance(this.transform.position, target);
            float step = dist * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            transform.rotation = Quaternion.Euler(NetworkPlayerManager.infoPl[this.name].rotation);
            moving = true;
        }
    }
}

