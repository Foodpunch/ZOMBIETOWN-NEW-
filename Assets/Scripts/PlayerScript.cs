using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class PlayerScript : MonoBehaviour,IDamagable {

	
	[SerializeField] private GameObject animGameObj;
	Animator _anim;				//player's anim
//	Rigidbody _rigidBody;		//player's rigidbody
	//int floorMask;				//layermask
	CharacterController _charCont;

    int playerHitCount = 0;

    [SerializeField]CanvasGroup _hurtUI;
    float _alpha;
    [SerializeField] Image BloodUI;


    float healthTimer;
    bool healthRegen;
    float bloodAlpha;
    bool hit;
    bool timerStart;

    bool deathSound;
    [SerializeField] AudioClip deathSong;
    [SerializeField] AudioClip hurtSound;
    [SerializeField] AudioSource PlayerAudioSource;
    [SerializeField] Text deathText1;
    [SerializeField] Text deathText2;
    [SerializeField] GameManager _gameManager;

    bool restart = false;




	// Use this for initialization
	void Start () {
	//	floorMask = LayerMask.GetMask("Floor"); //gets the layer mask for the ground, maybe change it to int
		_anim = animGameObj.GetComponent<Animator>();
	//	_rigidBody = GetComponent<Rigidbody>();
		_charCont = GetComponent<CharacterController>();
        BloodUI = BloodUI.GetComponent<Image>();
        PlayerAudioSource = GetComponent<AudioSource>();
        deathText1 = deathText1.GetComponent<Text>();
        deathText2 = deathText2.GetComponent<Text>();
        _gameManager = _gameManager.GetComponent<GameManager>();
        deathText1.text = "";
        deathText2.text = deathText1.text;


    }
	
	// Update is called once per frame
	void Update () {
        //		playerHealthSlider.value = playerHealth;
        
		Animate();
        _hurtUI.alpha = _alpha;
        switch(playerHitCount)
        {
            case 0:
                BloodUI.color = new Color(BloodUI.color.r, BloodUI.color.g, BloodUI.color.b, 0f);
                break;
            case 1:
                //bloodAlpha = 100f/ 255f;
                BloodUI.color = new Color(BloodUI.color.r, BloodUI.color.g, BloodUI.color.b, 100f/255f);
                break;
            case 2:
                BloodUI.color = new Color(BloodUI.color.r, BloodUI.color.g, BloodUI.color.b, 200f/255f);
                break;
            case 3:
                BloodUI.color = new Color(BloodUI.color.r, BloodUI.color.g, BloodUI.color.b, 1);                
                break;
            default:
                BloodUI.color = new Color(BloodUI.color.r, BloodUI.color.g, BloodUI.color.b, 1);
                timerStart = false;
                healthTimer = 0;
                deathText1.text = "YOU DIED\n" + _gameManager.ZombieNumber.ToString() + " Kills";
                deathText2.text = deathText1.text;
                if(GamepadManager.AnyButtonPressed() || OVRGamepadController.GPC_GetButtonDown((int)OVRGamepadController.Button.A)) //Restart Game button
                {
                    if(!restart)
                    {
                        restart = true;
                        Invoke("RestartGame", 3f);
                    }
                }
                //player dies here
                break;

        }
        HealthSystem();

    }
	void HealthSystem()
    {
        if (timerStart)
        {
            healthTimer += Time.deltaTime;
            BloodUI.color = new Color(BloodUI.color.r, BloodUI.color.g, BloodUI.color.b, Mathf.Lerp(BloodUI.color.a, 0, healthTimer / 5f));
            if (hit)
            {
                healthTimer = 0;
                hit = false;
            }
        }
        if (hit && !timerStart)
        {
            timerStart = true;
            hit = false;
        }
        if (healthTimer > 5f)
        {
            hit = false;
            timerStart = false;
            healthTimer = 0;
            BloodUI.color = new Color(BloodUI.color.r, BloodUI.color.g, BloodUI.color.b, 0);
            playerHitCount = 0;
        }
        if(deathSound)
        {
            //death stuff here
            AudioManager.CrossFade(deathSong, 1f);
            deathSound = false;
            gameObject.GetComponent<CharacterController>().enabled = false;

        }
    }
	public void Damage(HitInfo dmg)
	{
        //healthRegen = true;
        hit = true;
        playerHitCount++;
        if(!PlayerAudioSource.isPlaying)
        {
            PlayerAudioSource.clip = hurtSound;
            Debug.Log(PlayerAudioSource.clip);
            PlayerAudioSource.Play();
          //  AudioManager.PlaySFX(hurtSound.clip, AudioManager.SFXType.NORMAL, 0);
        }
        StartCoroutine("HitCoroutine");
        if(playerHitCount > 3)
        {
            deathSound = true;
        }
       // Utilities.instance.ImageFunction(HurtUI, Utilities.ImageEffect.FADEINFADEOUT);
     //   Utilities.instance.ImageFunction(BloodUI, Utilities.ImageEffect.FLASHTOFADE);

	}
	void Animate()
	{
		//float yRot = gameObject.transform.eulerAngles.y;
		//float xRot = gameObject.transform.eulerAngles.x;

		_anim.SetFloat("Speed_f", _charCont.velocity.magnitude);
		//_anim.SetFloat("Body_Horizontal_f",(yRot-360)/360);
		//_anim.SetFloat("Body_Vertical_f", );
		
	}

    IEnumerator HitCoroutine()
    {
        _alpha = 1;
        yield return new WaitForSeconds(0.1f);
        for (float j = 1; j > 0; j -= Time.deltaTime)
        {
            _alpha = j;
            yield return null;
        }
        _alpha = 0;
        yield return new WaitForSeconds(2f);
    }
    void RestartGame()
    {
        Application.LoadLevel(Application.loadedLevel);
        CancelInvoke("RestartGame");
    }
}
