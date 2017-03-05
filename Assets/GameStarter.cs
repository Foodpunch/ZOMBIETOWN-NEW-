using UnityEngine;
using System.Collections;

public class GameStarter : MonoBehaviour, IDamagable<HitInfo> {

    //maybe make a public enum for the UI?
    //shooting different things when the state is set differently
    //will cause them to trigger different UI elements.
    //e.g shooting an object in its start state will call game start etc.
    //shooting the things will trigger appropriate enums


    [SerializeField]enum MenuStates { START, OPTIONS,HELP,CREDITS,QUIT};
    [SerializeField]MenuStates MenuButton;
    [SerializeField] GameObject Gametext;
    [SerializeField]AudioSource MenuSound;

    void Start()
    {
        MenuSound = MenuSound.GetComponent<AudioSource>();
        gameObject.SetActive(false);
        MenuController();

    }

    public void Damage(HitInfo _info)
    {
        if(_info.damage > 0) //will only take one kind of damage because player only has pistols
        {
            gameObject.SetActive(false);
            MenuController();
        }
    }
    void MenuController()
    {
        switch(MenuButton)
        {
            case MenuStates.START:
                GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().StartGame();
                Gametext.SetActive(false);
                MenuSound.Stop();
                break;
            case MenuStates.HELP:
                Debug.Log("call help anim");
                break;
            case MenuStates.OPTIONS:
                Debug.Log("call options anim");
                break;
            case MenuStates.CREDITS:
                Debug.Log("call credits anim");
                break;
            case MenuStates.QUIT:
                Application.Quit();
                break;
        }
    }
}
