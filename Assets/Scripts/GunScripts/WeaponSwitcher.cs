using UnityEngine;
using System.Collections;

public class WeaponSwitcher : MonoBehaviour {

    [SerializeField] enum WeaponTypes
    {
        PISTOL,
        RIFLE,
        SHOTGUN,
        SNIPER,
        ROCKET
    }
    [SerializeField] WeaponTypes _weapons = WeaponTypes.PISTOL;
    [SerializeField] GameObject[] WeaponModels;
    [SerializeField] GameObject GunBarrel;
    [SerializeField] AudioClip[] tracks;
    int currentTrack = 0;



	// Use this for initialization
	void Start () {
        GunBarrel.GetComponent<LexScript>().enabled = false;
        GunBarrel.GetComponent<BratonScript>().enabled = false;
        GunBarrel.GetComponent<HekScript>().enabled = false;
        GunBarrel.GetComponent<VectisScript>().enabled = false;
        GunBarrel.GetComponent<OgrisScript>().enabled = false;
        
       foreach(GameObject gun in WeaponModels)
        {
            gun.SetActive(false);
        }
	
	}
	
	// Update is called once per frame
	void Update () {
        WeaponLogic();
	}
    void CheckInput(WeaponTypes nextState,int weaponIndex)
    {
        if(Input.GetKeyDown(KeyCode.Q) || GamepadManager.buttonXDown || OVRGamepadController.GPC_GetButtonDown((int)OVRGamepadController.Button.X)) //Controller stuff
        {
            if(nextState == WeaponTypes.ROCKET || nextState == WeaponTypes.PISTOL)
            {
                currentTrack++;
                if(currentTrack >= tracks.Length)
                {
                    currentTrack = 0;
                }
                AudioManager.CrossFade(tracks[currentTrack], 2.0f);
            }

            GunBarrel.GetComponent<LexScript>().enabled = false;
            GunBarrel.GetComponent<BratonScript>().enabled = false;
            GunBarrel.GetComponent<HekScript>().enabled = false;
            GunBarrel.GetComponent<VectisScript>().enabled = false;
            GunBarrel.GetComponent<OgrisScript>().enabled = false;

            WeaponModels[weaponIndex].SetActive(false);
            _weapons = nextState;
        }
    }
    void WeaponLogic()
    {
        switch(_weapons)
        {
            case WeaponTypes.PISTOL:
                GunBarrel.GetComponent<LexScript>().enabled = true;
                WeaponModels[0].SetActive(true);
                CheckInput(WeaponTypes.RIFLE, 0);
                break;
            case WeaponTypes.RIFLE:
                GunBarrel.GetComponent<BratonScript>().enabled = true;
                WeaponModels[1].SetActive(true);
                CheckInput(WeaponTypes.SHOTGUN, 1);
                break;
            case WeaponTypes.SHOTGUN:
                GunBarrel.GetComponent<HekScript>().enabled = true;
                WeaponModels[2].SetActive(true);
                CheckInput(WeaponTypes.SNIPER, 2);
                break;
            case WeaponTypes.SNIPER:
                GunBarrel.GetComponent<VectisScript>().enabled = true;
                WeaponModels[3].SetActive(true);
                CheckInput(WeaponTypes.ROCKET, 3);
                break;
            case WeaponTypes.ROCKET:
                GunBarrel.GetComponent<OgrisScript>().enabled = true;
                WeaponModels[4].SetActive(true);
                CheckInput(WeaponTypes.PISTOL, 4);
                break;
        }
    }
}
