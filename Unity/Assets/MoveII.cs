using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveII : MonoBehaviour
{
	public float speed = 10.0f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 2.0f;
	private bool grounded = false;
	public Rigidbody _rigidbody;

	public List<GameObject> pastSelcted = new List<GameObject>();

	public int numberOfFallowers;

	void Awake()
	{
		_rigidbody.freezeRotation = true;
	}
    private void Update()
    {
        
		transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * 2, 0));
	}
	void FixedUpdate()
	{
		Vector3 targetVelocity = new Vector3(0, 0, Input.GetAxis("Vertical"));
		targetVelocity = transform.TransformDirection(targetVelocity);
		targetVelocity *= speed;

		// Apply a force that attempts to reach our target velocity
		Vector3 velocity = _rigidbody.velocity;
		Vector3 velocityChange = (targetVelocity - velocity);
		velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
		velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
		velocityChange.y = 0;
		_rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

		// Jump or fall
		if ( Input.GetButton("Jump"))
			_rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
		else if(grounded)
			GetComponent<Rigidbody>().AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));
		grounded = false;
		_rigidbody.isKinematic = false; 

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
