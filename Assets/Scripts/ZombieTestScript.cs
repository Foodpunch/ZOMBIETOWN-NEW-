using UnityEngine;
using System.Collections;

public class ZombieTestScript : MonoBehaviour {

	Transform target;
	Vector3 direction;
	Quaternion rotate;
	Vector3 leftRayDirection;
	Vector3 rightRayDirection;
	RaycastHit hitLeft;
	RaycastHit hitRight;
	Vector3 hitNormal;
	public float speed = 5f;
	public float steerForce;
	public float rotateSpeed;
	public float translateSpeed;
	public float minimumDistanceToAvoid;
	// Use this for initialization
	void Start () {
		target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		direction = (target.position - transform.position).normalized; //current direction
		
		leftRayDirection = transform.TransformDirection(new Vector3(-1, 0, 1)); //left feeler
		rightRayDirection = transform.TransformDirection(new Vector3(1, 0, 1)); //right feeler
		
		RaycastHit hit;		//middle ray hit info
		// Check for forward raycast
		if (Physics.Raycast(transform.position, transform.forward, out hit, 4))
		{
			if (hit.transform != transform)
			{ // Intersection with own collider is omitted
				Debug.DrawLine(transform.position, hit.point, Color.red);
				direction += hit.normal * 50;
			}
		}
		if (Physics.Raycast(transform.position, leftRayDirection, out hitLeft, minimumDistanceToAvoid))
		{
			if (hitLeft.transform != transform)
			{ // Intersection with own collider is omitted
				MoveLeft();
			}
		}

		if (Physics.Raycast(transform.position, rightRayDirection, out hitRight, minimumDistanceToAvoid))
		{
			if (hitRight.transform != transform)
			{ // Intersection with own collider is omitted
				MoveRight();
			}
		}
		rotate = Quaternion.LookRotation(direction.normalized);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * rotateSpeed);
		transform.position += transform.forward * Time.deltaTime * translateSpeed;
	
	}

	void MoveLeft()
	{
		Vector3 leftHitNormal = hitLeft.normal;
		Debug.DrawRay(hitLeft.point, leftHitNormal, Color.red);
		leftHitNormal.y = 0.0f; // Restrict movement in y direction
		direction = transform.forward + leftHitNormal * steerForce;
			
	}
	void MoveRight()
	{
		Vector3 rightHitNormal = hitRight.normal;
		Debug.DrawRay(hitRight.point, rightHitNormal, Color.blue);
		rightHitNormal.y = 0.0f; // Restrict movement in y direction
		direction = transform.forward + rightHitNormal * steerForce;		
	}
}
