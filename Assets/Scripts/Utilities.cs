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
	bool bFade;
	float _alpha;

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
		if(!bFade)
		{
			bFade = true;
			_imgHold = _img;
			switch (_effect)
			{
				case ImageEffect.FADEINFADEOUT:
					StartCoroutine("Fade");
					break;
				case ImageEffect.FLASHTOFADE:
					StartCoroutine("Flash");
					break;
			}
		}
	}
	void CheckBooleans()
	{
		if(_imgHold != null)
		{
			_imgHold.color = new Color(_imgHold.color.r, _imgHold.color.g, _imgHold.color.b, _alpha);
		}
		if(camShake > 0)
		{
			//insert camera shake functions here
		}
	}
	IEnumerator Flash()
	{
		_alpha = 1;
		yield return new WaitForSeconds(0.5f);
		for(float j=1;j>0;j-=Time.deltaTime)
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
		yield return new WaitForSeconds(2);
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
