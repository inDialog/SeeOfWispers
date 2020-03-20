using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacter : MonoBehaviour
{
	public float speed = 10.0f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 2.0f;
	private bool grounded = false;
	public Rigidbody _rigidbody;
	public LineRenderer ln;
	int color = 1;

	public List < GameObject> pastSelcted = new List<GameObject> ();
	


	void Awake()
	{
		_rigidbody.freezeRotation = true;
		//_rigidbody.useGravity = false;
	}

	void FixedUpdate()
	{
		//if (grounded)
		//{
		// Calculate how fast we should be moving
		//Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		Vector3 targetVelocity = new Vector3(0, 0, Input.GetAxis("Vertical"));

		targetVelocity = transform.TransformDirection(targetVelocity);
		targetVelocity *= speed;

		// Apply a force that attempts to reach our target velocity
		Vector3 velocity = _rigidbody.velocity;
		Vector3 velocityChange = (targetVelocity - velocity);
		velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
		velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
		velocityChange.y = 0;
		transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * 2, 0));
		_rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

		// Jump
		if (canJump && Input.GetButton("Jump"))
		{
			_rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
		}
		else GetComponent<Rigidbody>().AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));



		grounded = false;

		if (Input.GetKey(KeyCode.L))
		{
			float dist;
			Vector3 targetColision;
			GameObject selcted = targetHit(out dist,out targetColision);

			if (selcted != null & dist != 0)
			{
                ln.positionCount = 2;
                ln.SetPosition(0, this.transform.position);
                ln.SetPosition(1, targetColision);
				ln.material.SetColor("_EmissionColor", Color.red);


				selcted.transform.GetComponent<Fallow>().SetMaster = this.transform.gameObject;
				selcted.transform.GetComponent<Fallow>().OnTarget = true;

				if (!pastSelcted.Contains(selcted))
				{
					pastSelcted.Add(selcted);
					pastSelcted[0].transform.GetComponent<Fallow>().OnTarget = false;
					pastSelcted[0].transform.GetComponent<Fallow>().Play = false;

					pastSelcted.Clear();
					pastSelcted.Add(selcted);
				}
			}
            else
            {
				ln.positionCount = 2;
				ln.material.SetColor("_EmissionColor", Color.black);

				ln.SetPosition(0, this.transform.position);
                ln.SetPosition(1, transform.position - transform.forward * -1);
            }
			
		}
        else
        {
            ln.positionCount = 0;
			if (pastSelcted.Count > 0)
			{
				pastSelcted[0].transform.GetComponent<Fallow>().OnTarget = false;
				pastSelcted.Clear();
			}
		}




	}
		GameObject targetHit(out float distance, out  Vector3 _hit)
		{

			RaycastHit hit;
			Ray ray = new Ray(this.transform.position, transform.forward);
		if (Physics.Raycast(ray, out hit))
		{
			distance = hit.distance;
			_hit = hit.point;
			return hit.transform.gameObject;
		}
		else
		{
			distance = 0;
			_hit = Vector3.zero;
			return null;
		}
	}

		void OnCollisionStay()
		{
			grounded = true;
		}

		float CalculateJumpVerticalSpeed()
		{
			return Mathf.Sqrt(jumpHeight * gravity);
		}
	}
