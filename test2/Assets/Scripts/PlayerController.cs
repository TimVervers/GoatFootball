using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

//We need the following components to make the player work
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	public float speed = 1;

	private static readonly float InputDelay = 0.5f;

	//The actual player with all the movement properties
	private BoxCollider box;
	private CapsuleCollider capsule;

	//Moving variables
	private Vector3 movement;
	private float turnAmount;
	private float forwardAmount;
	private float movingTurnSpeed = 360;
	private bool removeChargeOnNextFrame;
	private float stationaryTurnSpeed = 360;
	private float switchDelay;
	private Vector3 groundNormal;

	private bool moving;
	private float abilityCooldown;

	private Animation anim;

	void Start()
	{
		anim = GetComponentInChildren<Animation>();
		//Loop all animations
		anim.wrapMode = WrapMode.Loop;

		//Dont loop the animations below
		anim["ramAttack"].wrapMode = WrapMode.Once;
		anim["getHit"].wrapMode = WrapMode.Once;
		anim["death"].wrapMode = WrapMode.Once;
		anim["angry1"].wrapMode = WrapMode.Once;
		anim["angry2"].wrapMode = WrapMode.Once;

		// Put idle and walk into lower layers (The default layer is always 0)
		// This will do two things
		// - Since the others and idle/walk are in different layers they will not affect
		// each other's playback when calling CrossFade.
		// - Since the other is in a higher layer, the animation will replace idle/walk
		// animations when faded in.
		anim["ramAttack"].layer = 1;
		anim["getHit"].layer = 1;
		anim["death"].layer = 1;
		anim["angry1"].layer = 1;
		anim["angry2"].layer = 1;

		// Stop animations that are already playing
		//(In case user forgot to disable play automatically)
		anim.Stop();
	}

	private void FixedUpdate()
	{
		//Get Input controls
		float h = CrossPlatformInputManager.GetAxis("Horizontal");
		float v = CrossPlatformInputManager.GetAxis("Vertical");
		bool submit = CrossPlatformInputManager.GetButton("Submit");

		//Apply movement, jumping and rotation
		movement = new Vector3(h, 0f, v);
		Move(movement);

		if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1)
		{
			//Pressed some keys, lets walk
			anim.CrossFade("walk");
		}
		else
		{
			//Didn't do shit, lets breathe
			anim.CrossFade("idleBreathe");
		}

		if (submit)
		{
			//Smack that bitch
			anim.CrossFade("ramAttack");
		}
	}

	private void Move(Vector3 movement)
	{

		//Get camera position to face walking directory
		movement = Camera.main.transform.TransformDirection(movement);

		//If the magintude goes higher then 1, bring it back to 1
		//Magnitude is the length between the vectors origin and its endpoint
		if (movement.magnitude > 1f) movement.Normalize();

		//Invert direction - http://docs.unity3d.com/ScriptReference/Transform.InverseTransformDirection.html
		movement = transform.InverseTransformDirection(movement);

		//Check if the player is grounded
		IsGrounded();

		//Stick the model to the surface
		movement = Vector3.ProjectOnPlane(movement, groundNormal);

		//Calculate turning amount based on ???
		turnAmount = Mathf.Atan2(movement.x, movement.z);

		forwardAmount = movement.z;

		//Turn the character
		TurnRotation();

		//Translate the current position, based on the movementspeed / time to move the player
		GetComponent<Rigidbody>().MovePosition(transform.position + transform.TransformDirection(movement * Time.deltaTime * speed));
	}

	private void TurnRotation()
	{
		// help the character turn faster (this is in addition to root rotation in the animation)
		float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
		transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
	}

	private void IsGrounded()
	{
		RaycastHit hitInfo;

		// 0.1f is a small offset to start the ray from inside the character
		// it is also good to note that the transform position in the sample assets is at the base of the character
		if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, 0.1f))
		{
			groundNormal = hitInfo.normal;
		}
		else
		{
			groundNormal = Vector3.up;
		}
	}
}
