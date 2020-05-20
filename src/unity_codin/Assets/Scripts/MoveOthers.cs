using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOthers : MonoBehaviour
{
    private bool ChangePos;
    public Vector3 pastPosition;
    Multiplayer multiplayer;
    private Animator animations;
    private List<Vector3> posToMove;

    bool moving;
  
    private void Start()
    {
        multiplayer = FindObjectOfType<Multiplayer>();
        animations = GetComponent<Animator>();
        posToMove = new List<Vector3>();
        ChangePos = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 curentP = this.transform.position;
        UpdatePositionInList();
        UpdateOtherPostion();
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

    void UpdatePositionInList()
    {
        if (multiplayer.infoPl.ContainsKey(this.name))
        {
            if (posToMove.Count == 0)
            {
                posToMove.Add(multiplayer.infoPl[this.name].position);
                ChangePos = true;
            }
            else if (Vector3.Distance(multiplayer.infoPl[this.name].position, posToMove[posToMove.Count - 1]) != 0)
            {
                //transform.position = Vector3.Lerp(transform.position, multiplayer.infoPl[this.name].position, Vector3.Distance(multiplayer.infoPl[this.name].position, transform.position) * Time.deltaTime);
                posToMove.Add(multiplayer.infoPl[this.name].position);

                //if (Vector3.Distance(multiplayer.infoPl[this.name].position, transform.position) > 5f)
                //{
                //    transform.position = multiplayer.infoPl[this.name].position;
                //    transform.rotation = Quaternion.Euler(multiplayer.infoPl[this.name].rotation);
                //}
                //{
                //    Debug.Log("This is the postion of the character now ======" + transform.position
                //        + "\n This is the postion on the server ==========" + multiplayer.infoPl[this.name].position);
                //    float step = 10f * Time.deltaTime; // calculate distance to move
                //    transform.position = Vector3.MoveTowards(transform.position, multiplayer.infoPl[this.name].position, step);
                //    transform.rotation = Quaternion.Euler(multiplayer.infoPl[this.name].rotation);
                //    moving = true;
                //}
            }
            else if (Vector3.Distance(multiplayer.infoPl[this.name].position, transform.position) <= 0.01f)
            {
                posToMove.Clear();
            }
            else
                moving = false;

        }
    }

    void UpdateOtherPostion()
    {
        Debug.Log(posToMove.Count);

        Vector3 startPos = Vector3.zero;
        Vector3 endPos = Vector3.zero;
        Vector3 lastPos = Vector3.zero;
        float distanceBetweenpoints = 0.0f;
        float distanceBetweenEndLast = 0.0f;
        if (posToMove.Count == 1)
        {
            startPos = transform.position;
            endPos = posToMove[0];
            lastPos = endPos;
            distanceBetweenpoints = Vector3.Distance(startPos, endPos);
            distanceBetweenEndLast = Vector3.Distance(endPos, lastPos) * 10;
        }
        else if (posToMove.Count > 1 && ChangePos == true)
        {
            startPos = posToMove[0];
            endPos = posToMove[1];
            lastPos = posToMove[posToMove.Count - 1];
            distanceBetweenpoints = Vector3.Distance(startPos, endPos);
            distanceBetweenEndLast = Vector3.Distance(endPos, lastPos) * 10;
        }
        if (distanceBetweenpoints >= 0.01)
        {
            Debug.LogWarning("I am moving the character");
            if (distanceBetweenpoints == 0)
            {
                distanceBetweenpoints = 1;
            }
            Debug.Log("This is my distance between star and end ====" + distanceBetweenpoints +
                      "\n This is the distance between char and endPos ======" + distanceBetweenEndLast);
            transform.position = Vector3.Lerp(transform.position, endPos, (distanceBetweenpoints + distanceBetweenEndLast) * Time.deltaTime * posToMove.Count);
            if (Vector3.Distance(transform.position, endPos) <= 0.01)
            {
                Debug.LogWarning("I have reached the end");
                posToMove.RemoveAt(0);
            }
        }
    }
}
