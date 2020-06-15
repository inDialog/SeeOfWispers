using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCharacter : MonoBehaviour
{
    private Animator animations;
    public GameObject sphere;
    public GameObject parent;
    Collider body;
    Rigidbody sRigidBody;
    Vector3 pastVelocity;
   public  bool groud;
    // Start is called before the first frame update
    void Start()
    {
        animations = GetComponent<Animator>();
        sRigidBody = sphere.GetComponent<Rigidbody>();
        parent = this.transform.parent.gameObject;
        body = parent.GetComponent<SphereCollider>();

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Rotating" + sRigidBody.rotation);


        if (sRigidBody.velocity.x + sRigidBody.velocity.y + sRigidBody.velocity.z != 0)
        {
     
            //Debug.Log("Moving" + sRigidBody.velocity);
            Quaternion targetRotation = Quaternion.Euler(
                sRigidBody.velocity.y.Remap(-8, 6, 120, -30),
                parent.transform.parent.eulerAngles.y,
                parent.transform.parent.eulerAngles.z
            );
            if (groud)
            {
                animations.SetTrigger("walk");

            }
            else
            {
                if (sRigidBody.velocity.y - pastVelocity.y >= 0.01 | (sRigidBody.velocity.y >= 0.001))
                {
                    if (sRigidBody.velocity.x * sRigidBody.velocity.z < 0.5f)
                    {
                        animations.SetTrigger("takeOff");
                    }
                    else
                    {
                        animations.SetTrigger("forowrdUP");

                    }
                    parent.transform.rotation = Quaternion.Slerp(parent.transform.rotation, targetRotation, 10 * Time.deltaTime);

                }
                //}
                else if (sRigidBody.velocity.y - pastVelocity.y < 0.01)
                {

                    animations.SetTrigger("fall");
                    parent.transform.rotation = Quaternion.Slerp(parent.transform.rotation, targetRotation, 10 * Time.deltaTime);

                    //if (sRigidBody.velocity.x  + sRigidBody.velocity.z != 0)
                    //    parent.transform.Rotate(new Vector3(0, sRigidBody.velocity.y * Time.deltaTime * 50, 0));
                    //animations.SetTrigger("forowrdUP");
                }
                //animations.SetBool("idleFly", true);

            }
        }
        pastVelocity = sRigidBody.velocity;
        

        //}
        //else
        //{
        //    if (Input.GetButton("Jump"))

    }
    private void OnTriggerEnter(Collider other)
    {
        if(sRigidBody.velocity.y<0.1f)
        groud = true;
        else
            groud = false;

        Debug.Log(groud);
    }
    private void OnCollisionExit(Collision collision)
    {
        //groud = false;
    }
    //animations.SetTrigger("fall");
    //animations.SetBool("idleFly", false);
    //animations.SetTrigger("takeOff");
    //animations.SetTrigger("forowrdUP");
}

//}

