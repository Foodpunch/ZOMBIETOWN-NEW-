using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    [SerializeField] AudioSource GunShotSounds; // For gunshots
    [SerializeField] AudioSource ReloadSounds; //For Reloads
    [SerializeField] AudioSource NormalSounds; //For misc and normal sounds that don't overlap/play delayed
    public static AudioManager Instance; //makes it so that its static and accessible everywhere

    public enum SFXType { ONESHOT,DELAYED,NORMAL};
    public SFXType _soundtype;
    

	// Use this for initialization
	void Awake () {
        Instance = this; //makes it so that there's only one instance
	}
    void Start()
    {
        GunShotSounds = GunShotSounds.GetComponent<AudioSource>();
        ReloadSounds = ReloadSounds.GetComponent<AudioSource>();
        NormalSounds = NormalSounds.GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	public static void PlaySFX(AudioClip newClip,SFXType sound,float delay = 0f)
    {
        if(sound == SFXType.DELAYED)
        {
            Instance.ReloadSounds.clip = newClip;
            Instance.ReloadSounds.PlayDelayed(delay);
        }
        if(sound == SFXType.ONESHOT)
        {
            Instance.GunShotSounds.PlayOneShot(newClip);
        }
        if(sound == SFXType.NORMAL)
        {
            Instance.NormalSounds.clip = newClip;
            if(!Instance.NormalSounds.isPlaying)
            {
                Instance.NormalSounds.Play();
            }
        }
    }
 
    public static void CrossFade(AudioClip newTrack,float fadeTime = 1.0f) //static function that can be called anywhere
    {
        Instance.StopAllCoroutines();  //stop all the coroutines that THIS INSTANCE is running, so that it doesn't spoil the crossfade if you spam

        if(Instance.GetComponents<AudioSource>().Length > 1) //checks if more than 1 audiosource exists
        {
            Destroy(Instance.GetComponent<AudioSource>()); //destroy the excess audio source
        }
        AudioSource newAudioSource = Instance.gameObject.AddComponent<AudioSource>(); //adds the new audiosource for fade
        newAudioSource.loop = true;             //makes it loop, because BGM
        newAudioSource.volume = 0.0f;            //sets the new volume to 0
        newAudioSource.clip = newTrack;          //clip for new audiosource is the track that is sent into the function
        newAudioSource.Play();                   //start playing the audiosource

        Instance.StartCoroutine(Instance.CrossFadeCoroutine(newAudioSource,fadeTime)); //start the actual coroutine to fade
    }

    IEnumerator CrossFadeCoroutine(AudioSource newSource,float fadeTime)  //coroutine for fading 
    {
        float t = 0.0f;     //starting time always 0
        float initialVolume = GetComponent<AudioSource>().volume; //starting volume to prevent volume popping when spamming
        while (t<fadeTime) //while t is lesser than fade time
        {

            GetComponent<AudioSource>().volume = Mathf.Lerp(initialVolume, 0.0f, t / fadeTime); //lerp original audio DOWN
            newSource.volume = Mathf.Lerp(0.0f, 1.0f, t / fadeTime);  //lerp new audio UP
            
            t += Time.deltaTime;  //just adds time
            yield return null;  //exits routine each step
        }
        newSource.volume = 1.0f;   //once done, just set it to 1
        Destroy(GetComponent<AudioSource>()); //destroy the original audiosource
    }

}
