using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour {

    [HideInInspector]public int ZombieNumber;
    [SerializeField] Text killCounter;
    [SerializeField] Text gameTimeText;
    float killPercentage;
    [SerializeField] Transform SpawnPoint;
    [SerializeField] GameObject[] ZombieAreas;
    GameObject player;
    [SerializeField] AudioSource _BGM;
    [SerializeField] WeaponSwitcher _weaponSwitchScript;

    bool _timer;
    float gameTime;
    bool restart;
    [SerializeField] GameObject winText;
	// Use this for initialization
	void Start () {
        killCounter = killCounter.GetComponent<Text>();
        gameTimeText = gameTimeText.GetComponent<Text>();
        SpawnPoint = SpawnPoint.GetComponent<Transform>();
        _BGM = _BGM.GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        _weaponSwitchScript = _weaponSwitchScript.GetComponent<WeaponSwitcher>();
	}
	
	// Update is called once per frame
	void Update () {
        CheckKillCount();
        gameTimeText.text = Mathf.Round(gameTime*100f)/100f + "";
	    if(_timer)
        {
            gameTime += Time.deltaTime;
        }
	}
    public void StartGame()
    {
        player.transform.position = SpawnPoint.transform.localPosition;
        _BGM.Play();
        _timer = true;
        _weaponSwitchScript.enabled = true;
        foreach(GameObject _area in ZombieAreas)
        {
            _area.SetActive(true);
        }
    }
    void CheckKillCount()
    {
        killPercentage = ZombieNumber * 1.0f / 69f;
        killCounter.text = ZombieNumber + "/69";
        if (killPercentage >= 0.25f)
        {
            killCounter.text = "<color=#F57373>" + ZombieNumber.ToString() + "</color>" + "/69";
        }
        if (killPercentage >= 0.5f)
        {
            killCounter.text = "<color=#F5D773>" + ZombieNumber.ToString() + "</color>" + "/69";
        }
        if (killPercentage == 1)
        {
            killCounter.text = "<color=#73F57C>" + ZombieNumber.ToString() + "</color>" + "/69";
            _timer = false;
            winText.SetActive(true);
          
            if(GamepadManager.AnyButtonPressed() || OVRGamepadController.GPC_GetButtonDown((int)OVRGamepadController.Button.A))
            {
                if (!restart)
                {
                    restart = true;
                    Invoke("Reset", 2f);
                }
            }
        }
    }
    public void AddKillCount()
    {
        ZombieNumber++;
    }
    void Reset()
    {
        Application.LoadLevel(Application.loadedLevel);
        CancelInvoke("Reset");
    }
}
