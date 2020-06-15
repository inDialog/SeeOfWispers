using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniMainCharacter : MonoBehaviour
{
    public GameObject sphere;
    private Animator animations;
    bool ground;
    Rigidbody sRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        sRigidBody = sphere.GetComponent<Rigidbody>();
        animations = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (sRigidBody.velocity != Vector3.zero)
        {
            Transform parent = sphere.transform.GetComponentInParent<Transform>();
            Quaternion targetRotation = Quaternion.Euler(
                 sRigidBody.velocity.y.Remap(-8, 6, 100, -60),
                  parent.eulerAngles.y + Input.GetAxis("Horizontal") * 20,
                 parent.eulerAngles.z + Input.GetAxis("Horizontal") * -40
             );
            transform.parent.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);

            if (ground)
            {
                if (!Input.GetButton("Jump"))
                {
                    animations.SetBool("walk", true);
                }
                if (Input.GetAxis("Vertical") != 0 | Input.GetAxis("Horizontal") != 0)
                    animations.SetBool("run", true);
                else
                    animations.SetBool("run", false);

            }
            else
            {
                animations.SetBool("walk", false);
                if (Input.GetButton("Jump"))
                {
                    animations.speed = 1.2f;
                    if (Input.GetAxis("Vertical") != 0)
                        animations.SetTrigger("forowrdUP");
                    else
                        animations.SetTrigger("takeOff");
                }
                else if (sRigidBody.velocity.y < 0)
                {
                    animations.speed = 1f;

                    if (Input.GetAxis("Vertical") != 0)
                        animations.SetTrigger("forowrdUP");
                    else
                        animations.SetTrigger("fall");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ground")
            ground = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ground")
            ground = false;
    }
}

