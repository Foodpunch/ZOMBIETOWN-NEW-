using UnityEngine;
using System.Collections;

public class ObstacleAvoidance : MonoBehaviour {
	private Transform target;
	float speed = 5.0f;
	
	void Awake() {
		target = GameObject.FindWithTag("Player").transform; // Target which AI seek
	}
	
	void Update () {
		// directionectional vector to target
		Vector3 direction = (target.position - transform.position).normalized;
		RaycastHit hit;
		// Check for forward raycast
		if(Physics.Raycast(transform.position, transform.forward, out hit, 4)) {
			if(hit.transform != transform) { // Intersection with own collider is omitted
				Debug.DrawLine(transform.position, hit.point, Color.red);
				direction += hit.normal * 50;
			}
		}
		
		var leftRay = transform.position;
		var rightRay = transform.position;
		leftRay.x -= 1;
		rightRay.x += 1;
		
		if(Physics.Raycast(leftRay, transform.forward, out hit, 4)) {
			if(hit.transform != transform) { // Intersection with own collider is omitted
				Debug.DrawLine(leftRay, hit.point, Color.green);
				direction += hit.normal * 50;
			}
		}
		if(Physics.Raycast(rightRay, transform.forward, out hit, 4)) {
			if(hit.transform != transform) { // Intersection with own collider is omitted
				Debug.DrawLine(rightRay, hit.point, Color.blue);
				direction += hit.normal * 50;
			}
		}
		var rotate = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * 2);
		transform.position += transform.forward * speed * Time.deltaTime;
	}
	
	public void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Vector3 ray = transform.TransformDirection(Vector3.forward) * 4;
		Gizmos.DrawRay (transform.position + new Vector3(0, 0.5f, 0), ray);
		Gizmos.color = Color.green;
		Gizmos.DrawRay (transform.position + new Vector3(-1, 0.5f, 0), ray);
		Gizmos.color = Color.blue;
		Gizmos.DrawRay (transform.position + new Vector3(1, 0.5f, 0), ray);
	}
}