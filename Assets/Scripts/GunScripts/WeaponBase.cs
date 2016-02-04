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

	[SerializeField] Transform shootPoint; //point where the bullet flies out

	protected float _bulletForce; //Force of bullet (500 - weak||  2000 - Mid || 5000 - Strong) Probably should randomise it/calculate it based on range

	public string primaryFire = "Fire1"; //button to shoot
	public string reloadButt = "Reload"; //button to reload

	bool readyToFire = true;
	bool readyToReload = false;
	bool isReloading = false;
	public float fireTime;			//time elapsed while firing

	Text AmmoCounter;				//Ammo text
	Text ReloadText;				//Reload Text

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

	//Line renderer stuff
	LineRenderer bulletLine;			//Line renderer for bullet shots
	Vector3 endPoint;					//End point for the ray
	float lineSpeed = 6f;
	float lineTime;
	bool bLine;
	
	//MUZZLE FLASH
	[SerializeField] Image MuzzleFlash;
	bool bMuzzleFlash;

	//Player Animator
	[SerializeField]protected Animator _playerAnim;


	//Abstract Functions
	
	protected abstract void PrimaryFire();
	// Use this for initialization
	protected virtual void Start () {
		AmmoCounter = GameObject.FindGameObjectWithTag("AmmoCounter").GetComponent<Text>();
		ReloadText = GameObject.FindGameObjectWithTag("Reload").GetComponent<Text>();
		bulletLine = GetComponent<LineRenderer>();
		_audioSources = GetComponentsInChildren<AudioSource>();
		_playerAnim = _playerAnim.GetComponent<Animator>();
		soundIndex = 0;
		MuzzleFlash.enabled = false;
		

//		MuzzleFlare.Stop();
	}
	
	// Update is called once per frame
	void Update () {
		CheckInput();
		UpdateAmmoCounter();
	}
	protected virtual void CheckInput()
	{
		bool buttonPress;
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
		}
		else
		{
			fireTime = 0.0f;
		}
		if (Input.GetButton(reloadButt))
		{
			fireTime = 0.0f;
			if (readyToReload && currentAmmo < maxAmmo) //if gun is read to reload and your ammo is not maxed
			{
				PlaySound(soundIndex, SoundType.RELOADSOUND,reloadDelay); //plays reload sound
				_playerAnim.SetBool("Reload_b", true);
				readyToReload = false;
				readyToFire = false;
				InvokeRepeating("ReloadBullet", 0f, reloadDelay); //calls reload function, either reloads entire clip or individual shells
			}
		}
	}


	void UpdateAmmoCounter() //simple function to show the ammo count
	{
		AmmoCounter.text = currentAmmo.ToString() + "/" + totalAmmo.ToString();
		if (isReloading)
		{
			ReloadText.text = "Reloading...";
		}
		else
		{
			ReloadText.text = "";
		}
	}


	void SetReadyToFire() //allows player to fire again
	{
		readyToFire = true;
		_playerAnim.SetBool("Shoot_b", false);
		MuzzleFlash.enabled = false;
	}
	void ReloadBullet()
	{
		if (currentAmmo == maxAmmo  || totalAmmo == 0) //if clip is full or no ammo left, stop reload
		{
			isReloading = false;	//you're not reloading anymore
			readyToFire = true;		//let it fire again
			_playerAnim.SetBool("Reload_b", false);
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
				PlaySound(soundIndex,SoundType.RELOADSOUND); //plays reload sound
				
				currentAmmo++;
				totalAmmo--;
				isReloading = false; //makes it so that you don't have to spam 'R' to reload everything
			}
		}
	}
	protected void SpawnParticles(HitInfo hitInfo)//Spawns particles on EVEYRTHING you hit lel
	{
		GameObject obj = ObjectPoolingScript.current.GetParticles();
		if (System.Object.ReferenceEquals(obj,null))
		{
			return;
		}
		obj.transform.position = hitInfo.raycastHit.point; //sets position of the particle
//======commented out because physics.spherecast is more accurate==============================\\
		//Vector3 incVector = hitInfo.raycastHit.point-hitInfo.shooterPos; //direction of shot
		//Vector3 reflVector = Vector3.Reflect(incVector,hitInfo.raycastHit.normal); //calculates particle's direction
		obj.transform.rotation = Quaternion.LookRotation(hitInfo.raycastHit.normal); //changes the particle's direction
		obj.SetActive(true); //shows the particle
		hitInfo.raycastHit.collider.gameObject.SendMessage("Damage", hitInfo,SendMessageOptions.DontRequireReceiver); //sends damage IF it has health
		hitInfo.raycastHit.collider.gameObject.SendMessage("KnockBack", hitInfo, SendMessageOptions.DontRequireReceiver); //knocks objectback IF it can be knocked back
	}
	protected void SpawnFakeBullet(HitInfo _info)//spawns a fake bullet and fires it
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
	
	//omfg just use play oneshot. .__. and make a dictionary for the sounds lol.
	protected void PlaySound(int audioIndex,SoundType _sound,float _reloadDelay = 0f) //Takes in soundIndex and it's type to see what kind of sound to play
	{
		switch (_sound) //Checks which sound is being played
		{
 			case SoundType.FIRESOUND: //if it's a firesound, use the FIRE sounds array
				//Randomises sounds 
				_audioSources[audioIndex].clip = fireSounds[Random.Range(0,fireSounds.Length-1)]; //changes the audio source's sound clip to the firesound clip
				break;
			case SoundType.RELOADSOUND: //if it's a reload sound, use the RELOAD sounds array
				if (audioIndex > reloadSounds.Length-1) //catches if you exceed array index
				{
					audioIndex = reloadSounds.Length - 1; //sets it back to LAST thing in array
				}
				_audioSources[audioIndex].clip = reloadSounds[audioIndex]; //Same as above, sets the appropriate clip for the appropriate sound source
				break;
		}
	//	_audioSources[audioIndex].PlayOneShot();
		_audioSources[audioIndex].PlayDelayed(_reloadDelay); //plays sound with delay associated, 0 if no delay. Number of audioclips must match number of audio sources.
		soundIndex++;			//cycle's through the array
		if (soundIndex > _audioSources.Length-1) //catches if you exceed the array's length
		{
			soundIndex = 0;						//resets it back to 0 if you exceed
		}
	}


}
