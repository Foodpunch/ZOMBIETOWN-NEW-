using UnityEngine;
using System.Collections;

public class Evade : MonoBehaviour {
	public Transform target;
	private float moveSpeed;
	private float rotationSpeed;
	private int safeDistance;
	private int iterationAhead;
	private Vector3 targetSpeed;
	private Vector3 targetFuturePosition;
	private Vector3 direction;
	private Vector3 moveVector;
	
	void Awake() {
		target = GameObject.FindWithTag("Player").transform;
	}
	
	void Start() {
		moveSpeed = 5.0f;
		rotationSpeed = 5.0f;
		safeDistance = 5;
		iterationAhead = 30;
	}
	
	void Update() {
		EvadeBehavior();
	}
	
	void EvadeBehavior() {
		targetSpeed = target.gameObject.GetComponent<CharacterLogic>().instantVelocity;
		targetFuturePosition = target.position + (targetSpeed * iterationAhead);
		direction = transform.position - targetFuturePosition;
		direction.y = 0;
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
		if(direction.magnitude < safeDistance) {
			moveVector = direction.normalized * moveSpeed * Time.deltaTime;
			transform.position += moveVector;
		}
	}
}