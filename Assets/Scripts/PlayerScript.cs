using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class PlayerScript : MonoBehaviour,IDamagable<float> {

	
	[SerializeField] private GameObject animGameObj;
	Animator _anim;				//player's anim
	Rigidbody _rigidBody;		//player's rigidbody
	int floorMask;				//layermask
	CharacterController _charCont;

	
	float playerHealth = 100;
	Slider playerHealthSlider;


	// Use this for initialization
	void Start () {
		floorMask = LayerMask.GetMask("Floor"); //gets the layer mask for the ground, maybe change it to int
		_anim = animGameObj.GetComponent<Animator>();
		_rigidBody = GetComponent<Rigidbody>();
		_charCont = GetComponent<CharacterController>();
		playerHealthSlider = GameObject.FindGameObjectWithTag("PlayerHP").GetComponent<Slider>();
		playerHealthSlider.maxValue = playerHealth;
	}
	
	// Update is called once per frame
	void Update () {
		playerHealthSlider.value = playerHealth;
		Animate();
	}
	
	public void Damage(float dmg)
	{
		playerHealth -= dmg;
	}
	void Animate()
	{
		//float yRot = gameObject.transform.eulerAngles.y;
		//float xRot = gameObject.transform.eulerAngles.x;

		_anim.SetFloat("Speed_f", _charCont.velocity.magnitude);
		//_anim.SetFloat("Body_Horizontal_f",(yRot-360)/360);
		//_anim.SetFloat("Body_Vertical_f", );
		
	}
}
