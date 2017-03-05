using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Utilities : MonoBehaviour {

	public static Utilities instance;


	//Variables for Camera Shaking
	Vector3 _originalPos;
	[SerializeField]Transform _cam;
	Transform _camPos;
	float camShake;
	float shakeAmt;
	
	
	
	//Variables for image functions 
	public enum ImageEffect
	{
		FLASHTOFADE,
		FADEINFADEOUT
	}
	ImageEffect effectType;
	Image _imgHold;
    Image _imgHold2;

	bool bFade;
    bool bFlash;

	float _alpha;
    float _alpha2;


	// Use this for initialization
	void Start () {
		instance = this;
		_camPos = _cam.transform;
		_originalPos = _camPos.localPosition;
		shakeAmt = 2f;
	}
	
	// Update is called once per frame
	void Update () {
		CheckBooleans();
	}

	public void ImageFunction(Image _img, ImageEffect _effect)
	{
        switch (_effect)
        {
            //case ImageEffect.FADEINFADEOUT:
            //    if (!bFade && !bFlash)
            //    {
            //        bFade = true;
            //        _imgHold = _img;
            //        StartCoroutine("Flash2");
            //    }
            //    break;
            case ImageEffect.FLASHTOFADE:
                if (!bFlash)
                {
                    //bFlash = true;
                    _imgHold = _img;
                    StartCoroutine("Flash2");
                    _imgHold2 = _img;
                    StartCoroutine("Flash");
                }
               
                break;
        }
       
       
	}
	void CheckBooleans()
	{
		if(_imgHold != null)
		{
			_imgHold.color = new Color(_imgHold.color.r, _imgHold.color.g, _imgHold.color.b, _alpha);
		}
        if (_imgHold2 != null)
        {
            _imgHold2.color = new Color(_imgHold2.color.r, _imgHold2.color.g, _imgHold2.color.b, _alpha2);
        }

        if (camShake > 0)
		{
			//insert camera shake functions here
		}
	}
	IEnumerator Flash()
	{
		_alpha2 = 1;
		yield return new WaitForSeconds(2f);
		for(float j=1;j>0;j-=Time.deltaTime/2f)
		{
			_alpha2 = j;
			yield return null;
		}
		_alpha2 = 0;
		yield return new WaitForSeconds(2f);
		bFlash = false;
  
	}
    IEnumerator Flash2()
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
        bFade = false;

    }
    IEnumerator Fade()
	{
		for(float i = 0; i < 1;i+=Time.deltaTime*3.5f)
		{
			_alpha = i;
			yield return null;
		}
		_alpha = 1;
		yield return new WaitForSeconds(0.5f);
		for(float k = 1;k>0;k-=Time.deltaTime)
		{
			_alpha = k;
			yield return null;
		}
		_alpha = 0;
		yield return new WaitForSeconds(1);
        bFade = false;

	}
	public static Vector3 OnUnitRect(float x, float z)
	{
		float newX = Random.Range(0, x);
		float newZ = Random.Range(0, z);
		return new Vector3(newX, 0, newZ);
	}
	public float DistanceBetween(Vector3 A, Vector3 B)
	{
		return Vector3.Distance(A, B);
	}
}
