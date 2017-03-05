using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class WeaponBase : MonoBehaviour {

	//Base Gun Variables
	protected float damage;
	protected int totalAmmo;  //total ammo available for gun
	protected int maxAmmo = 0; //a.k.a clip size, 0 for infinite? 
	protected int currentAmmo; //the current ammo count 
	protected float fireDelay = 0.1f; //delay for shooting
	protected float reloadDelay = 0.2f; //delay for reloading
	protected bool automatic = false; //Semi or auto

	[SerializeField]  protected Transform shootPoint; //point where the bullet flies out

	protected float _bulletForce; //Force of bullet (500 - weak||  2000 - Mid || 5000 - Strong) Probably should randomise it/calculate it based on range

	public string primaryFire = "Fire1"; //button to shoot
	public string reloadButt = "Reload"; //button to reload

	bool readyToFire = true;
	bool readyToReload = false;
	bool isReloading = false;
	public float fireTime;			//time elapsed while firing

	[SerializeField]Text AmmoCounter;				//Ammo text
	[SerializeField]CanvasGroup ReloadCanvas;		//Reload Text

	//Sound stuff
	protected int soundIndex;
	[SerializeField] protected AudioClip[] fireSounds;
	[SerializeField] protected AudioClip[] reloadSounds;
	protected AudioSource[] _audioSources;
    protected enum SoundType
    {
        FIRESOUND,
        RELOADSOUND
    };
    protected SoundType soundToPlay = SoundType.FIRESOUND;
    bool hasAmmoClicked;

    //Line renderer stuff
    LineRenderer bulletLine;			//Line renderer for bullet shots
	Vector3 endPoint;					//End point for the ray
	float lineSpeed = 6f;
	float lineTime;
	bool bLine;
	
	//MUZZLE FLASH
	[SerializeField] ParticleSystem MuzzleFlash;
	bool bMuzzleFlash;

	//Player Animator
	[SerializeField]protected Animator _playerAnim;


	//Abstract Functions
	
	protected abstract void PrimaryFire();
	// Use this for initialization
	protected virtual void Start () {
        //	AmmoCounter = GameObject.FindGameObjectWithTag("AmmoCounter").GetComponent<Text>();
        AmmoCounter = AmmoCounter.GetComponent<Text>();
        ReloadCanvas = ReloadCanvas.GetComponent<CanvasGroup>();
        ReloadCanvas.alpha = 0;
		bulletLine = GetComponent<LineRenderer>();
		_audioSources = GetComponentsInChildren<AudioSource>();
		_playerAnim = _playerAnim.GetComponent<Animator>();
		soundIndex = 0;
//		MuzzleFlash.enabled = false;
		

//		MuzzleFlare.Stop();
	}
	
	// Update is called once per frame
	void Update () {
		CheckInput();
		UpdateAmmoCounter();
	}
	protected virtual void CheckInput()
	{
        bool buttonPress = false;
        if (automatic)
        {
            buttonPress = Input.GetButton(primaryFire);
        }
        else
        {
            buttonPress = Input.GetButtonDown(primaryFire);
        }

        //if (buttonPress)
        //{
        //    fireTime += Time.deltaTime; //feels like it needs to be edited.
        //    if (readyToFire && (maxAmmo == 0 || currentAmmo > 0 && !isReloading)) //If MAXammo is 0, infinite Ammo. Or if you have ammo to shoot.
        //    {
        //        CancelInvoke("ReloadBullet"); // stops you from trying to reload when firing
        //        PrimaryFire();
        //        //PlaySound(soundIndex, SoundType.FIRESOUND); //plays fire sound
        //        _playerAnim.SetBool("Shoot_b", true);
        //        _playerAnim.SetTrigger("Shoot_t");
        //      //  MuzzleFlash.enabled = true;
        //        readyToFire = false;
        //        currentAmmo--;
        //        readyToReload = true;
        //        bLine = true;
        //        Invoke("SetReadyToFire", fireDelay); //timing between each bullet firing
        //    }

            //float inputAxis = GamepadManager.triggerR;


            //if (automatic)
            //{
            //          if(inputAxis > 0 || OVRGamepadController.GPC_GetAxis((int)OVRGamepadController.Axis.RightTrigger) > 0)
            //          {
            //              buttonPress = true;
            //          }
            //          else
            //          {
            //              buttonPress = false;
            //          }
            //}
            //else
            //{
            //          bool axisInUse = false;
            //          if(inputAxis != 0 || OVRGamepadController.GPC_GetAxis((int)OVRGamepadController.Axis.RightTrigger) != 0)
            //          {
            //              if(!axisInUse)
            //              {
            //                  buttonPress = true;
            //                  axisInUse = true;
            //              }
            //          }
            //          if( inputAxis == 0 || OVRGamepadController.GPC_GetAxis((int)OVRGamepadController.Axis.RightTrigger) == 0)
            //          {
            //              axisInUse = false;
            //          }

        //}

        /*
         * bool buttonPress;
		if (automatic)
		{
			buttonPress = Input.GetButton(primaryFire);
		}
		else
		{
			buttonPress = Input.GetButtonDown(primaryFire);
		}
		
		if (buttonPress)
		{
			fireTime += Time.deltaTime; //feels like it needs to be edited.
			if (readyToFire && (maxAmmo ==  0 || currentAmmo >0 &&  !isReloading)) //If MAXammo is 0, infinite Ammo. Or if you have ammo to shoot.
			{
				CancelInvoke("ReloadBullet"); // stops you from trying to reload when firing
				PrimaryFire();
				PlaySound(soundIndex, SoundType.FIRESOUND); //plays fire sound
				_playerAnim.SetBool("Shoot_b", true);
				_playerAnim.SetTrigger("Shoot_t");
				MuzzleFlash.enabled = true;
				readyToFire = false;
				currentAmmo--;
				readyToReload = true;
				bLine = true; 
				Invoke("SetReadyToFire", fireDelay); //timing between each bullet firing
			}
         */

        if (buttonPress)
		{
			fireTime += Time.deltaTime; //feels like it needs to be edited.
            
			if (readyToFire && (maxAmmo ==  0 || currentAmmo >0 &&  !isReloading)) //If MAXammo is 0, infinite Ammo. Or if you have ammo to shoot.
			{
				CancelInvoke("ReloadBullet"); // stops you from trying to reload when firing
				PrimaryFire();
                //PlaySound(soundIndex, SoundType.FIRESOUND); //plays fire sound
                AudioManager.PlaySFX(fireSounds[SoundRandomiser(fireSounds.Length)],AudioManager.SFXType.ONESHOT);
                _playerAnim.SetBool("Shoot_b", true);
				_playerAnim.SetTrigger("Shoot_t");
                MuzzleFlash.Play();
				readyToFire = false;
				currentAmmo--;
				readyToReload = true;
				bLine = true; 
				Invoke("SetReadyToFire", fireDelay); //timing between each bullet firing
			}
            if(currentAmmo ==0 && !hasAmmoClicked)
            {
                AudioManager.PlaySFX(reloadSounds[2], AudioManager.SFXType.NORMAL);
                hasAmmoClicked = true;
            }
		}
		else
		{
			fireTime = 0.0f;
            hasAmmoClicked = false;
		}
		if (Input.GetButtonDown(reloadButt) || GamepadManager.buttonBDown || OVRGamepadController.GPC_GetButtonDown((int)OVRGamepadController.Button.B)) //when you press button to reload
		{
			fireTime = 0.0f;
            if(automatic && readyToReload && currentAmmo < maxAmmo) //if it's automatic, it should click first to play sound of "unloading" clip
            {
                AudioManager.PlaySFX(reloadSounds[1], AudioManager.SFXType.ONESHOT);
            }
			if (readyToReload && currentAmmo < maxAmmo) //if gun is ready to reload and your ammo is not maxed
			{
                //	PlaySound(soundIndex, SoundType.RELOADSOUND,reloadDelay); //plays reload sound
                AudioManager.PlaySFX(reloadSounds[0], AudioManager.SFXType.DELAYED,reloadDelay);
                _playerAnim.SetBool("Reload_b", true);
				readyToReload = false;
				readyToFire = false;
				InvokeRepeating("ReloadBullet", 0f, reloadDelay); //calls reload function, either reloads entire clip or individual shells
			}
		}
        if (GamepadManager.stickLDown && GamepadManager.stickRDown || (OVRGamepadController.GPC_GetButtonDown((int)OVRGamepadController.Button.RStick) && OVRGamepadController.GPC_GetButtonDown((int)OVRGamepadController.Button.LStick)))
        {
            CheatCode();
        }
	}
    protected virtual void CheatCode()
    {

    }
    protected virtual void SecondaryFire()
    {

    }
	void UpdateAmmoCounter() //simple function to show the ammo count
	{
        AmmoCounter.text = "<color=#73F57C>" + currentAmmo.ToString() + "</color>/" + totalAmmo.ToString();
        float tempFloat = (currentAmmo*1.0f / maxAmmo*1.0f);
     
       
        if (tempFloat <= 0.5f)
        {
            AmmoCounter.text = "<color=#F5D773>" + currentAmmo.ToString() + "</color>/" + totalAmmo.ToString(); 
        }
        if(tempFloat <=0.25f)
        {
            AmmoCounter.text = "<color=#F57373>" + currentAmmo.ToString() + "</color>/" + totalAmmo.ToString();
        }


        if (isReloading)
		{
            //ReloadText.text = "Reloading...";
            ReloadCanvas.alpha = Mathf.PingPong(Time.time*1.5f, 1.0f-0.3f)+0.3f;
		}
		else
		{
            //ReloadText.text = "";
            ReloadCanvas.alpha = 0;
		}
	}


	void SetReadyToFire() //allows player to fire again
	{
		readyToFire = true;
		_playerAnim.SetBool("Shoot_b", false);
	//	MuzzleFlash.enabled = false;
	}
	void ReloadBullet()
	{
		if (currentAmmo == maxAmmo  || totalAmmo == 0) //if clip is full or no ammo left, stop reload
		{
            if(automatic)
            {
                AllowFiring();
            }
            else
            {
                Invoke("AllowFiring", reloadDelay);
            }
			_playerAnim.SetBool("Reload_b", false);
            hasAmmoClicked = false;
			CancelInvoke("ReloadBullet");
		}
		else
		{
			readyToFire = false;	//don't shoot, you're reloading
			isReloading = true;
			if (automatic)
			{
				int tempAmmo = maxAmmo-currentAmmo; //remainder Ammo to fill
				if (tempAmmo <= totalAmmo) //if remainder is more than total, add some ammo
				{
					currentAmmo += tempAmmo;
					totalAmmo -= tempAmmo;
				}
				else if (tempAmmo > totalAmmo) //if you don't have enough ammo
				{
					currentAmmo += totalAmmo; //take all the ammo that's left
					totalAmmo = 0;
					readyToReload = false;
				}
				
			}
			if (!automatic) //shotgun reload logic
			{
                //	PlaySound(soundIndex,SoundType.RELOADSOUND); //plays reload sound
                AudioManager.PlaySFX(reloadSounds[0],AudioManager.SFXType.DELAYED);
				currentAmmo++;
				totalAmmo--;
                Invoke("AllowFiring",reloadDelay); //makes it so that you don't have to spam 'R' to reload everything
                                                   //isReloading = false; 
            }
		}
	}
    void AllowFiring() //I put it in a function so I can invoke and not allow the player to spam Reload+shoot for rapid firing with the shotgun
    {
        isReloading = false;    //you're not reloading anymore
        readyToFire = true;		//let it fire again
        CancelInvoke("AllowFiring");
    }
	protected void SpawnParticles(HitInfo hitInfo)//Spawns particles on EVEYRTHING you hit lel
	{
        GameObject hitParticle = ObjectPoolingScript.current.GetParticles();
		if (System.Object.ReferenceEquals(hitParticle, null))
		{
			return;
		}
        hitParticle.transform.position = hitInfo.raycastHit.point; //sets position of the particle
//======commented out because physics.spherecast is more accurate==============================\\
		//Vector3 incVector = hitInfo.raycastHit.point-hitInfo.shooterPos; //direction of shot
		//Vector3 reflVector = Vector3.Reflect(incVector,hitInfo.raycastHit.normal); //calculates particle's direction
//==============================================================================================\\
		hitParticle.transform.rotation = Quaternion.LookRotation(hitInfo.raycastHit.normal); //changes the particle's direction
        hitParticle.SetActive(true); //shows the particle
		hitInfo.raycastHit.collider.gameObject.SendMessage("Damage", hitInfo,SendMessageOptions.DontRequireReceiver); //sends damage IF it has health
	//	hitInfo.raycastHit.collider.gameObject.SendMessage("KnockBack", hitInfo, SendMessageOptions.DontRequireReceiver); //knocks objectback IF it can be knocked back
	}
    protected void SpawnBlood(HitInfo _info)
    {
        GameObject bloodSpurt = ObjectPoolingScript.current.GetBlood();
        if(System.Object.ReferenceEquals(bloodSpurt,null))
        {
            return;
        }
        bloodSpurt.transform.position = _info.raycastHit.point;
        bloodSpurt.transform.rotation = Quaternion.LookRotation(_info.raycastHit.normal);
        bloodSpurt.SetActive(true);
        _info.raycastHit.collider.gameObject.SendMessage("Damage", _info, SendMessageOptions.DontRequireReceiver); //sends damage IF it has health
    }
	protected virtual void SpawnFakeBullet(HitInfo _info)//spawns a fake bullet and fires it
	{
		GameObject fakeBulletClone = ObjectPoolingScript.current.GetBullets(); //gets bullets
		if (System.Object.ReferenceEquals(fakeBulletClone,null))
		{
			return;
		}
		//sets bullet's transform and target direction
		fakeBulletClone.transform.localPosition = shootPoint.position;
		fakeBulletClone.transform.LookAt(_info.raycastHit.point); //makes it face where it should hit
		fakeBulletClone.GetComponent<TrailRenderer>().time = -1f; //kills the previous trail renderer
		fakeBulletClone.SetActive(true);						//sets it to true so you can see it
		fakeBulletClone.GetComponent<Rigidbody>().AddForce(fakeBulletClone.transform.forward * 100f, ForceMode.Impulse); //applies force to shoot it

	}
   
    protected void DrawLine(HitInfo hitInfo)//Outdated function, keeping just for logic
	{

		bulletLine.SetPosition(0, gameObject.transform.position);
		bulletLine.SetPosition(1, transform.forward * 100);
		//if (bLine)
		//{
		//	lineTime += Time.deltaTime;
		//	endPoint = Vector3.Lerp(gameObject.transform.position, hitInfo.raycastHit.point, lineTime);
		//	bulletLine.SetPosition(1, endPoint);
		//	bulletLine.SetPosition(0, gameObject.transform.position);
		//	if (lineTime > 1)
		//	{
		//		lineTime = 0;
		//		bLine = false;
		//		bulletLine.SetPosition(1, gameObject.transform.position);
		//	}
		//}
	} 
	
	protected int SoundRandomiser(int arrayLength)
    {
        return Random.Range(0, arrayLength-1);
    }
    
    //omfg just use play oneshot. .__. and make a dictionary for the sounds lol.
	//protected void PlaySound(int audioIndex,SoundType _sound,float _reloadDelay = 0f) //Takes in soundIndex and it's type to see what kind of sound to play
	//{
	//	switch (_sound) //Checks which sound is being played
	//	{
 //			case SoundType.FIRESOUND: //if it's a firesound, use the FIRE sounds array
	//			//Randomises sounds 
	//			_audioSources[audioIndex].clip = fireSounds[Random.Range(0,fireSounds.Length-1)]; //changes the audio source's sound clip to the firesound clip
	//			break;
	//		case SoundType.RELOADSOUND: //if it's a reload sound, use the RELOAD sounds array
	//			if (audioIndex > reloadSounds.Length-1) //catches if you exceed array index
	//			{
	//				audioIndex = reloadSounds.Length - 1; //sets it back to LAST thing in array
	//			}
	//			_audioSources[audioIndex].clip = reloadSounds[audioIndex]; //Same as above, sets the appropriate clip for the appropriate sound source
	//			break;
	//	}
	////	_audioSources[audioIndex].PlayOneShot();
	//	_audioSources[audioIndex].PlayDelayed(_reloadDelay); //plays sound with delay associated, 0 if no delay. Number of audioclips must match number of audio sources.
	//	soundIndex++;			//cycle's through the array
	//	if (soundIndex > _audioSources.Length-1) //catches if you exceed the array's length
	//	{
	//		soundIndex = 0;						//resets it back to 0 if you exceed
	//	}
	//}


}
